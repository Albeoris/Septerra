using System;
using System.IO;
using Septerra.Core;

namespace Septerra
{
    public class ConvertTxToTxtCoroutine : ConvertPipelineCoroutine
    {
        public ConvertTxToTxtCoroutine(ConvertSpec spec)
            : base(spec)
        {
        }

        protected override void Convert(String sourceName, ArraySegment<Byte> sourceData, String target)
        {
            TextTarget.Instance.Write(sourceData, target, 0);
        }
    }

    public abstract class ConvertPipelineCoroutine : ICoroutine
    {
        private readonly ConvertSpec _spec;

        protected ConvertPipelineCoroutine(ConvertSpec spec)
        {
            _spec = spec;
        }

        public void Execute()
        {
            Byte[] buff = null;

            foreach (var (source, target) in _spec.EnumerateFiles())
            {
                Int32 size;

                using (var sourceFile = File.OpenRead(source))
                {
                    size = (Int32)sourceFile.Length;
                    if (buff == null || buff.Length < size)
                        buff = new Byte[size];

                    sourceFile.EnsureRead(buff, 0, size);
                }

                ArraySegment<Byte> arraySegment = new ArraySegment<Byte>(buff, 0, size);
                Convert(source, arraySegment, target);
            }
        }

        protected abstract void Convert(String sourceName, ArraySegment<Byte> sourceData, String target);
    }
}