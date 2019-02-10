using System;

namespace Septerra.Core
{
    public sealed class MultiTarget : ITarget
    {
        private readonly ITarget[] _targets;

        public MultiTarget(params ITarget[] targets)
        {
            _targets = targets;
        }

        public void Write(ArraySegment<Byte> segment, String outputPath, UInt16 expectedVersion)
        {
            foreach (var target in _targets)
                target.Write(segment, outputPath, expectedVersion);
        }
    }
}