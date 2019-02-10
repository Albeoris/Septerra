#pragma once

// ReSharper disable CppCStyleCast

using namespace Septerra::Core;

namespace SepterraInjection
{
	typedef int (__cdecl* Septerra_DbRecord_Read)(Int32 dbRecordIndex, Byte* output, Int32 outputSize);

	Septerra_DbRecord_Read O_Septerra_DbRecord_Read = (Septerra_DbRecord_Read)0x00445C80;

	int __cdecl Hook_ReadDbRecord(Int32 dbRecordIndex, Byte* output, Int32 outputSize)
	{
		Int32 readedSize;
		if (Hooks::DbRecord::TryRead(dbRecordIndex, output, outputSize, readedSize))
			return readedSize;

		return O_Septerra_DbRecord_Read(dbRecordIndex, output, outputSize);
	}

	Septerra_DbRecord_Read H_Septerra_DbRecord_Read = Hook_ReadDbRecord;
}
