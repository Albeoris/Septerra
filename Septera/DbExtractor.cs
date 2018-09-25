using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using SepteraNative;

namespace Septera
{
    public sealed class DbExtractor : IDisposable
    {
        private readonly TerrabuilderVersion _version;
        private readonly String _outputDirectory;

        private readonly Thread[] _threads;
        private readonly Semaphore _semaphore;
        private readonly Queue<Tuple<DbPackage, IdxEntry>> _queue;
        private readonly List<Exception> _exceptions;
        private volatile Boolean _disposed;

        public DbExtractor(TerrabuilderVersion version, String outputDirectory)
        {
            _version = version;
            _outputDirectory = outputDirectory ?? throw new ArgumentNullException(nameof(outputDirectory));

            Int32 count = Environment.ProcessorCount;

            _threads = new Thread[count];
            _semaphore = new Semaphore(count, count);
            _queue = new Queue<Tuple<DbPackage, IdxEntry>>(count);
            _exceptions = new List<Exception>(count);

            for (Int32 i = 0; i < _threads.Length; i++)
            {
                Thread thread = new Thread(Extract);
                _threads[i] = thread;

                thread.Start(new ThreadContext());
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            lock (_queue)
                Monitor.PulseAll(_queue);

            foreach (Thread thread in _threads)
                thread.Join();

            _semaphore.Dispose();

            CheckExceptions();
        }

        private void CheckExceptions()
        {
            lock (_exceptions)
            {
                if (_exceptions.Count == 0)
                    return;

                if (_exceptions.Count == 1)
                    ExceptionDispatchInfo.Capture(_exceptions[0]).Throw();
                else
                    throw new AggregateException("Failed to extract files.", _exceptions);
            }
        }

        public void Enqueue(DbPackage package, IdxEntry entry)
        {
            if (_disposed)
                throw new ObjectDisposedException("DbExtractor already disposed.");

            if (_exceptions.Count > 0)
                Dispose();

            _semaphore.WaitOne();

            lock (_queue)
            {
                _queue.Enqueue(Tuple.Create(package, entry));
                Monitor.Pulse(_queue);
            }
        }

        private void Extract(Object state)
        {
            ThreadContext context = (ThreadContext)state;
            try
            {

                while (true)
                {
                    Tuple<DbPackage, IdxEntry> task;

                    lock (_queue)
                    {
                        if (_queue.Count == 0)
                        {
                            if (_disposed)
                                return;

                            Monitor.Wait(_queue);
                        }

                        lock (_queue)
                        {
                            if (_queue.Count == 0)
                                continue;

                            task = _queue.Dequeue();
                            _semaphore.Release();
                        }
                    }

                    Extract(task.Item1, task.Item2, context);
                }
            }
            catch (Exception ex)
            {
                lock (_exceptions)
                    _exceptions.Add(ex);
            }
            finally
            {
                context.Stream?.Dispose();
                context.Buffer = null;
            }
        }

        private sealed class ThreadContext
        {
            public volatile DbPackage Package;
            public volatile FileStream Stream;
            public volatile Byte[] Buffer;
            public readonly Decompressor Decompressor = new Decompressor();
        }

        private void Extract(DbPackage package, IdxEntry entry, ThreadContext context)
        {
            if (context.Package != package)
            {
                context.Stream?.Dispose();
                context.Stream = File.OpenRead(package.FullPath);
                context.Package = package;
            }

            Byte[] buff = context.Buffer;
            if (buff == null)
            {
                buff = new Byte[entry.UncompressedSize];
                context.Buffer = buff;
            }
            else if (buff.Length < entry.UncompressedSize)
            {
                Array.Resize(ref buff, entry.UncompressedSize);
                context.Buffer = buff;
            }

            FileStream inputStream = context.Stream;
            if (inputStream.Position != entry.Offset)
                inputStream.Position = entry.Offset;

            if (entry.IsCompressed)
            {
                unsafe
                {
                    fixed (Byte* ptr = buff)
                    {
                        Int32 readedSize = context.Decompressor.ReadCompressedFile(entry.CompressedSize, ptr, inputStream);
                        if (readedSize != entry.UncompressedSize)
                            throw new EndOfStreamException($"Failed to decompress the file {package.Name}:{entry.Offset}");
                    }
                }
            }
            else
            {
                Int32 offset = 0;
                Int32 size = entry.UncompressedSize;
                while (size > 0)
                {
                    Int32 readed = inputStream.Read(buff, offset, size);
                    if (readed == 0 && size > 0)
                        throw new EndOfStreamException($"Failed to copy the file {package.Name}:{entry.Offset}");

                    size -= readed;
                    offset += readed;
                }
            }

            String directoryPath = Path.Combine(_outputDirectory, package.Name);

            if (entry.CompressedSize < 8)
                throw new NotSupportedException($"Compressed size is too small: {entry.CompressedSize}");

            void CheckVersion(String name, Int32 supported)
            {
                Int32 version = Int32.Parse(Encoding.ASCII.GetString(buff, 2, 2), NumberStyles.Integer, CultureInfo.InvariantCulture);
                if (version != supported)
                    throw new NotSupportedException($"Invalid {name} version: {version}. Expected: {supported}");
            }

            String GetOutputPath(String tag)
            {
                return Path.Combine(directoryPath, $"{entry.Id:D4}.{tag.ToLowerInvariant()}");
            }

            String outputPath;

            ITarget target;

            String magicTag = Encoding.ASCII.GetString(buff, 0, 2);
            switch (magicTag)
            {
                case "LV":
                    CheckVersion(magicTag, _version.LV);
                    outputPath = GetOutputPath(magicTag);
                    target = CopyTarget.Instance;
                    break;
                case "AM":
                    CheckVersion(magicTag, _version.AM);
                    outputPath = GetOutputPath(magicTag);
                    target = CopyTarget.Instance;
                    break;
                case "GV":
                    CheckVersion(magicTag, _version.GV);
                    outputPath = GetOutputPath(magicTag);
                    target = CopyTarget.Instance;
                    break;
                case "TX":
                    CheckVersion(magicTag, _version.TX);
                    outputPath = GetOutputPath("txt");
                    target = TextTarget.Instance;
                    break;
                case "CH":
                    CheckVersion(magicTag, _version.CH);
                    outputPath = GetOutputPath(magicTag);
                    target = CopyTarget.Instance;
                    break;
                case "IL":
                    CheckVersion(magicTag, _version.IL);
                    outputPath = GetOutputPath(magicTag);
                    target = CopyTarget.Instance;
                    break;
                case "VS":
                    outputPath = GetOutputPath("vssf");
                    target = CopyTarget.Instance;
                    break;
                default:
                {
                    if (Encoding.ASCII.GetString(buff, 4, 4) != "moov")
                        throw new NotSupportedException($"Invalid magic number: {Encoding.ASCII.GetString(buff, 0, 8)}");

                    directoryPath = _outputDirectory;
                    outputPath = GetOutputPath("qt");
                    target = CopyTarget.Instance;
                    break;
                }
            }

            Directory.CreateDirectory(directoryPath);

            ArraySegment<Byte> segment = new ArraySegment<Byte>(buff, 0, entry.UncompressedSize);
            target.Write(segment, outputPath);
        }
    }
}