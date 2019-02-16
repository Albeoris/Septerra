using System;
using Septerra.Core;
using Septerra.Core.Sources;

namespace Septerra
{
    public class ConvertVssfToMp3Coroutine : ConvertPipelineCoroutine
    {
        public ConvertVssfToMp3Coroutine(ConvertSpec spec)
            : base(spec)
        {
        }

        protected override void Convert(String sourceName, ArraySegment<Byte> sourceData, String target)
        {
            SoundTarget.Instance.Write(sourceData, target, 0);
        }
    }
}