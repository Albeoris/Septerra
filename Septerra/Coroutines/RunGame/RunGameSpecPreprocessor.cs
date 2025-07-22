using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CsDebugScript.DwarfSymbolProvider;
using PeNet;
using PeNet.Header.Pe;
using Septerra.Core.Hooks;

namespace Septerra
{
    public sealed class RunGameSpecPreprocessor : RunGameSpec
    {
        public String GameDirectoryPath { get; set; }

        public void Preprocess()
        {
            if (GameDirectory == null)
            {
                GameDirectory = new GameDirectoryDescriptor(Path.GetFullPath(GameDirectoryPath));
                if (!GameDirectory.IsMftExists)
                    throw new FileNotFoundException($"Cannot find a game archive file descriptor ({GameDirectory.MftPath}).");
            }

            if (GameInjection == null)
            {
                var dllPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Septerra.Injection.dll");
                if (!File.Exists(dllPath))
                    throw new FileNotFoundException(dllPath);

                GameInjection = new GameInjectionDescriptor { DllPath = dllPath };

                if (GameDirectory.IsSRExecutableExists)
                    GameInjection.AddressTable = ResolveSrInjectionParams();
                else
                    GameInjection.AddressTable = ResolveNativeInjectionParams();
            }
        }

        private GameInjectionHookAddressTable ResolveNativeInjectionParams()
        {
            return new GameInjectionHookAddressTable
            {
                Septerra_WinMain = 0x00412B80,
                Septerra_HandleWindowsMessage = 0x00411730,
                
                Septerra_Common_ProcessHotKey = 0x0040A8D0,
                Septerra_Common_ShowError = 0x00410960,
                Septerra_DbRecord_Close = 0x00445EC0,
                Septerra_DbRecord_GetDecompressedSize = 0x00445F30,
                Septerra_DbRecord_Open = 0x00445900,
                Septerra_DbRecord_Read = 0x00445C80,
                Septerra_DbRecord_Seek = 0x00445BE0,
                Septerra_DispatchBattle = 0x00430500,
                Septerra_QtRecord_FindFile = 0x00445B60,
                Septerra_TxRecord_Acquire = 0x0040D1D0,
                Septerra_TxRecord_Find = 0x0040D3F0,
                Septerra_TxRecord_FindString = 0x00442F40,
                Septerra_TxRecord_ReleaseByPointer = 0x0040D350,
                Septerra_TxRecord_ReleaseByResourceId = 0x0040E370,
                
                Setperra_Cdecl_Function_407B70 = 0x00407B70,
                Setperra_Cdecl_Function_IncreaseActorBattleTime = 0x00442310,
                    
                Setperra_Global_CurrentSceneType = 0x004AE978,
                Setperra_Global_EnemiesCombatList = 0x004C9100,
                Setperra_Global_ArrayOf3CombatEntries = 0x004BF040,
                Setperra_Global_AlliesCombatList = 0x004C13F8,
                Setperra_Global_Dword_0x4A2968 = 0x004A2968,
                Setperra_Global_SelectedCharacterInBattle = 0x004A2678,
                Setperra_Global_EnemiesCombatListCount =  0x004AF8C4,
                Setperra_Global_AlliesCombatListCount = 0x004AF8C8,
                Setperra_Global_IsAtbDisabled = 0x004D0814,
                Setperra_Global_SelectedPlayerActor = 0x004DE624,
                Setperra_Global_BattlePartySize = 0x004AF8CC,
                Setperra_Global_DesiredActorToSelect = 0x004A295C,
            };
        }

