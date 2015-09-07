using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet;

namespace PackageGenerator
{
    class Program
    {
        public static void Main(string[] args)
        {
            string contentPath;
            string destinationPath;
            string packageId;
            int numberOfPackages;
            int randomFileSize;

            if (args.Length == 5)
            {
                contentPath = args[0];
                destinationPath = args[1];
                packageId = args[2];
                numberOfPackages = Int32.Parse(args[3]);
                randomFileSize = Int32.Parse(args[4]);
            }
            else
            {
                Console.WriteLine("Package content path:");
                contentPath = Console.ReadLine();
                Console.WriteLine("Package destination path:");
                destinationPath = Console.ReadLine();
                Console.WriteLine("Package id:");
                packageId = Console.ReadLine();
                Console.WriteLine("Number of packages:");
                numberOfPackages = Int32.Parse(Console.ReadLine());
                Console.WriteLine("Random file size (kb):");
                randomFileSize = Int32.Parse(Console.ReadLine());
            }

            var includes = new List<string>
            {
                "**"
            };

            for (var i = 0; i < numberOfPackages; i++)
            {
                var version = new SemanticVersion(1, i, 0, 0);
                var metadata = new ManifestMetadata
                {
                    Id = packageId,
                    Version = version.ToString(),
                    Authors = "Testing",
                    Description = "A test package"
                };

                var randomFile = Path.Combine(contentPath, "random.txt");
                GenerateRandomFile(randomFile, randomFileSize);

                var package = new PackageBuilder();

                package.PopulateFiles(contentPath, includes.Select(include =>
                    new ManifestFile { Source = include })
                );
                package.Populate(metadata);

                var filename = metadata.Id + "." + metadata.Version + ".nupkg";
                var output = Path.Combine(destinationPath, filename);

                using (var outStream = File.Open(output, FileMode.Create))
                    package.Save(outStream);
            }
        }

        private static void GenerateRandomFile(string filename, int sizeInKb)
        {
            const int blockSize = 1024;
            byte[] data = new byte[blockSize];
            var rng = new Random();
            using (var stream = File.Open(filename, FileMode.Create))
            {
                for (var i = 0; i < sizeInKb; i++)
                {
                    rng.NextBytes(data);
                    stream.Write(data, 0, data.Length);
                }
            }
        }
    }
}
