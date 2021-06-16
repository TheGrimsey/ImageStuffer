using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

            // Process directories in parallel.
            Parallel.For(0, directories.Count, i => ProcessDirectory(directories[i]));
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

        void ProcessDirectory(DirectoryConfig config)
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(config.sourceDirectory);
            DirectoryInfo targetDirectory = new DirectoryInfo(config.targetDirectory);

            // Check directories.
            if (!sourceDirectory.Exists)
            {
                Console.WriteLine("Directory does not exist: " + config.sourceDirectory);
                return;
            }
            targetDirectory.Create();

            // Grab file infos..
            FileInfo[] files = sourceDirectory.GetFiles();

            Console.WriteLine("Processing directory: " + sourceDirectory.FullName + ", Found " + files.Length + " files.");

            JpegEncoder jpegEncoder = new JpegEncoder
            {
                Quality = config.quality
            };

            // Process files in parallel as well.
            Parallel.For(0, files.Length, i =>
            {
                string filePath = config.targetDirectory + Path.DirectorySeparatorChar + files[i].Name.Replace(files[i].Extension, ".jpg");
                if (Image.DetectFormat(files[i].FullName) != null && !File.Exists(filePath))
                {
                    using (Image image = Image.Load(files[i].FullName))
                    {
                        image.SaveAsJpeg(filePath, jpegEncoder);
                    }
                }
            });

            Console.WriteLine("Directory done: " + config.sourceDirectory);
        }
    }
}
