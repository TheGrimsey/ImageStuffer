using System;
using System.IO;
namespace ImageStuffer
{
    enum ImageType
    {
        Invalid,
        PNG,
        JPEG
    }

    struct DirectoryConfig
    {
        public readonly string sourceDirectory;
        public readonly string targetDirectory;
        public readonly int quality;

        public DirectoryConfig(string sourceDirectory, string targetDirectory, int quality)
        {
            this.sourceDirectory = sourceDirectory;
            this.targetDirectory = targetDirectory;
            this.quality = quality;
        }
    }
}
