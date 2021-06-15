using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System;
using System.Collections.Generic;
using System.IO;

namespace ImageStuffer
{
    class Program
    {
        static readonly string configPath = "Directories.json";

        static void Main(string[] args)
        {
            new Program();
        }

        List<DirectoryConfig> directories;

        Program()
        {
            // Load directories.
            LoadDirectoryJson();

            // Process directories.
            foreach(DirectoryConfig config in directories)
            {
                ProcessDirectory(config);
            }
        }

        void LoadDirectoryJson()
        {
            string directoriesString = "";

            if (!File.Exists(configPath))
            {
                DirectoryConfig directoryConfig = new DirectoryConfig(Directory.GetCurrentDirectory(), Directory.GetCurrentDirectory(), 90);

                using (StreamWriter streamWriter = new StreamWriter(configPath))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(new List<DirectoryConfig> { directoryConfig }, Formatting.Indented));
                    streamWriter.Close();
                }
            }

            using (StreamReader streamReader = new StreamReader(configPath))
            {
                directoriesString = streamReader.ReadToEnd();
                streamReader.Close();
            }

            directories = JsonConvert.DeserializeObject<List<DirectoryConfig>>(directoriesString);
        }

        void ProcessDirectory(in DirectoryConfig config)
        {
            if(!new DirectoryInfo(config.sourceDirectory).Exists)
            {
                Console.WriteLine("Directory does not exist: " + config.sourceDirectory);
                return;
            }

            new DirectoryInfo(config.targetDirectory).Create();

            Console.WriteLine("Processing directory: " + config.sourceDirectory);

            // Load & Save images.
            FileInfo[] files = new DirectoryInfo(config.sourceDirectory).GetFiles();

            JpegEncoder jpegEncoder = new JpegEncoder
            {
                Quality = config.quality
            };

            foreach (FileInfo fileInfo in files)
            {
                if (Image.DetectFormat(fileInfo.FullName) == null)
                    continue;

                using (Image image = Image.Load(fileInfo.FullName))
                {
                    image.SaveAsJpeg(config.targetDirectory + Path.DirectorySeparatorChar + fileInfo.Name.Replace(fileInfo.Extension, ".jpg"), jpegEncoder);
                }
            }
        }
    }
}
