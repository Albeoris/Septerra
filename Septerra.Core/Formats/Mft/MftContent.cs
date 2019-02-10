using System;
using System.Collections.Generic;

namespace Septerra.Core
{
    public sealed class MftContent
    {
        public TerrabuilderVersion Version { get; }
        public String IndexFilePath { get; }
        public IReadOnlyList<DbPackage> Packages { get; }

        public MftContent(TerrabuilderVersion version, String indexFilePath, IReadOnlyList<DbPackage> dbPackage)
        {
            Version = version ?? throw new NullReferenceException(nameof(version));
            IndexFilePath = indexFilePath ?? throw new NullReferenceException(nameof(indexFilePath));
            Packages = dbPackage ?? throw new NullReferenceException(nameof(dbPackage));
        }
    }
}