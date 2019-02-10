using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using Septerra;
using Septerra.Core.Hooks;

namespace Septerra.Core.Utils
{
    public sealed class FileSystemCommittedWatcher : FileSystemWatcher
    {
        private readonly CommittedQueue _queue = new CommittedQueue();
        private readonly IFileSystemWatcherFilter _filter;

        public event Action<String, FileContentProvider> Commited;

        public FileSystemCommittedWatcher(String path, IFileSystemWatcherFilter filter)
            : base(path, "*")
        {
            _filter = filter;

            this.Changed += OnChanged;
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
                _queue.Dispose();

            base.Dispose(disposing);
        }

        private void OnChanged(Object sender, FileSystemEventArgs e)
        {
            if (_filter.CanProcessChanged(e.FullPath))
                _queue.TryRegister(e.FullPath, RaiseCommited);
        }

        private void RaiseCommited(String filePath, FileContentProvider fileStream)
        {
            Commited?.Invoke(filePath, fileStream);
        }

        private sealed class CommittedQueue : IDisposable
        {
            private readonly HashSet<String> _knownPaths = new HashSet<String>();
            private readonly Queue<Item> _inputQueue = new Queue<Item>();
            private readonly Queue<Item> _internalQueue = new Queue<Item>();
            private readonly Thread _thread;
            private Boolean _disposed;

            public CommittedQueue()
            {
                _thread = new Thread(Dispatch);
                _thread.IsBackground = true;
                _thread.Start();
            }
            
            public void Dispose()
            {
                _disposed = true;
                Monitor.Pulse(_inputQueue);

                _thread.Join();

                _internalQueue.Clear();

                lock (_inputQueue)
                    _inputQueue.Clear();
            }

            public Boolean TryRegister(String filePath, Action<String, FileContentProvider> callback)
            {
                if (_disposed)
                    throw new ObjectDisposedException(this.ToString());

                lock (_knownPaths)
                    if (!_knownPaths.Add(filePath))
                        return false;

                lock (_inputQueue)
                {
                    _inputQueue.Enqueue(new Item(filePath, callback));
                    Monitor.Pulse(_inputQueue);
                }

                return true;
            }

            private void Dispatch()
            {
                try
                {
                    while (!_disposed)
                    {
                        if (_internalQueue.Count == 0)
                        {
                            lock (_inputQueue)
                            {
                                if (_inputQueue.Count == 0)
                                    Monitor.Wait(_inputQueue);

                                while (_inputQueue.Count > 0)
                                    _internalQueue.Enqueue(_inputQueue.Dequeue());
                            }
                        }

                        DateTime now = DateTime.UtcNow;
                        TimeSpan max = TimeSpan.Zero;

                        for (Int32 i = 0; i < _internalQueue.Count; i++)
                        {
                            Item item = _internalQueue.Dequeue();

                            TimeSpan delta = item.DeltaTime - (now - item.CheckTime).Duration();
                            if (delta.Ticks > 0)
                            {
                                _internalQueue.Enqueue(item);

                                if (delta > max)
                                    max = delta;

                                continue;
                            }

                            Process(item);
                        }

                        if (_disposed)
                            return;

                        Thread.Sleep(max);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[CommittedQueue] Fatal error.");
                    _disposed = true;

                    _internalQueue.Clear();

                    lock (_inputQueue)
                        _inputQueue.Clear();
                }
            }

            private void Process(Item item)
            {
                try
                {
                    const Int32 genericRead = unchecked((Int32)0x80000000);

                    using (SafeFileHandle ptr = Kernel32.CreateFile(item.FilePath, genericRead, FileShare.Read, null, FileMode.Open, FileOptions.SequentialScan, IntPtr.Zero))
                    {
                        if (!ptr.IsInvalid)
                        {
                            Unregister(item);

                            using (FileStream stream = new FileStream(ptr, FileAccess.Read))
                                item.Callback(item.FilePath, new FileContentProvider(item.FilePath, stream));

                            return;
                        }
                    }

                    Int32 error = Marshal.GetLastWin32Error();
                    switch (error)
                    {
                        case 2: // ERROR_FILE_NOT_FOUND
                        case 3: // ERROR_PATH_NOT_FOUND
                        case 4: // ERROR_TOO_MANY_OPEN_FILES
                        case 5: // ERROR_ACCESS_DENIED
                        case 206: // ERROR_FILENAME_EXCED_RANGE
                        default:
                            Unregister(item);
                            Log.Error(new Win32Exception(error), $"[CommittedQueue] Failed to process file [{item.FilePath}].");
                            return;

                        case 32: // ERROR_SHARING_VIOLATION
                            if (item.DeltaTime.TotalMinutes < 1)
                                item.DeltaTime = new TimeSpan((Int64)(item.DeltaTime.Ticks * 1.2));
                            item.CheckTime = DateTime.UtcNow;
                            _internalQueue.Enqueue(item);
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, $"[CommittedQueue] Cannot process file [{item.FilePath}].");
                    _internalQueue.Enqueue(item);
                }
            }

            private void Unregister(Item item)
            {
                lock (_knownPaths)
                    _knownPaths.Remove(item.FilePath);
            }

            private sealed class Item
            {
                public Item(String filePath, Action<String, FileContentProvider> callback)
                {
                    FilePath = filePath;
                    CheckTime = DateTime.UtcNow;
                    DeltaTime = TimeSpan.FromSeconds(2);
                    Callback = callback;
                }

                public String FilePath { get; }
                public DateTime CheckTime { get; set; }
                public TimeSpan DeltaTime { get; set; }
                public Action<String, FileContentProvider> Callback { get; }
            }
        }
    }
}