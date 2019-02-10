using System;
using System.IO;
using Microsoft.Win32;

namespace Septerra
{
    public static class GameDirectoryProvider
    {
        public static GameDirectoryDescriptor GetDefault()
        {
            if (FindDefault(out var dir))
                return dir;

            throw new DirectoryNotFoundException("Cannot find the game folder.");
        }

        public static Boolean FindDefault(out GameDirectoryDescriptor descriptor)
        {
            return ByCurrentFolder(out descriptor) ||
                   ByRegistryKey(out descriptor);
        }

        public static Boolean ByCurrentFolder(out GameDirectoryDescriptor descriptor)
        {
            GameDirectoryDescriptor result = new GameDirectoryDescriptor(Environment.CurrentDirectory);
            if (result.IsExecutableExists || result.IsMftExists)
            {
                descriptor = result;
                return true;
            }

            descriptor = null;
            return false;
        }

        public static Boolean ByRegistryKey(out GameDirectoryDescriptor descriptor)
        {
            const String RegistryPath = @"SOFTWARE\Wow6432Node\Valkyrie Studios\Septerra Core";

            using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,RegistryView.Registry32))
            using (RegistryKey registryKey = localMachine.OpenSubKey(RegistryPath))
            {
                if (registryKey == null)
                {
                    descriptor = null;
                    return false;
                }

                foreach (String name in new[] {"executable", "installpath", "SourcePath"})
                {
                    if (registryKey.GetValue(name) is String gameDirectory)
                    {
                        GameDirectoryDescriptor result = new GameDirectoryDescriptor(gameDirectory);
                        if (result.IsExecutableExists || result.IsMftExists)
                        {
                            descriptor = result;
                            return true;
                        }
                    }
                }
                
                descriptor = null;
                return false;
            }
        }
    }
}