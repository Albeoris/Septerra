#pragma once

// ReSharper disable CppCStyleCast

using namespace Septerra::Core;

namespace SepterraInjection
{
	typedef void (__cdecl* Septerra_DbRecord_Close)(Int32 dbRecordIndex);

	Septerra_DbRecord_Close O_Septerra_DbRecord_Close = (Septerra_DbRecord_Close)0x00445EC0;

	void __cdecl Hook_DbRecord_Close(Int32 dbRecordIndex)
	{
		if (Hooks::DbRecord::TryClose(dbRecordIndex))
			return;

		return O_Septerra_DbRecord_Close(dbRecordIndex);
	}

	Septerra_DbRecord_Close H_Septerra_DbRecord_Close = Hook_DbRecord_Close;
}
