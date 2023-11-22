/*
 * Copyright (c) 2023 smatthew
 * All rights reserved.
 */
using SharpCompress;
using SharpCompress.Archives;
class Program
{
    static void Main(string[] args)
    {
        Console.Title = "GSX Pro Profile Installer-dev v1.0";
        ConsoleColor DefaultColor = Console.ForegroundColor;
        string extractPath = Path.Combine(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)), "Virtuali", "GSX", "MSFS");

        
        if (args.Length == 0)
        // IDEA: We should make a search for .py and .ini files in the running directory.
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
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"You initiated the installation process from this archive:\n{zipPath}");
            Console.ForegroundColor = DefaultColor;
            List<string> overwrittenFiles = new();
            List<string> extractedFiles = new();

            using (var archive = ArchiveFactory.Open(zipPath))
            {
                /* Problem: Double foreach not so efficient. Maybe we should iterate through just once the archive and save the .py and .ini file names, then
                overwrite files with the same icao code and then just extract the saved files from the archive? It would need only 1 foreach and we could use the already existing list. */

                // Feature: If there is no .py or .ini files in the root directory of the archive, then check if there is a folder and search in that.

                foreach (IArchiveEntry entry in archive.Entries)
                {
                    string icao_code = entry.Key.Split('-')[0];
                    string[] matchingFiles = Directory.GetFiles(extractPath, $"*{icao_code}*");
                    overwrittenFiles.AddRange(matchingFiles);
                    matchingFiles.ForEach(filePath => File.Delete(filePath));            
                }
                foreach (IArchiveEntry entry in archive.Entries)
                {
                    if (entry.Key.EndsWith(".py", StringComparison.OrdinalIgnoreCase)
                        || entry.Key.EndsWith(".ini", StringComparison.OrdinalIgnoreCase))
                    {
                        string finalExtractPath = Path.GetFullPath(Path.Combine(extractPath, entry.Key));
                        extractedFiles.Add(finalExtractPath);
                        using (Stream stream = entry.OpenEntryStream())
                        {
                            using (FileStream writer = File.OpenWrite(finalExtractPath))
                            {
                                stream.CopyTo(writer);
                            }

                            }
                    }
                    else if (entry.IsDirectory)
                    {
                        string searchPath = Path.GetFullPath(Path.Combine(zipPath, entry.Key));

                        }
                    }
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
