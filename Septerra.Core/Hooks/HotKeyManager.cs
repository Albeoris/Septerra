using System;
using System.Collections.Generic;

namespace Septerra.Core.Hooks
{
    public static class HotKeyManager
    {
        public static HotKey Current { get; set; }

        public static bool TryHandle(UInt32 hotKey)
        {
            Current = 0;

            switch (SceneManager.CurrentSceneType)
            {
                case 4:
                    return ProcessBattle4(hotKey);
                default:
                    return false;

            }
        }

        private static Boolean ProcessBattle4(UInt32 hotKey)
        {
            if (hotKey == 'F')
            {
                Current = HotKey.NextTurn;
                return true;
            }

            return false;
        }

        public static Boolean TryEvict(HotKey hotKey)
        {
            if (Current == hotKey)
            {
                Current = HotKey.None;
                return true;
            }

            return false;
        }
    }
}