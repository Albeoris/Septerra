using System;
using System.Collections.Generic;

namespace Septerra
{
    public class ConvertSpec
    {
        public Func<IEnumerable<(String source, String target)>> EnumerateFiles { get; protected set; }
    }
}