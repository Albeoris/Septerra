#pragma once

// ReSharper disable CppCStyleCast

using namespace Septerra::Core;

namespace SepterraInjection
{
	typedef Char* (__cdecl* Septerra_TxRecord_Find)(UInt32 txRecordId);

	Septerra_TxRecord_Find O_Septerra_TxRecord_Find = (Septerra_TxRecord_Find)0x0040D3F0;

	Char* __cdecl Hook_TxRecord_Find(UInt32 txRecordId)
	{
		Char* result;
		if (Hooks::TxRecord::TryFind(txRecordId, result))
			return result;

		return O_Septerra_TxRecord_Find(txRecordId);
	}

	Septerra_TxRecord_Find H_Septerra_TxRecord_Find = Hook_TxRecord_Find;
}
