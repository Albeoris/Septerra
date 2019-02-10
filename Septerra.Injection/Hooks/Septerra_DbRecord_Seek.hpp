#pragma once

// ReSharper disable CppCStyleCast

using namespace Septerra::Core;

namespace SepterraInjection
{
	typedef void (__cdecl* Septerra_DbRecord_Seek)(Int32 dbRecordIndex, Int32 offset, Int32 seekType);

	Septerra_DbRecord_Seek O_Septerra_DbRecord_Seek = (Septerra_DbRecord_Seek)0x00445BE0;

	void __cdecl Hook_SeekDbRecord(Int32 dbRecordIndex, Int32 offset, Int32 seekType)
	{
		if (Hooks::DbRecord::TrySeek(dbRecordIndex, offset, seekType))
			return;

		return O_Septerra_DbRecord_Seek(dbRecordIndex, offset, seekType);
	}

	Septerra_DbRecord_Seek H_Septerra_DbRecord_Seek = Hook_SeekDbRecord;
}
