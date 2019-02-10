using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Septerra.Core.Utils;

namespace Septerra.Core.Hooks
{
    public sealed class FileSystemWatchService : IService
    {
        private Int64 _id = Int64.MinValue;

        private readonly Dictionary<String, List<Item>> _dic;
        private readonly FileSystemCommittedWatcher _fsWatcher;
        private readonly String _directoryPath;

        public FileSystemWatchService()
        {
            _directoryPath = Services<InteractionService>.Instance.DataDirectoryPath;
            _dic  = new Dictionary<String, List<Item>>(StringComparer.OrdinalIgnoreCase);
            _fsWatcher = new FileSystemCommittedWatcher(_directoryPath, new FSFilter(this));
            _fsWatcher.Commited += OnCommited;
            _fsWatcher.Renamed += OnRenamed;
            _fsWatcher.IncludeSubdirectories = true;
            _fsWatcher.EnableRaisingEvents = true;
        }

        private void OnCommited(String filePath, FileContentProvider contentProvider)
        {
            CheckFilePath(filePath);

            Action<FileContentProvider>[] callbacks;
            lock (_dic)
            {
                if (!_dic.TryGetValue(filePath, out var list))
                    return;

                callbacks = list.Select(i => i.Callback).ToArray();
            }

            foreach (var callback in callbacks)
                callback(contentProvider);
        }

        private void OnRenamed(Object sender, RenamedEventArgs e)
        {
            String oldPath = e.OldFullPath;
            CheckFilePath(oldPath);

            String newPath = e.FullPath;
            CheckFilePath(newPath);

            lock (_dic)
            {
                List<Item> list;

                if (_dic.TryGetValue(newPath, out list))
                {
                    while (list.Count > 0)
                        list[0].Dispose();
                }

                if (_dic.TryGetValue(oldPath, out list))
                {
                    _dic[newPath] = list;
                    _dic.Remove(oldPath);
                }
            }
        }

        public IDisposable Register(String filePath, Action<FileContentProvider> action)
        {
            CheckFilePath(filePath);

            lock (_dic)
            {
                if (!_dic.TryGetValue(filePath, out var list))
                {
                    list = new List<Item>(1);
                    _dic.Add(filePath, list);
                }

                Item item = new Item(this, _id++, filePath, action);
                list.Add(item);

                return item;
            }
        }

        private Boolean IsWatching(String filePath)
        {
            CheckFilePath(filePath);

            lock (_dic)
                return _dic.ContainsKey(filePath);
        }

        private void CheckFilePath(String filePath)
        {
            if (!filePath.StartsWith(_directoryPath, StringComparison.OrdinalIgnoreCase))
                throw new NotSupportedException($"Cannot handle file [{filePath}] outside the data directory [{_directoryPath}]");
        }

        private sealed class Item : IDisposable
        {
            private readonly Int64 _id;
            private readonly String _filePath;
            private readonly FileSystemWatchService _service;
            public readonly Action<FileContentProvider> Callback;

            private Boolean _isDisposed = false;

            public Item(FileSystemWatchService service, Int64 id, String filePath, Action<FileContentProvider> callback)
            {
                _service = service;
                _id = id;
                _filePath = filePath;
                Callback = callback;
            }

            public void Dispose()
            {
                if (_isDisposed)
                    return;

                Dictionary<String, List<Item>> dic = _service._dic;
                lock (dic)
                {
                    if (_isDisposed)
                        return;

                    if (!dic.TryGetValue(_filePath, out var list))
                        throw new ObjectDisposedException(_filePath);

                    Int32 index = list.IndexOf(this);
                    if (index >= 0)
                        list.RemoveAt(index);
                    else
                        throw new ObjectDisposedException(_filePath + _id);

                    if (list.Count == 0)
                        dic.Remove(_filePath);
                }
            }
        }

        private sealed class FSFilter : IFileSystemWatcherFilter
        {
            private readonly FileSystemWatchService _self;

            public FSFilter(FileSystemWatchService self)
            {
                _self = self;
            }

            public Boolean CanProcessChanged(String filePath)
            {
                return _self.IsWatching(filePath);
            }
        }
    }
}