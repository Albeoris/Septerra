#pragma once

// ReSharper disable CppCStyleCast

using namespace System;
using namespace Septerra::Core;

namespace SepterraInjection
{
	typedef LRESULT(__stdcall* Septerra_HandleWindowsMessage)(HWND hWnd, int Msg, int wParam, unsigned int lParam);

	Septerra_HandleWindowsMessage O_Septerra_HandleWindowsMessage = (Septerra_HandleWindowsMessage)0x00411730;

	LRESULT __stdcall Hook_HandleWindowsMessage(HWND hWnd, int Msg, int wParam, unsigned int lParam)
	{
		Int32 result;
		if (Hooks::Main::HandleWindowsMessage(IntPtr(hWnd), static_cast<Septerra::WindowsMessage>(Msg), wParam, lParam, result))
			return (LRESULT)result;

		return O_Septerra_HandleWindowsMessage(hWnd, Msg, wParam, lParam);
	}

	Septerra_HandleWindowsMessage H_Septerra_HandleWindowsMessage = Hook_HandleWindowsMessage;
}
