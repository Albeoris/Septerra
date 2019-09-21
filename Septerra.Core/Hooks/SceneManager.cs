using System;

namespace Septerra.Core.Hooks
{
    public static unsafe class SceneManager
    {
        public static Int32 CurrentSceneType
        {
            get => *((Int32*)0x004AE978);
            set => *((Int32*)0x004AE978) = value;
        }
    }
}