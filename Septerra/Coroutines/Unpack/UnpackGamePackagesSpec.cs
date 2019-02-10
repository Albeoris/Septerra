using System;

namespace Septerra
{
    public class UnpackGamePackagesSpec
    {
        public GameDirectoryDescriptor GameDirectory { get; set; }
        public String OutputDirectory { get; set; }
        public Boolean Convert { get; set; }
        public Boolean Rename { get; set; }
    }
}