using System;
using System.IO;
using Ionic.Zip;

namespace DotNetZipConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2) Usage();
            if (!System.IO.Directory.Exists(args[1]))
            {
                Console.WriteLine("The directory does not exist!\n");
                Usage();
            }
            if (File.Exists(args[0]))
            {
                Console.WriteLine("That zipfile already exists!\n");
                Usage();
            }

            string ZipFileToCreate = args[0];
            string DirectoryToZip = args[1];
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.AddDirectory(DirectoryToZip);
                    zip.Save(ZipFileToCreate);
                }
            }
            catch (Exception ex1)
            {
                Console.Error.WriteLine("exception: " + ex1);
            }

        }

        private static void Usage()
        {
            Console.WriteLine("usage:\n  DotNetZipConsole <ZipFileToCreate> <directoryToZip>");
            Environment.Exit(1);
        }
    }
}
