#pragma once

namespace SepterraInjection
{
    struct UnmanagedGameInjectionHookAddressTable
    {
        uint32_t Septerra_WinMain;
        uint32_t Septerra_HandleWindowsMessage;

        uint32_t Septerra_Common_ProcessHotKey;
        uint32_t Septerra_Common_ShowError;
        uint32_t Septerra_DbRecord_Close;
        uint32_t Septerra_DbRecord_GetDecompressedSize;
        uint32_t Septerra_DbRecord_Open;
        uint32_t Septerra_DbRecord_Read;
        uint32_t Septerra_DbRecord_Seek;
        uint32_t Septerra_DispatchBattle;
        uint32_t Septerra_QtRecord_FindFile;
        uint32_t Septerra_TxRecord_Acquire;
        uint32_t Septerra_TxRecord_Find;
        uint32_t Septerra_TxRecord_FindString;
        uint32_t Septerra_TxRecord_ReleaseByPointer;
        uint32_t Septerra_TxRecord_ReleaseByResourceId;
    
        uint32_t Setperra_Cdecl_Function_407B70;
        uint32_t Setperra_Cdecl_Function_IncreaseActorBattleTime;
    
        uint32_t Setperra_Global_CurrentSceneType;
        uint32_t Setperra_Global_EnemiesCombatList;
        uint32_t Setperra_Global_ArrayOf3CombatEntries;
        uint32_t Setperra_Global_AlliesCombatList;
        uint32_t Setperra_Global_Dword_0x4A2968;
        uint32_t Setperra_Global_SelectedCharacterInBattle;
        uint32_t Setperra_Global_EnemiesCombatListCount;
        uint32_t Setperra_Global_AlliesCombatListCount;
        uint32_t Setperra_Global_IsAtbDisabled;
        uint32_t Setperra_Global_SelectedPlayerActor;
        uint32_t Setperra_Global_BattlePartySize;
        uint32_t Setperra_Global_DesiredActorToSelect;
    };
}