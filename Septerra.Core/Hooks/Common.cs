using System;
using System.Diagnostics;

namespace Septerra.Core.Hooks
{
    public static unsafe class Common
    {
        public static Boolean TryShowError(Byte* errorMessage, Int32 errorCode, out Int32 result)
        {
            result = 0;
            return false;
        }
    }
}