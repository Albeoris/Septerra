using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace Septerra
{
    internal sealed partial class MyProcess
    {
        [SuppressUnmanagedCodeSecurity]
        [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
        internal static class SafeNativeMethods
        {

            [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, BestFitMapping = false)]
            public static extern Boolean DuplicateHandle(HandleRef hSourceProcessHandle, SafeHandle hSourceHandle, HandleRef hTargetProcess, out SafeFileHandle targetHandle, Int32 dwDesiredAccess, Boolean bInheritHandle, Int32 dwOptions);

            [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
            public static extern IntPtr GetCurrentProcess();

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern Boolean CreatePipe(out SafeFileHandle hReadPipe, out SafeFileHandle hWritePipe, SECURITY_ATTRIBUTES lpPipeAttributes, Int32 nSize);

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern Boolean CloseHandle(IntPtr handle);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern Int32 ResumeThread(IntPtr handle);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
            public static extern Boolean CreateProcess([MarshalAs(UnmanagedType.LPTStr)] String lpApplicationName, StringBuilder lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, Boolean bInheritHandles, Int32 dwCreationFlags, IntPtr lpEnvironment, [MarshalAs(UnmanagedType.LPTStr)] String lpCurrentDirectory, STARTUPINFO lpStartupInfo, out MyProcess.PROCESS_INFORMATION lpProcessInformation);

            [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
            public static extern IntPtr GetStdHandle(Int32 whichHandle);
        }
    }
}