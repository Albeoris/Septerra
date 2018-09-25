using System;

namespace Septera
{
    class Program
    {
        static void Main(string[] rawArgs)
        {
            try
            {
                ProgramArguments arguments = new ProgramArguments(rawArgs);
                String gameDirectory = arguments.GetGameDirectoryPath();
                String outputDirectory = arguments.GetOutputDirectoryPath();

                MftReader mft = MftReader.Create(gameDirectory);
                MftContent mftContent = mft.ReadContent();

                IdxReader idx = new IdxReader(mftContent);
                IdxContent idxContent = idx.ReadContent();

                using (DbExtractor extractor = new DbExtractor(mftContent.Version, outputDirectory))
                {
                    foreach (var group in idxContent)
                    {
                        DbPackage package = group.Key;
                        foreach (IdxEntry entry in group)
                            extractor.Enqueue(package, entry);
                    }
                }

                Console.WriteLine("Extracted!");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("----------------------------");
                Console.Error.WriteLine("Failed to extract game data.");
                Console.Error.WriteLine("----------------------------");
                Console.Error.WriteLine(ex);
                Console.Error.WriteLine("----------------------------");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}