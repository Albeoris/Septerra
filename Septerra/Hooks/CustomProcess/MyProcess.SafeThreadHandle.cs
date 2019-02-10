using System;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Septerra
{
    internal sealed partial class MyProcess
    {
        [SuppressUnmanagedCodeSecurity]
        internal sealed class SafeThreadHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            internal SafeThreadHandle()
                : base(true)
            {
            }

            internal void InitialSetHandle(IntPtr h)
            {
                this.SetHandle(h);
            }

            protected override bool ReleaseHandle()
            {
                return SafeNativeMethods.CloseHandle(this.handle);
            }
        }
    }
}