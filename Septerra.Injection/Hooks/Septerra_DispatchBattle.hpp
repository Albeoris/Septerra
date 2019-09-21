#pragma once

// ReSharper disable CppCStyleCast

using namespace System;
using namespace Septerra::Core;

namespace SepterraInjection
{
	typedef void(_cdecl* Septerra_DispatchBattle)();

	Septerra_DispatchBattle O_Septerra_DispatchBattle = (Septerra_DispatchBattle)0x00430500;

	void _cdecl Hook_DispatchBattle()
	{
		if (Hooks::Main::DispatchBattle())
			return;

		return O_Septerra_DispatchBattle();
	}

	Septerra_DispatchBattle H_Septerra_DispatchBattle = Hook_DispatchBattle;
}