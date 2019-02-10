using System;
using System.IO;
using Septerra.Core;
using Septerra.Core.Sources;

namespace Septerra
{
    public class ConvertTiffToAmCoroutine : ConvertPipelineCoroutine
    {
        private readonly TiffImageReader _reader = new TiffImageReader();

        public ConvertTiffToAmCoroutine(ConvertSpec spec)
            : base(spec)
        {
        }

        protected override void Convert(String sourceName, ArraySegment<Byte> sourceData, String target)
        {
            using (MemoryStream ms = new MemoryStream(sourceData.Array, sourceData.Offset, sourceData.Count))
            using (FileStream output = File.Create(target))
            {
                UnsafeList<Byte> result = _reader.Read(sourceName, ms);
                ArraySegment<Byte> buff = result.GetBuffer();
                output.Write(buff.Array, buff.Offset, buff.Count);
            }
        }
    }
}