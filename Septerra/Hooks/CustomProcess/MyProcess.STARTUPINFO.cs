using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Septerra
{
    internal sealed partial class MyProcess
    {
        [StructLayout(LayoutKind.Sequential)]
        internal class STARTUPINFO
        {
            public IntPtr lpReserved = IntPtr.Zero;
            public IntPtr lpDesktop = IntPtr.Zero;
            public IntPtr lpTitle = IntPtr.Zero;
            public IntPtr lpReserved2 = IntPtr.Zero;
            public SafeFileHandle hStdInput = new SafeFileHandle(IntPtr.Zero, false);
            public SafeFileHandle hStdOutput = new SafeFileHandle(IntPtr.Zero, false);
            public SafeFileHandle hStdError = new SafeFileHandle(IntPtr.Zero, false);
            public int cb;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;

            public STARTUPINFO()
            {
                this.cb = Marshal.SizeOf((object) this);
            }

            public void Dispose()
            {
                if (this.hStdInput != null && !this.hStdInput.IsInvalid)
                {
                    this.hStdInput.Close();
                    this.hStdInput = (SafeFileHandle) null;
                }
                if (this.hStdOutput != null && !this.hStdOutput.IsInvalid)
                {
                    this.hStdOutput.Close();
                    this.hStdOutput = (SafeFileHandle) null;
                }
                if (this.hStdError == null || this.hStdError.IsInvalid)
                    return;
                this.hStdError.Close();
                this.hStdError = (SafeFileHandle) null;
            }
        }
    }
}