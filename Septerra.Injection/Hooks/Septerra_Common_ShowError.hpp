#pragma once

// ReSharper disable CppCStyleCast

using namespace Septerra::Core;

namespace SepterraInjection
{
	typedef int (__cdecl* Septerra_Common_ShowError)(Char* errorMessage, Int32 errorCode);

	Septerra_Common_ShowError O_Septerra_Common_ShowError = (Septerra_Common_ShowError)0x00410960;

	int __cdecl Hook_Common_ShowError(Char* errorMessage, Int32 errorCode)
	{
		Int32 result;
		if (Hooks::Common::TryShowError(errorMessage, errorCode, result))
			return result;

		return O_Septerra_Common_ShowError(errorMessage, errorCode);
	}

	Septerra_Common_ShowError H_Septerra_Common_ShowError = Hook_Common_ShowError;
}
