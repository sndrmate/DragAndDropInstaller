/*
 * Copyright (c) 2023 smatthew
 * All rights reserved.
 */

using SharpCompress;
using System.IO.Compression;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "GSX Pro Profile Installer-dev v1.0 | by smatthew & FatGingerHead";
        var DefaultColor = Console.ForegroundColor;
        string extractPath = Path.Combine(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)), "Virtuali", "GSX", "MSFS");
        if (args.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Please drag and drop a file onto the executable.");
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
            return;
        }

        try
        {
            string zipPath = args[0];
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"You dropped the file: {zipPath}");
            Console.ForegroundColor = DefaultColor;
            List<string> overwrittenFiles = new();
            List<string> extractedFiles = new();

            using ZipArchive archive = ZipFile.OpenRead(zipPath);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string icao_code = entry.FullName.Split('-')[0];
                string[] matchingFiles = Directory.GetFiles(extractPath, $"*{icao_code}*");
                overwrittenFiles.AddRange(matchingFiles);
                matchingFiles.ForEach(filePath => File.Delete(filePath));

                if (entry.FullName.EndsWith(".py", StringComparison.OrdinalIgnoreCase)
                    || entry.FullName.EndsWith(".ini", StringComparison.OrdinalIgnoreCase))
                {
                    string finalExtractPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));
                    extractedFiles.Add(finalExtractPath);
                    entry.ExtractToFile(finalExtractPath);
                }

            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nFiles overwritten:\n" + string.Join('\n', overwrittenFiles));
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nExtracted files:\n" + string.Join('\n', extractedFiles));
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine();
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.ToString());
        }
        finally
        {
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
        }
    }
}
