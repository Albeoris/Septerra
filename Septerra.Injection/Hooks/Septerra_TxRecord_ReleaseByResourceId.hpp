#pragma once

// ReSharper disable CppCStyleCast

using namespace Septerra::Core;

namespace SepterraInjection
{
	typedef Int32 (__cdecl* Septerra_TxRecord_ReleaseByResourceId)(UInt32 txRecordId);

	Septerra_TxRecord_ReleaseByResourceId O_Septerra_TxRecord_ReleaseByResourceId = (Septerra_TxRecord_ReleaseByResourceId)0x0040E370;

	Int32 __cdecl Hook_TxRecord_ReleaseByResourceId(UInt32 txRecordId)
	{
		if (Hooks::TxRecord::TryReleaseByResourceId(txRecordId))
			return 1;

		return O_Septerra_TxRecord_ReleaseByResourceId(txRecordId);
	}

	Septerra_TxRecord_ReleaseByResourceId H_Septerra_TxRecord_ReleaseByResourceId = Hook_TxRecord_ReleaseByResourceId;
}
