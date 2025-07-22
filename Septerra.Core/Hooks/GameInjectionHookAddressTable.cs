using System;
using System.Runtime.InteropServices;

namespace Septerra.Core.Hooks;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GameInjectionHookAddressTable
{
    public UInt32 Septerra_WinMain;
    public UInt32 Septerra_HandleWindowsMessage;

    public UInt32 Septerra_Common_ProcessHotKey;
    public UInt32 Septerra_Common_ShowError;
    public UInt32 Septerra_DbRecord_Close;
    public UInt32 Septerra_DbRecord_GetDecompressedSize;
    public UInt32 Septerra_DbRecord_Open;
    public UInt32 Septerra_DbRecord_Read;
    public UInt32 Septerra_DbRecord_Seek;
    public UInt32 Septerra_DispatchBattle;
    public UInt32 Septerra_QtRecord_FindFile;
    public UInt32 Septerra_TxRecord_Acquire;
    public UInt32 Septerra_TxRecord_Find;
    public UInt32 Septerra_TxRecord_FindString;
    public UInt32 Septerra_TxRecord_ReleaseByPointer;
    public UInt32 Septerra_TxRecord_ReleaseByResourceId;
    
    public UInt32 Setperra_Cdecl_Function_407B70;
    public UInt32 Setperra_Cdecl_Function_IncreaseActorBattleTime;
    
    public UInt32 Setperra_Global_CurrentSceneType;
    public UInt32 Setperra_Global_EnemiesCombatList;
    public UInt32 Setperra_Global_ArrayOf3CombatEntries;
    public UInt32 Setperra_Global_AlliesCombatList;
    public UInt32 Setperra_Global_Dword_0x4A2968;
    public UInt32 Setperra_Global_SelectedCharacterInBattle;
    public UInt32 Setperra_Global_EnemiesCombatListCount;
    public UInt32 Setperra_Global_AlliesCombatListCount;
    public UInt32 Setperra_Global_IsAtbDisabled;
    public UInt32 Setperra_Global_SelectedPlayerActor;
    public UInt32 Setperra_Global_BattlePartySize;
    public UInt32 Setperra_Global_DesiredActorToSelect;
}