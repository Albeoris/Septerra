#pragma once

// ReSharper disable CppCStyleCast

using namespace Septerra::Core;

namespace SepterraInjection
{
	typedef int (__cdecl* Septerra_DbRecord_Open)(UInt32 dbRecordId);

	Septerra_DbRecord_Open O_Septerra_DbRecord_Open = (Septerra_DbRecord_Open)0x00445900;

	int __cdecl Hook_OpenDbRecord(UInt32 dbRecordId)
	{
		Int32 recordIndex;
		if (Hooks::DbRecord::TryOpen(dbRecordId, recordIndex))
			return recordIndex;

		return O_Septerra_DbRecord_Open(dbRecordId);
	}

	Septerra_DbRecord_Open H_Septerra_DbRecord_Open = Hook_OpenDbRecord;
}
