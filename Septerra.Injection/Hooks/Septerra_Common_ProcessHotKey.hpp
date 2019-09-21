#pragma once

// ReSharper disable CppCStyleCast

using namespace Septerra::Core;

namespace SepterraInjection
{
	typedef int(__cdecl* Septerra_Common_ProcessHotKey)(unsigned int hotKey);

	Septerra_Common_ProcessHotKey O_Septerra_Common_ProcessHotKey = (Septerra_Common_ProcessHotKey)0x40A8D0;

	int __cdecl Hook_ProcessHotKey(UInt32 hotKey)
	{
		if (Hooks::Common::TryProcessHotKey(hotKey))
			return 1;

		return O_Septerra_Common_ProcessHotKey(hotKey);
	}

	Septerra_Common_ProcessHotKey H_Septerra_Common_ProcessHotKey = Hook_ProcessHotKey;
}
