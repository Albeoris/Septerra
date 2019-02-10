#pragma once

// ReSharper disable CppCStyleCast

using namespace Septerra::Core;

namespace SepterraInjection
{
	typedef Char* (__cdecl* Septerra_TxRecord_Acquire)(UInt32 txRecordId, Int32 somePtr);

	Septerra_TxRecord_Acquire O_Septerra_TxRecord_Acquire = (Septerra_TxRecord_Acquire)0x0040D1D0;

	Char* __cdecl Hook_TxRecord_Acquire(UInt32 txRecordId, Int32 somePtr)
	{
		Char* result;
		if (Hooks::TxRecord::TryAcquire(txRecordId, somePtr, result))
			return result;

		return O_Septerra_TxRecord_Acquire(txRecordId, somePtr);
	}

	Septerra_TxRecord_Acquire H_Septerra_TxRecord_Acquire = Hook_TxRecord_Acquire;
}