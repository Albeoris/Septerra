#pragma once

// ReSharper disable CppCStyleCast

using namespace Septerra::Core;

namespace SepterraInjection
{
	typedef void* (__cdecl* Septerra_TxRecord_FindString)(int stringId, int *txFileContent);

	Septerra_TxRecord_FindString O_Septerra_TxRecord_FindString = (Septerra_TxRecord_FindString)0x00442F40;

	void* __cdecl Hook_TxRecord_FindString(int stringId, int *txFileContent)
	{
		int *currentIdPtr; // ecx
		int count; // eax
		
		if ( !txFileContent )
			return 0;
  
		currentIdPtr = txFileContent + 3;
		count = txFileContent[1];
  
		if ( !count )
			return 0;
  
		while ( 1 )
		{
			--count;
			if ( *currentIdPtr == stringId )
    			break;
			currentIdPtr += 3;
			if ( !count )
				return 0;
		}

		int result = (int)txFileContent + (int)currentIdPtr[1];
		return (void*)result;

		/*Char* result;
		if (Hooks::TxRecord::TryFindString(txRecordId, somePtr, result))
			return result;

		return O_Septerra_TxRecord_FindString(txRecordId, somePtr);*/
	}

	Septerra_TxRecord_FindString H_Septerra_TxRecord_FindString = Hook_TxRecord_FindString;
}