using System;
using System.Diagnostics;

namespace Septerra.Core.Hooks
{
    public static unsafe class Common
    {
        public static Boolean TryShowError(Byte* errorMessage, Int32 errorCode, out Int32 result)
        {
            String error = new String((SByte*)errorMessage);
            Log.Error($"Internal game error ({errorCode}): {error}");

            Debugger.Launch();

            result = 0;
            return false;
        }
    }
}