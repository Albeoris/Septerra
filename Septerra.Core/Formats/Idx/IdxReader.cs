using System;
using System.Collections.Generic;
using System.IO;

namespace Septerra.Core
{
    public sealed class IdxReader
    {
        private readonly MftContent _mftContent;

        public IdxReader(MftContent mftContent)
        {
            _mftContent = mftContent ?? throw new ArgumentNullException(nameof(mftContent));
        }

        public IdxContent ReadContent()
        {
            unsafe
            {
                Byte[] idxData = File.ReadAllBytes(_mftContent.IndexFilePath);
                if (idxData.Length % sizeof(IdxStruct) != 0)
                    throw new NotSupportedException($"The size of the file [{_mftContent.IndexFilePath}] is not a multiple of {sizeof(IdxStruct)}");

                Int32 entriesCount = idxData.Length / sizeof(IdxStruct);

                fixed (Byte* idxDataPtr = idxData)
                {
                    IdxStruct* idxPtr = (IdxStruct*)idxDataPtr;

                    Dictionary<DbPackage, IdxEntry[]> dic = ReadDictionary(entriesCount, idxPtr);
                    
                    return new IdxContent(_mftContent.Packages, dic);
                }
            }
        }

        private unsafe Dictionary<DbPackage, IdxEntry[]> ReadDictionary(Int32 entriesCount, IdxStruct* idxPtr)
        {
            // Count files in packages
            Dictionary<Int32, Int32> fileNumbers = new Dictionary<Int32, Int32>(_mftContent.Packages.Count);
            for (Int32 i = 0; i < entriesCount; i++)
            {
                Int32 packageIndex = idxPtr[i].Package;
                if (fileNumbers.TryGetValue(packageIndex, out Int32 count))
                    fileNumbers[packageIndex] = count + 1;
                else
                    fileNumbers[packageIndex] = 1;
            }

            // Prepare dictionary
            Dictionary<DbPackage, IdxEntry[]> dic = new Dictionary<DbPackage, IdxEntry[]>(fileNumbers.Count);
            foreach (var pair in fileNumbers)
            {
                Int32 packageIndex = pair.Key;
                Int32 fileCount = pair.Value;

                DbPackage package = _mftContent.Packages[packageIndex];
                IdxEntry[] idxEntries = new IdxEntry[fileCount];

                dic.Add(package, idxEntries);
            }

            // Fill dicrionary
            for (Int32 i = 0; i < entriesCount; i++)
            {
                IdxStruct idxEntry = idxPtr[i];
                DbPackage package = _mftContent.Packages[idxEntry.Package];
                IdxEntry[] idxEntries = dic[package];

                Int32 fileIndex = IndexInPackage(idxEntry.Package, idxEntries);
                idxEntries[fileIndex] = ToEntry(i, in idxEntry);
            }

            Int32 IndexInPackage(Int32 packageNumber, IdxEntry[] array)
            {
                return array.Length - fileNumbers[packageNumber]--;
            }

            return dic;
        }

        private static IdxEntry ToEntry(Int32 id, in IdxStruct idx)
        {
            if (idx.CompressedSize != idx.UncompressedSize)
            {
                Int32 compressionType = idx.Flags & 0xFF;
                if (compressionType != 1)
                    throw new NotSupportedException($"Not supported compression type: {compressionType}");
            }

            return new IdxEntry(id, idx.ResourceId, idx.Offset, idx.CompressedSize, idx.UncompressedSize, idx.ModifiedTime);
        }
    }
}