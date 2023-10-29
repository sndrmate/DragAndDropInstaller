// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "DragAndDropExtractor V1.0 Beta | by smatthew";
        var DefaultColor = Console.ForegroundColor;
        string extractPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+ "\\Virtuali\\MSFS\\GSX");
        if (args.Length > 0)
        {
            try
            {
                string ZipPath = args[0];
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"You dropped the file: {ZipPath}");

                if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                    extractPath += Path.DirectorySeparatorChar;

                using (ZipArchive archive = ZipFile.OpenRead(ZipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string icao_code = entry.FullName.Split('-')[0];
                        string[] matchingFiles = Directory.GetFiles(extractPath, $"*{icao_code}*");

                        if (matchingFiles.Any())
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\nFiles overwritten:");
                            Console.ForegroundColor = DefaultColor;
                            foreach (string file in matchingFiles)
                            {
                                Console.WriteLine(file);
                                File.Delete(file);
                            }
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nExtracted files:");
                    Console.ForegroundColor = DefaultColor;
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(".py", StringComparison.OrdinalIgnoreCase) || entry.FullName.EndsWith(".ini", StringComparison.OrdinalIgnoreCase))
                        {   
                            // Gets the full path to ensure that relative segments are removed.
                            string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

                            // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                            // are case-insensitive.
                            if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                            {
                                Console.WriteLine(destinationPath);
                                entry.ExtractToFile(destinationPath);
                            }
                                
                        }
                    }
                }
                Console.WriteLine("\nPress any key to exit.");
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
            }
            
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Please drag and drop a file onto the DragAndDropExtractor.exe.");
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine("\nPress any key to exit.");
        }
        Console.ReadKey();
    }
}