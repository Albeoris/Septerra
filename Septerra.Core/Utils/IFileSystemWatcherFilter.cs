using System;

namespace Septerra.Core.Utils
{
    public interface IFileSystemWatcherFilter
    {
        Boolean CanProcessChanged(String filePath);
    }
}