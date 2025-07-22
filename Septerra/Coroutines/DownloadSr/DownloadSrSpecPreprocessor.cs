using System;
using System.IO;
using System.Net.Http;

namespace Septerra;

public sealed class DownloadSrSpecPreprocessor : DownloadSrSpec
{
    public GameDirectoryDescriptor GameDirectory { get; set; }

    public void Preprocess()
    {
        ResolveSourceLink();
        ResolveOutputDirectory();
    }
    
    private void ResolveSourceLink()
    {
        if (SourceLink == null)
            SourceLink = "https://github.com/M-HT/SR/releases/download/septerra_v1.04.0.11/SepterraCore-Windows-x86-v1.04.0.11.zip";

        using (HttpClient client = new HttpClient())
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, SourceLink);
            HttpResponseMessage response = client.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
                throw new FileNotFoundException($"Download URL is invalid. Error: {response.StatusCode} Url: {SourceLink}.");
        }
    }

    private void ResolveOutputDirectory()
    {
        if (OutputDirectory == null)
        {
            if (GameDirectory == null)
                throw new FileNotFoundException($"Cannot resolve the output directory. GameDirectory is not set.");
            
            if (!GameDirectory.IsMftExists)
                throw new FileNotFoundException($"Cannot find a game archive file descriptor ({GameDirectory.MftPath}).");

            OutputDirectory = GameDirectory.DirectoryPath;
        }
    }

    private void CreateTargetDirectory()
    {
        if (String.IsNullOrEmpty(OutputDirectory))
            OutputDirectory = Path.Combine(GameDirectory.DirectoryPath, "Data");

        OutputDirectory = Path.GetFullPath(OutputDirectory);
        Directory.CreateDirectory(OutputDirectory);
    }
}