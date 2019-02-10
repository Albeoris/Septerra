using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Septerra
{
    internal sealed partial class MyProcess : Process
    {
        private Boolean _disposed;
        private SafeThreadHandle _mainThread;

        public PROCESS_INFORMATION ProcessInformation { get; set; }

        public bool Start(Int32 flags)
        {
            this.Close();
            ProcessStartInfo startInfo = this.StartInfo;
            if (startInfo.FileName.Length == 0)
                throw new InvalidOperationException("FileNameMissing");
            if (startInfo.UseShellExecute)
                throw new NotSupportedException();
            return this.StartWithCreateProcess(startInfo, flags);
        }

        protected override void Dispose(bool disposing)
        {
            _disposed = true;
            base.Dispose(disposing);
        }

        private Boolean StartWithCreateProcess(ProcessStartInfo startInfo, Int32 flags)
        {
            if (startInfo.StandardOutputEncoding != null && !startInfo.RedirectStandardOutput)
                throw new InvalidOperationException("StandardOutputEncodingNotAllowed");
            if (startInfo.StandardErrorEncoding != null && !startInfo.RedirectStandardError)
                throw new InvalidOperationException("StandardErrorEncodingNotAllowed");
            if (this._disposed)
                throw new ObjectDisposedException(this.GetType().Name);
            StringBuilder stringBuilder = (StringBuilder)Private.s_BuildCommandLine.Invoke(null, new[] {startInfo.FileName, startInfo.Arguments});
            STARTUPINFO lpStartupInfo = new STARTUPINFO();
            MyProcess.PROCESS_INFORMATION lpProcessInformation = new MyProcess.PROCESS_INFORMATION();
            Microsoft.Win32.SafeHandles.SafeProcessHandle processHandle = null;
            SafeThreadHandle safeThreadHandle = new SafeThreadHandle();
            int error = 0;
            SafeFileHandle parentHandle1 = (SafeFileHandle)null;
            SafeFileHandle parentHandle2 = (SafeFileHandle)null;
            SafeFileHandle parentHandle3 = (SafeFileHandle)null;
            GCHandle gcHandle = new GCHandle();

            lock (typeof(Process).GetField("s_CreateProcessLock", BindingFlags.Static | BindingFlags.NonPublic))
            {
                try
                {
                    if (startInfo.RedirectStandardInput || startInfo.RedirectStandardOutput || startInfo.RedirectStandardError)
                    {
                        if (startInfo.RedirectStandardInput)
                            this.CreatePipe(out parentHandle1, out lpStartupInfo.hStdInput, true);
                        else
                            lpStartupInfo.hStdInput = new SafeFileHandle(SafeNativeMethods.GetStdHandle(-10), false);
                        if (startInfo.RedirectStandardOutput)
                            this.CreatePipe(out parentHandle2, out lpStartupInfo.hStdOutput, false);
                        else
                            lpStartupInfo.hStdOutput = new SafeFileHandle(SafeNativeMethods.GetStdHandle(-11), false);
                        if (startInfo.RedirectStandardError)
                            this.CreatePipe(out parentHandle3, out lpStartupInfo.hStdError, false);
                        else
                            lpStartupInfo.hStdError = new SafeFileHandle(SafeNativeMethods.GetStdHandle(-12), false);
                        lpStartupInfo.dwFlags = 256;
                    }

                    int num1 = flags;
                    if (startInfo.CreateNoWindow)
                        num1 |= 134217728;
                    IntPtr num2 = (IntPtr)0;
                    // Not supported
                    //if (startInfo.environmentVariables != null)
                    //{
                    //    bool unicode = false;
                    //    if (ProcessManager.IsNt)
                    //    {
                    //        num1 |= 1024;
                    //        unicode = true;
                    //    }

                    //    gcHandle = GCHandle.Alloc((object)EnvironmentBlock.ToByteArray(startInfo.environmentVariables, unicode), GCHandleType.Pinned);
                    //    num2 = gcHandle.AddrOfPinnedObject();
                    //}

                    string lpCurrentDirectory = startInfo.WorkingDirectory;
                    if (lpCurrentDirectory == String.Empty)
                        lpCurrentDirectory = Environment.CurrentDirectory;
                    if (startInfo.UserName.Length != 0)
                        throw new NotSupportedException("startInfo.UserName.Length != 0");
                    else
                    {
                        RuntimeHelpers.PrepareConstrainedRegions();
                        bool process;
                        try
                        {
                        }
                        finally
                        {
                            process = SafeNativeMethods.CreateProcess((string)null, stringBuilder, IntPtr.Zero, IntPtr.Zero, true, num1, num2, lpCurrentDirectory, lpStartupInfo, out lpProcessInformation);
                            if (!process)
                                error = Marshal.GetLastWin32Error();
                            if (lpProcessInformation.hProcess != (IntPtr)0 && lpProcessInformation.hProcess != new IntPtr(-1))
                                processHandle = new Microsoft.Win32.SafeHandles.SafeProcessHandle(lpProcessInformation.hProcess, true);
                            if (lpProcessInformation.hThread != (IntPtr)0 && lpProcessInformation.hThread != new IntPtr(-1))
                                safeThreadHandle.InitialSetHandle(lpProcessInformation.hThread);
                        }

                        if (!process)
                        {
                            if (error == 193 || error == 216)
                                throw new Win32Exception(error, "InvalidApplication");
                            throw new Win32Exception(error);
                        }
                    }
                }
                finally
                {
                    if (gcHandle.IsAllocated)
                        gcHandle.Free();
                    lpStartupInfo.Dispose();
                }
            }

            if (startInfo.RedirectStandardInput)
            {
                var sw = new StreamWriter((Stream)new FileStream(parentHandle1, FileAccess.Write, 4096, false), Console.InputEncoding, 4096);
                sw.AutoFlush = true;

                Private.standardInput.SetValue(this, sw);
            }

            if (startInfo.RedirectStandardOutput)
            {
                Encoding encoding = startInfo.StandardOutputEncoding != null ? startInfo.StandardOutputEncoding : Console.OutputEncoding;
                var sr = new StreamReader((Stream)new FileStream(parentHandle2, FileAccess.Read, 4096, false), encoding, true, 4096);

                Private.standardOutput.SetValue(this, sr);
            }

            if (startInfo.RedirectStandardError)
            {
                Encoding encoding = startInfo.StandardErrorEncoding != null ? startInfo.StandardErrorEncoding : Console.OutputEncoding;
                var sr = new StreamReader((Stream)new FileStream(parentHandle3, FileAccess.Read, 4096, false), encoding, true, 4096);

                Private.standardError.SetValue(this, sr);
            }

            bool flag = false;
            if (processHandle != null && !processHandle.IsInvalid)
            {
                Private.SetProcessHandle.Invoke(this, new[] {processHandle});
                Private.SetProcessId.Invoke(this, new Object[] {lpProcessInformation.dwProcessId});
                ProcessInformation = lpProcessInformation;
                _mainThread = safeThreadHandle;
                flag = true;
            }

            return flag;
        }

        private static void CreatePipeWithSecurityAttributes(out SafeFileHandle hReadPipe, out SafeFileHandle hWritePipe, SECURITY_ATTRIBUTES lpPipeAttributes, int nSize)
        {
            if (!SafeNativeMethods.CreatePipe(out hReadPipe, out hWritePipe, lpPipeAttributes, nSize) || hReadPipe.IsInvalid || hWritePipe.IsInvalid)
                throw new Win32Exception();
        }

        private void CreatePipe(out SafeFileHandle parentHandle, out SafeFileHandle childHandle, bool parentInputs)
        {
            SECURITY_ATTRIBUTES lpPipeAttributes = new SECURITY_ATTRIBUTES();
            lpPipeAttributes.bInheritHandle = true;
            SafeFileHandle safeFileHandle = (SafeFileHandle)null;
            try
            {
                if (parentInputs)
                    CreatePipeWithSecurityAttributes(out childHandle, out safeFileHandle, lpPipeAttributes, 0);
                else
                    CreatePipeWithSecurityAttributes(out safeFileHandle, out childHandle, lpPipeAttributes, 0);
                if (!SafeNativeMethods.DuplicateHandle(new HandleRef((object)this, SafeNativeMethods.GetCurrentProcess()), (SafeHandle)safeFileHandle, new HandleRef((object)this, SafeNativeMethods.GetCurrentProcess()), out parentHandle, 0, false, 2))
                    throw new Win32Exception();
            }
            finally
            {
                if (safeFileHandle != null && !safeFileHandle.IsInvalid)
                    safeFileHandle.Close();
            }
        }

        public void ResumeMainThread()
        {
            var mainThread = Threads[0];
            if (mainThread.ThreadState == ThreadState.Wait)
            {
                if (SafeNativeMethods.ResumeThread(_mainThread.DangerousGetHandle()) < 0)
                    throw new Win32Exception();
            }
            else
            {
                throw new NotSupportedException(mainThread.ThreadState.ToString());
            }
        }
    }
}