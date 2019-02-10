#pragma once

// ReSharper disable CppCStyleCast

using namespace Septerra::Core;

namespace SepterraInjection
{
	typedef Int32 (__cdecl* Septerra_TxRecord_ReleaseByPointer)(Char* cachedTextFile);

	Septerra_TxRecord_ReleaseByPointer O_Septerra_TxRecord_ReleaseByPointer = (Septerra_TxRecord_ReleaseByPointer)0x0040D350;

	Int32 __cdecl Hook_TxRecord_ReleaseByPointer(Char* cachedTextFile)
	{
		if (Hooks::TxRecord::TryReleaseByPointer(cachedTextFile))
			return 1;

		return O_Septerra_TxRecord_ReleaseByPointer(cachedTextFile);
	}

	Septerra_TxRecord_ReleaseByPointer H_Septerra_TxRecord_ReleaseByPointer = Hook_TxRecord_ReleaseByPointer;
}
