// dllmain.cpp : Определяет точку входа для приложения DLL.
#include "stdafx.h"

#include "libs/detours.h"
#include <sstream>
#include <iostream>

const auto OriginalFunctionAddress = 0x445C80;

int (__cdecl * Original_ReadDbRecord)(int dbRecordIndex, void *output, int outputSize);

int __cdecl Hooked_ReadDbRecord(int dbRecordIndex, void *output, int outputSize)
{
	std::cout << L"Hooked_ReadDbRecord: " << dbRecordIndex << ", " << outputSize;
	return Original_ReadDbRecord(dbRecordIndex, output, outputSize);
}

bool launchDebugger()
{
	// Get System directory, typically c:\windows\system32
	std::wstring systemDir(MAX_PATH + 1, '\0');
	UINT nChars = GetSystemDirectoryW(&systemDir[0], systemDir.length());
	if (nChars == 0) return false; // failed to get system directory
	systemDir.resize(nChars);

	// Get process ID and create the command line
	DWORD pid = GetCurrentProcessId();
	std::wostringstream s;
	s << systemDir << L"\\vsjitdebugger.exe -p " << pid;
	std::wstring cmdLine = s.str();

	// Start debugger process
	STARTUPINFOW si;
	ZeroMemory(&si, sizeof(si));
	si.cb = sizeof(si);

	PROCESS_INFORMATION pi;
	ZeroMemory(&pi, sizeof(pi));

	if (!CreateProcessW(NULL, &cmdLine[0], NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi)) return false;

	// Close debugger process handles to eliminate resource leak
	CloseHandle(pi.hThread);
	CloseHandle(pi.hProcess);

	// Wait for the debugger to attach
	while (!IsDebuggerPresent()) Sleep(100);

	// Stop execution so the debugger can take over
	DebugBreak();
	return true;
}

DWORD WINAPI HookGameFunctions(LPVOID lpParameter)
{
	Original_ReadDbRecord = (int(__cdecl*)(int, void*, int))DetourAttach((PVOID*)OriginalFunctionAddress, (PBYTE)Hooked_ReadDbRecord); //Magic
	return 0;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		launchDebugger(); // Uncomment to debug
		HookGameFunctions(NULL);
		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}
