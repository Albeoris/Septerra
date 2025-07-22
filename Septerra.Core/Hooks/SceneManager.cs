using System;

namespace Septerra.Core.Hooks
{
    public static unsafe class SceneManager
    {
        public static Int32 CurrentSceneType
        {
            get => *((Int32*)Main.AddressTable.Setperra_Global_CurrentSceneType);
            set => *((Int32*)Main.AddressTable.Setperra_Global_CurrentSceneType) = value;
        }
    }
}