using System;
using System.Diagnostics;
using System.Reflection;

namespace Septerra
{
    internal sealed partial class MyProcess
    {
        private static class Private
        {
            public static readonly Type Type = typeof(Process);
            public static readonly Object s_CreateProcessLock = Type.GetField("s_CreateProcessLock", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);
            public static readonly MethodInfo s_BuildCommandLine = Type.GetMethod("BuildCommandLine", BindingFlags.Static | BindingFlags.NonPublic);
            public static readonly MethodInfo CreatePipe = Type.GetMethod("CreatePipe", BindingFlags.Instance | BindingFlags.NonPublic);
            public static readonly MethodInfo SetProcessHandle = Type.GetMethod("SetProcessHandle", BindingFlags.Instance | BindingFlags.NonPublic);
            public static readonly MethodInfo SetProcessId = Type.GetMethod("SetProcessId", BindingFlags.Instance | BindingFlags.NonPublic);
            public static readonly FieldInfo standardInput = Type.GetField("standardInput", BindingFlags.Instance | BindingFlags.NonPublic);
            public static readonly FieldInfo standardOutput = Type.GetField("standardOutput", BindingFlags.Instance | BindingFlags.NonPublic);
            public static readonly FieldInfo standardError = Type.GetField("standardError", BindingFlags.Instance | BindingFlags.NonPublic);
        }
    }
}