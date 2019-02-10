#include "../libs/detours.h"
#include "../CLR/AssemblyResolver.hpp"
#include "Septerra_Common_ShowError.hpp"
#include "Septerra_DbRecord_Close.hpp"
#include "Septerra_DbRecord_GetDecompressedSize.hpp"
#include "Septerra_DbRecord_Open.hpp"
#include "Septerra_DbRecord_Read.hpp"
#include "Septerra_DbRecord_Seek.hpp"
#include "Septerra_TxRecord_Acquire.hpp"
#include "Septerra_TxRecord_Find.hpp"
#include "Septerra_TxRecord_ReleaseByPointer.hpp"
#include "Septerra_TxRecord_ReleaseByResourceId.hpp"
#include "Septerra_TxRecord_FindString.hpp"

#pragma comment(lib, "detours.lib")
#pragma unmanaged

#define Hook(delegaeName) if (DetourAttach((PVOID*)(&O_##delegaeName), H_##delegaeName)) return FALSE

namespace SepterraInjection
{
	BOOL DllProcessAttach()
	{
		if (DetourTransactionBegin()) return FALSE;
		if (DetourUpdateThread(GetCurrentThread())) return FALSE;

		Hook(Septerra_Common_ShowError);
		Hook(Septerra_DbRecord_Close);
		Hook(Septerra_DbRecord_GetDecompressedSize);
		Hook(Septerra_DbRecord_Open);
		Hook(Septerra_DbRecord_Read);
		Hook(Septerra_DbRecord_Seek);
		Hook(Septerra_TxRecord_Acquire);
		Hook(Septerra_TxRecord_Find);
		Hook(Septerra_TxRecord_ReleaseByPointer);
		Hook(Septerra_TxRecord_ReleaseByResourceId);

		/*Hook(Septerra_TxRecord_FindString);*/

		if (DetourTransactionCommit()) return FALSE;

		return TRUE;
	}

	BOOL DllThreadAttach()
	{
		return TRUE;
	}

	#pragma managed
	BOOL DllThreadDetach()
	{
		// Invoke after CLR initialization
		SepterraInjection::AssemblyResolver::Ensure();
		return TRUE;
	}
	#pragma unmanaged

	BOOL DllProcessDetach()
	{
		return TRUE;
	}

	// Export DLL main. CLR will invoke it.
	// Don't invoke any managed methods before thread detach.

	extern "C" __declspec(dllexport)
	BOOL WINAPI DllMain(HMODULE dllInstance, DWORD callReason, LPVOID reserved)
	{
		//LaunchDebugger(); // Uncomment to debug

		switch (callReason)
		{
		case DLL_PROCESS_ATTACH: return DllProcessAttach();
		case DLL_THREAD_ATTACH: return DllThreadAttach();
		case DLL_THREAD_DETACH: return DllThreadDetach();
		case DLL_PROCESS_DETACH: return DllProcessDetach();
		default: return TRUE;
		}
	}
}
#pragma managed