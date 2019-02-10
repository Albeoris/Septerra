using System;
using System.Runtime.InteropServices;

namespace Septerra
{
    [StructLayout(LayoutKind.Sequential)]
    public class SECURITY_ATTRIBUTES
    {
        public int nLength = 12;
        public IntPtr lpSecurityDescriptor;
        public bool bInheritHandle;
    }
}