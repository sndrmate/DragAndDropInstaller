/*
 * Copyright (c) 2023 smatthew
 * All rights reserved.
 */


using System.IO.Compression;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "GSX Pro Profile Installer v1.0 | by smatthew & FatGingerHead";
        var DefaultColor = Console.ForegroundColor;
        string extractPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+ "\\Virtuali\\GSX\\MSFS");
        if (args.Length > 0)
        {
            try
            {
                string ZipPath = args[0];
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"You dropped the file: {ZipPath}");
                Console.ForegroundColor = DefaultColor;

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
                            string finalExtractPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

                            if (finalExtractPath.StartsWith(extractPath, StringComparison.Ordinal))
                            {
                                Console.WriteLine(finalExtractPath);
                                entry.ExtractToFile(finalExtractPath);
                            }
                                
                        }
                    }
                }
                Console.WriteLine("\nPress any key to exit.");
            }
            catch (Exception e)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ForegroundColor = DefaultColor;
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
            }
            
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Please drag and drop a file onto the executable.");
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine("\nPress any key to exit.");
        }
        Console.ReadKey();
    }
}