using System;

namespace Septerra
{
    public class RunGameSpec
    {
        public GameDirectoryDescriptor GameDirectory { get; set; }
        public GameInjectionDescriptor GameInjection { get; set; }
        public Boolean SkipSRDownload { get; set; }
        public String GameArguments { get; set; }
    }
}