using System;
using System.Diagnostics;

namespace Septerra.Core.Hooks
{
    public static unsafe class Main
    {
        private static readonly BattleDispatcher BattleDispatcher = new BattleDispatcher();

        public static Boolean WinMain(IntPtr hInstance, IntPtr hPrevInstance, SByte* lpCmdLine, int nShowCmd, out int result)
        {
            //Debugger.Launch();
            result = 0;
            return false;
        }

        public static Boolean DispatchBattle()
        {
            return BattleDispatcher.Dispatch();
        }
    }
}