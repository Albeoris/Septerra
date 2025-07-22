using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Septerra.WindowsMessages;

namespace Septerra.Core.Hooks
{
    public static unsafe class Main
    {
        static Main()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
            {
                if (args.Exception.Message.Contains(".XmlSerializers"))
                    return;
                
                Log.Error(args.Exception, "UnhandledException");
                Log.Error(new StackTrace(skipFrames: 2).ToString());
            };
            
            using (FileStream addressOutput = File.OpenRead(nameof(GameInjectionHookAddressTable)))
            {
                Int32 size = Marshal.SizeOf<GameInjectionHookAddressTable>();
                Byte[] buffer = addressOutput.ReadToEnd();
                if (buffer.Length != size)
                    throw new InvalidDataException($"File [{nameof(GameInjectionHookAddressTable)}] contains {buffer.Length} bytes but expected number is {size}");

                fixed (Byte* bufferPtr = buffer)
                    AddressTable = Marshal.PtrToStructure<GameInjectionHookAddressTable>(new IntPtr(bufferPtr));
            }
            
            BattleDispatcher = new BattleDispatcher();
        }

        private static readonly BattleDispatcher BattleDispatcher;

        public static GameInjectionHookAddressTable AddressTable { get; }

        public static Boolean WinMain(IntPtr hInstance, IntPtr hPrevInstance, SByte* lpCmdLine, Int32 nShowCmd, out Int32 result)
        {
            //Debugger.Launch();
            result = 0;
            return false;
        }
        
        public static Boolean HandleWindowsMessage(IntPtr hWnd, WindowsMessage message, Int32 wParam, Int32 lParam, out Int32 result)
        {
            result = 0;
            
            switch (message)
            {
                case WindowsMessage.WM_KEYDOWN:
                {
                    KeyDownMessageInfo keyDown = new(wParam, lParam);
                    if (HotKeyManager.TryHandle(keyDown))
                        return true;
                    break;
                }
                case WindowsMessage.WM_KEYUP:
                {
                    KeyUpMessageInfo keyUp = new(wParam, lParam);
                    if (HotKeyManager.TryHandle(keyUp))
                        return true;
                    break;
                }
            }

            //Debugger.Launch();
            return false;
        }

        public static Boolean DispatchBattle()
        {
            return BattleDispatcher.Dispatch();
        }
    }
}