#pragma once

// ReSharper disable CppCStyleCast

using namespace Septerra::Core;

namespace SepterraInjection
{
	typedef int (__cdecl* Septerra_DbRecord_GetDecompressedSize)(Int32 dbRecordIndex);

	Septerra_DbRecord_GetDecompressedSize O_Septerra_DbRecord_GetDecompressedSize = (Septerra_DbRecord_GetDecompressedSize)0x00445F30;

	int __cdecl Hook_GetDecompressedSizeDbRecord(Int32 dbRecordIndex)
	{
		Int32 recordIndex;
		if (Hooks::DbRecord::TryGetDecompressedSize(dbRecordIndex, recordIndex))
			return recordIndex;

		return O_Septerra_DbRecord_GetDecompressedSize(dbRecordIndex);
	}

	Septerra_DbRecord_GetDecompressedSize H_Septerra_DbRecord_GetDecompressedSize = Hook_GetDecompressedSizeDbRecord;
}