        private GameInjectionHookAddressTable ResolveSrInjectionParams()
        {
            Dictionary<String, UInt32> symboldAddresses = GetFunctionAddressesFromDwarfDebugSymbols();

            return new GameInjectionHookAddressTable
            {
                Septerra_WinMain = symboldAddresses["WinMain_"],
                Septerra_HandleWindowsMessage = symboldAddresses["loc_411730"],
                Septerra_Common_ProcessHotKey = symboldAddresses["loc_40A8D0"],
                Septerra_Common_ShowError = symboldAddresses["loc_410960"],
                Septerra_DbRecord_Close = symboldAddresses["_RecordClose"],
                Septerra_DbRecord_GetDecompressedSize = symboldAddresses["_RecordGetSize"],
                Septerra_DbRecord_Open = symboldAddresses["_RecordOpen"],
                Septerra_DbRecord_Read = symboldAddresses["_RecordRead"],
                Septerra_DbRecord_Seek = symboldAddresses["_RecordSeek"],
                Septerra_DispatchBattle = symboldAddresses["loc_430500"],
                Septerra_QtRecord_FindFile = symboldAddresses["_RecordGetDataFilePathAndOffset"],
                Septerra_TxRecord_Acquire = symboldAddresses["loc_40D1D0"],
                Septerra_TxRecord_Find = symboldAddresses["loc_40D3F0"],
                Septerra_TxRecord_FindString = symboldAddresses["loc_442F40"],
                Septerra_TxRecord_ReleaseByPointer = symboldAddresses["loc_40D350"],
                Septerra_TxRecord_ReleaseByResourceId = symboldAddresses["loc_40E370"],
                
                Setperra_Cdecl_Function_407B70 = symboldAddresses["loc_407B70"],
                Setperra_Cdecl_Function_IncreaseActorBattleTime = symboldAddresses["loc_442310"],
                    
                Setperra_Global_CurrentSceneType = symboldAddresses["loc_4AE978"],
                Setperra_Global_EnemiesCombatList = symboldAddresses["loc_4C9100"],
                Setperra_Global_ArrayOf3CombatEntries = symboldAddresses["loc_4BF040"],
                Setperra_Global_AlliesCombatList = symboldAddresses["loc_4C13F8"],
                Setperra_Global_Dword_0x4A2968 = symboldAddresses["loc_4A2968"],
                Setperra_Global_SelectedCharacterInBattle = symboldAddresses["loc_4A2678"],
                Setperra_Global_EnemiesCombatListCount =  symboldAddresses["loc_4AF8C4"],
                Setperra_Global_AlliesCombatListCount = symboldAddresses["loc_4AF8C8"],
                Setperra_Global_IsAtbDisabled = symboldAddresses["loc_4D0814"],
                Setperra_Global_SelectedPlayerActor = symboldAddresses["loc_4DE624"],
                Setperra_Global_BattlePartySize = symboldAddresses["loc_4AF8CC"],
                Setperra_Global_DesiredActorToSelect = symboldAddresses["loc_4A295C"],
            };
        }

        private Dictionary<String, UInt32> GetFunctionAddressesFromDwarfDebugSymbols()
        {
            UInt64 baseAddress = GetTextSectionImageBaseAddress();

            IReadOnlyList<PublicSymbol> dwarfSymbols = GetDwarfDebugSymbols();

            Dictionary<String, List<UInt32>> symbolNameToAddress = new(capacity: dwarfSymbols.Count);
            foreach (PublicSymbol symbol in dwarfSymbols)
            {
                if (!symbolNameToAddress.TryGetValue(symbol.Name, out var addresses))
                {
                    addresses = new List<UInt32>();
                    symbolNameToAddress.Add(symbol.Name, addresses);
                }

                UInt32 imageSymbolAddress = checked((UInt32)(symbol.Address + baseAddress));
                addresses.Add(imageSymbolAddress);
            }

            Dictionary<String, UInt32> uniqueSymbolAddresses = new();
            foreach (KeyValuePair<String,List<UInt32>> pair in symbolNameToAddress)
            {
                if (pair.Value.Count == 1)
                    uniqueSymbolAddresses.Add(pair.Key, pair.Value[0]);
            }

            return uniqueSymbolAddresses;
        }

        private UInt64 GetTextSectionImageBaseAddress()
        {
            PeFile peFile = new(GameDirectory.SRExecutablePath);
            ImageSectionHeader textSection = peFile.ImageSectionHeaders.Single(s => s.Name == ".text");
            return textSection.ImageBaseAddress;
        }

        private IReadOnlyList<PublicSymbol> GetDwarfDebugSymbols()
        {
            Type peImageType = typeof(PublicSymbol).Assembly.GetType("CsDebugScript.DwarfSymbolProvider.PeImage");
            Object dwarfImage = peImageType.GetConstructors().First().Invoke(new Object[] { GameDirectory.SRExecutablePath });
            return (IReadOnlyList<PublicSymbol>)peImageType.GetProperty("PublicSymbols").GetValue(dwarfImage);
        }
    }
}