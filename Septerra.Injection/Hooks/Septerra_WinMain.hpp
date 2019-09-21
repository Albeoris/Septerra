#pragma once

// ReSharper disable CppCStyleCast

using namespace System;
using namespace Septerra::Core;

namespace SepterraInjection
{
	typedef int(__stdcall* Septerra_WinMain)(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nShowCmd);

	Septerra_WinMain O_Septerra_WinMain = (Septerra_WinMain)0x00412B80;

	int __stdcall Hook_WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nShowCmd)
	{
		Int32 result;
		if (Hooks::Main::WinMain(IntPtr(hInstance), IntPtr(hPrevInstance), (signed char*)lpCmdLine, nShowCmd, result))
			return result;

		return O_Septerra_WinMain(hInstance, hPrevInstance, lpCmdLine, nShowCmd);
	}

	Septerra_WinMain H_Septerra_WinMain = Hook_WinMain;
}
