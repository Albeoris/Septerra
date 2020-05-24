using System;
using Septerra.Core;

namespace Septerra
{
    public sealed class UnpackGamePackagesCoroutine : ICoroutine
    {
        private readonly UnpackGamePackagesSpec _spec;

        public UnpackGamePackagesCoroutine(UnpackGamePackagesSpec spec)
        {
            _spec = spec;
        }

        public void Execute()
        {
            MftReader mft = MftReader.Create(_spec.GameDirectory.DirectoryPath);
            MftContent mftContent = mft.ReadContent();

            IdxReader idx = new IdxReader(mftContent);
            IdxContent idxContent = idx.ReadContent();

            TXEncoding.TryReadFromExecutable(_spec.GameDirectory.ExecutablePath);

            using (DbExtractor extractor = new DbExtractor(mftContent.Version, _spec.OutputDirectory))
            {
                extractor.Rename = _spec.Rename;
                extractor.Convert = _spec.Convert;

                foreach (var group in idxContent)
                {
                    DbPackage package = group.Key;
                    foreach (IdxEntry entry in group)
                        extractor.Enqueue(package, entry);
                }
            }

            Console.WriteLine("Extracted!");
        }
    }
}