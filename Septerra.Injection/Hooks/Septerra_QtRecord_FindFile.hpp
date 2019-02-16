#pragma once

// ReSharper disable CppCStyleCast

using namespace Septerra::Core;

namespace SepterraInjection
{
	typedef Char* (__cdecl* Septerra_QtRecord_FindFile)(UInt32 txRecordId, UInt32* fileOffset);

	Septerra_QtRecord_FindFile O_Septerra_QtRecord_FindFile = (Septerra_QtRecord_FindFile)0x00445B60;

	Char* __cdecl Hook_QtRecord_FindFile(UInt32 qtRecordId, UInt32* fileOffset)
	{
		Char* resultPath;
		UInt32 resultOffset;
		if (Hooks::QtRecord::Find(qtRecordId, resultPath, resultOffset))
		{
			*fileOffset = resultOffset;
			return resultPath;
		}

		return O_Septerra_QtRecord_FindFile(qtRecordId, fileOffset);
	}

	Septerra_QtRecord_FindFile H_Septerra_QtRecord_FindFile = Hook_QtRecord_FindFile;
}