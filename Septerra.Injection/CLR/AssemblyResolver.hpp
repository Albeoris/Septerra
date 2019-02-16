#pragma once

#include <windows.h>
#include <stdio.h>

#pragma unmanaged

LPTOP_LEVEL_EXCEPTION_FILTER oldFilter_ = NULL;

LONG WINAPI MyExceptionFilter(__in struct _EXCEPTION_POINTERS *ExceptionInfo)
{
	if (oldFilter_ == NULL)
		return EXCEPTION_CONTINUE_SEARCH;
	return oldFilter_(ExceptionInfo);
}

#pragma managed

namespace SepterraInjection {
	public ref class SEHInstall
	{
	public:
		static void InstallFilter()
		{
			oldFilter_ = SetUnhandledExceptionFilter(MyExceptionFilter);
		}
	};
}

using namespace System;
using namespace System::IO;
using namespace System::Reflection;
using namespace Septerra::Core;

namespace SepterraInjection {

	public ref class AssemblyResolver
	{
		public:

		static bool s_initialized = false;

		static void Ensure()
		{
			// System::Diagnostics::Debugger::Launch();
			if (s_initialized)
				return;

			s_initialized = true;

			AppDomain^ currentDomain = AppDomain::CurrentDomain;
			currentDomain->AssemblyResolve += gcnew ResolveEventHandler( AssemblyResolver::MyResolveEventHandler );
			
			currentDomain->UnhandledException += gcnew System::UnhandledExceptionEventHandler( AssemblyResolver::OnUnhandledException );

			SEHInstall::InstallFilter();

		}
		
		static Assembly^ MyResolveEventHandler( Object^ sender, ResolveEventArgs^ args )
		{
			//System::Diagnostics::Debugger::Launch();

			String^ outputDir = BUILD_OUTPUT_DIR;
			outputDir = outputDir->Replace("C=", "C#"); // Idk how to correct this path :(
		    String^ pathToManagedAssembly = Path::Combine(outputDir, GetAssemblyName(args) + ".dll");
		    Assembly^ newAssembly = Assembly::LoadFile(pathToManagedAssembly);
		    return newAssembly;
		}

		static String^ GetAssemblyName (ResolveEventArgs^ args)
		{
			String^ name;
			if (args->Name->IndexOf (",") > -1)
				name = args->Name->Substring (0, args->Name->IndexOf (","));
			else
				name = args->Name;
			return name;
		}

		static void OnUnhandledException(System::Object ^sender, System::UnhandledExceptionEventArgs ^e)
		{
			System::Diagnostics::Debugger::Launch();
			Exception^ ex = (Exception^)(e->ExceptionObject);
			String^ errorMessage = "Unhandled error.";
			Log::Error(ex, errorMessage);
			throw;
		}

	};
}