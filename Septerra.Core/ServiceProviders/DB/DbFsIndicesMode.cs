using System;

namespace Septerra.Core.DB
{
    [Flags]
    public enum DbFsIndicesMode
    {
        OnInitialize = 1,
        OnRequest = 2,
        OnAny = OnInitialize | OnRequest
    }
}