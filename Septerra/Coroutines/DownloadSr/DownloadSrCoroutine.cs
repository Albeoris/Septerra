using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using Septerra.Core;

namespace Septerra
{
    public sealed class DownloadSrCoroutine : ICoroutine
    {
        private readonly DownloadSrSpec _spec;

        public DownloadSrCoroutine(DownloadSrSpec spec)
        {
            _spec = spec;
        }

        public void Execute()
        {
            Console.WriteLine("Downloading SR...");
            
            using (HttpClient client = new HttpClient())
            using (Stream stream = client.GetStreamAsync(_spec.SourceLink).Result)
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyToAsync(memoryStream).Wait();
                memoryStream.Seek(0, SeekOrigin.Begin);

                using (ZipArchive archive = new ZipArchive(memoryStream))
                    ExtractToDirectory(archive, _spec.OutputDirectory);
            }

            Console.WriteLine("SR has been downloaded!");
        }
        
        private static void ExtractToDirectory(ZipArchive source, string destinationDirectoryPath)
        {
            if (source == null)
                throw new ArgumentNullException(nameof (source));

            if (destinationDirectoryPath == null)
                throw new ArgumentNullException(nameof(destinationDirectoryPath));

            String path = Directory.CreateDirectory(destinationDirectoryPath).FullName;
            
            foreach (ZipArchiveEntry entry in source.Entries)
            {
                String fullPath = Path.GetFullPath(Path.Combine(path, entry.FullName));
                if (!fullPath.StartsWith(path, StringComparison.OrdinalIgnoreCase))
                    throw new IOException("IO_ExtractingResultsInOutside");
                
                if (Path.GetFileName(fullPath).Length == 0)
                {
                    if (entry.Length != 0L)
                        throw new IOException("IO_DirectoryNameWithData");
                    
                    Directory.CreateDirectory(fullPath);
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                    entry.ExtractToFile(fullPath, overwrite: true);
                }
            }
        }
    }
}