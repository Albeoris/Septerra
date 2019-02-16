using System;
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
}