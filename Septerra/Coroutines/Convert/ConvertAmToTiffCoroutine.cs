using System;
using Septerra.Core;

namespace Septerra
{
    public class ConvertAmToTiffCoroutine : ConvertPipelineCoroutine
    {
        public ConvertAmToTiffCoroutine(ConvertSpec spec)
            : base(spec)
        {
        }

        protected override void Convert(String sourceName, ArraySegment<Byte> sourceData, String target)
        {
            ImageTarget.Instance.Write(sourceData, target, 4);
        }
    }
}