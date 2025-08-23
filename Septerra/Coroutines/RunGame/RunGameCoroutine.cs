using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using Septerra.Core.Hooks;

namespace Septerra
{
    public sealed class RunGameCoroutine : ICoroutine
    {
        private readonly RunGameSpec _spec;

        public RunGameCoroutine(RunGameSpec spec)
        {
            _spec = spec;
        }

        public void Execute()
        {
            if (!_spec.GameDirectory.IsDataExists)
            {
                UnpackGamePackagesSpecPreprocessor unpackSpec = new UnpackGamePackagesSpecPreprocessor {GameDirectory = _spec.GameDirectory, OutputDirectory = _spec.GameDirectory.DataPath, Convert = true, Rename = true};
                unpackSpec.Preprocess();

                UnpackGamePackagesCoroutine unpack = new UnpackGamePackagesCoroutine(unpackSpec);
                unpack.Execute();
            }

            if (!_spec.SkipSRDownload && !_spec.GameDirectory.IsSRExecutableExists)
            {
                try
                {
                    DownloadSrSpecPreprocessor downloadSrSpec = new() { GameDirectory = _spec.GameDirectory };
                    downloadSrSpec.Preprocess();

                    DownloadSrCoroutine downloadSr = new(downloadSrSpec);
                    downloadSr.Execute();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to download Septerra-SR. To skip this step run the launcher with -skipsrdownload argument.");
                    Console.WriteLine(ex);
                }
            }

            MyProcess process = new MyProcess();
            if (_spec.GameDirectory.IsSRExecutableExists)
                process.StartInfo = new ProcessStartInfo(_spec.GameDirectory.SRExecutablePath, _spec.GameArguments);
            else
                process.StartInfo = new ProcessStartInfo(_spec.GameDirectory.ExecutablePath, _spec.GameArguments);
            process.StartInfo.WorkingDirectory = _spec.GameDirectory.DirectoryPath;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.Start(0x4);

            try
            {
                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();
                process.OutputDataReceived += (s, z) => output.AppendLine(z.Data);
                process.ErrorDataReceived += (s, z) => error.AppendLine(z.Data);
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                var dllPath = _spec.GameInjection.DllPath;
                var unicodeDllPath = Encoding.Unicode.GetBytes(dllPath + '\0');

                String gameInjectionHookAddressTablePath = Path.Combine(_spec.GameDirectory.DirectoryPath, nameof(GameInjectionHookAddressTable));
                using (FileStream addressOutput = File.Create(gameInjectionHookAddressTablePath))
                {
                    unsafe
                    {
                        Int32 size = Marshal.SizeOf<GameInjectionHookAddressTable>();
                        Byte[] buffer = new Byte[size];

                        fixed (Byte* bufferPtr = buffer)
                            Marshal.StructureToPtr(_spec.GameInjection.AddressTable, (IntPtr)bufferPtr, false);

                        addressOutput.Write(buffer, 0, size);
                        addressOutput.Flush();
                    }
                }
                
                using (SafeProcessHandle processHandle = new SafeProcessHandle(process.Id, ProcessAccessFlags.All, false))
                using (SafeVirtualMemoryHandle memoryHandle = processHandle.Allocate(unicodeDllPath.Length, AllocationType.Commit, MemoryProtection.ReadWrite))
                {
                    memoryHandle.Write(unicodeDllPath);

                    IntPtr loadLibraryAddress = GetLoadLibraryAddress();
                    using (SafeRemoteThread thread = processHandle.CreateThread(loadLibraryAddress, memoryHandle))
                    {
                        thread.Join();
                        Thread.Sleep(1000); // TODO:: Wait for DLL_THREAD_DETACH
                    }

                    var dxWndDirectory = Path.GetFullPath("dxwnd");
                    var dxWndPath = dxWndDirectory + "\\dxwnd.exe";
                    if (File.Exists(dxWndPath))
                    {

                        unsafe
                        {
                            Int32 processInformationSize = sizeof(MyProcess.PROCESS_INFORMATION);
                            using (MemoryMappedFile mmf = MemoryMappedFile.CreateNew("Global\\DxWndSuspendedProcessInfo", processInformationSize, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, HandleInheritability.None))
                            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor(0, processInformationSize, MemoryMappedFileAccess.Write))
                            {
                                MyProcess.PROCESS_INFORMATION processInformation = process.ProcessInformation;
                                accessor.Write(0, ref processInformation);

                                Process dxWnd = new Process
                                {
                                    StartInfo = new ProcessStartInfo(dxWndPath, "/r:0")
                                    {
                                        WorkingDirectory = dxWndDirectory
                                    }
                                };

                                dxWnd.Start();

                                process.WaitForExit();
                                dxWnd.Kill();
                            }
                        }
                    }
                    else
                    {
                        process.ResumeMainThread();
                        HideConsole();
                        process.WaitForExit();
                    }
                }
            }
            catch
            {
                process.Kill();
                throw;
            }
        }

        private static IntPtr GetLoadLibraryAddress()
        {
            IntPtr kernelHandle = Kernel32.GetModuleHandle("kernel32.dll");
            if (kernelHandle == IntPtr.Zero)
                throw new Win32Exception();

            IntPtr loadLibraryAddress = Kernel32.GetProcAddress(kernelHandle, "LoadLibraryW");
            if (loadLibraryAddress == IntPtr.Zero)
                throw new Win32Exception();

            return loadLibraryAddress;
        }

        private static void HideConsole()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
    }
}