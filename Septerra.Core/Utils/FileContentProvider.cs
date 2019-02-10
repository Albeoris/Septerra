using System;
using System.IO;
using Septerra.Core;
using Septerra.Core.DB;

namespace Septerra.Core.Hooks
{
    public sealed class FileContentProvider
    {
        private readonly Object _lock = new Object();
        private readonly String _filePath;
        private FileStream _stream;
        private Byte[] _content;

        public FileContentProvider(String filePath, FileStream stream)
        {
            _filePath = filePath;
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public Int64 Length
        {
            get
            {
                if (_content != null)
                    return _content.Length;

                FileStream stream = _stream;
                if (stream != null)
                    return _stream.Length;

                EnsureReaded();
                return _content.Length;
            }
        }

        public Byte[] GetArray()
        {
            EnsureReaded();
            return _content;
        }

        public ArraySegment<Byte> GetArraySegment()
        {
            EnsureReaded();
            return new ArraySegment<Byte>(_content);
        }

        public MemoryStream GetStream()
        {
            return new MemoryStream(_content);
        }

        private void EnsureReaded()
        {
            if (_content != null)
                return;

            lock (_lock)
            {
                _content = NativeFormatProvider.ReadAllBytes(_filePath, _stream);
                _stream = null;
            }
        }
    }
}