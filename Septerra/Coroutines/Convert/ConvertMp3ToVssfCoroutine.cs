using System;
using System.IO;

namespace Septerra
{
    public class ConvertMp3ToVssfCoroutine : ConvertPipelineCoroutine
    {
        public ConvertMp3ToVssfCoroutine(ConvertSpec spec)
            : base(spec)
        {
        }

        protected override void Convert(String sourceName, ArraySegment<Byte> sourceData, String target)
        {
            using (var output = File.Create(target))
            {
                output.WriteByte(0x56); // V
                output.WriteByte(0x53); // S
                output.WriteByte(0x53); // S
                output.WriteByte(0x46); // F

                output.Write(sourceData.Array, sourceData.Offset, sourceData.Count);
            }
        }
    }
}