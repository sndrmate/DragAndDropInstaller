/*
 * Copyright (c) 2023 smatthew
 * All rights reserved.
 */
using SharpCompress;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace DragAndDropExtractor;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "GSX Pro Profile Installer-dev v1.0";
        ConsoleColor DefaultColor = Console.ForegroundColor;
        string destinationPath = Path.Combine(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)), "Virtuali", "GSX", "MSFS");


        if (args.Length == 0)
        // IDEA: We should make a search for .py and .ini files in the running directory (or copy the archive to the terminal?).
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: To install a GSX Pro Profile, please drag and drop the archive onto the executable.");
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
            return;
        }

        try
        {
            string archivePath = args[0];
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"You initiated the installation process from this archive:");
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine(archivePath);
            List<IArchiveEntry> selectedArchiveFiles = new List<IArchiveEntry>();
            List<string> deletedFiles = new();
            List<string> installedFiles = new();

            ReadArchive(archivePath);

            void ReadArchive(string archivePath)
            {
                using (IArchive archive = ArchiveFactory.Open(archivePath))
                {
                    /* Problem: Double foreach not so efficient. Maybe we should iterate through just once the archive and save the .py and .ini file names, then
                    overwrite files with the same icao code and then just extract the saved files from the archive? It would need only 1 foreach and we could use the already existing list. */
                    Console.ForegroundColor = ConsoleColor.Yellow; //remove this line before merge to master
                    foreach (IArchiveEntry entry in archive.Entries)
                    {
                        if (entry.Key.EndsWith(".py", StringComparison.OrdinalIgnoreCase)
                            || entry.Key.EndsWith(".ini", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"DEBUG: FILE FOUND AND PROCESSING BEGINS: {entry.Key}"); //remove this line before merge to master
                            selectedArchiveFiles.Add(entry);
                            string filename = entry.Key;
                            if (entry.Key.Contains("/")) { filename = entry.Key.Split('/').Last(); }
                            string icao_code = filename.Split('-')[0];
                            string[] matchingFiles = Directory.GetFiles(destinationPath, $"*{icao_code}*");
                            deletedFiles.AddRange(matchingFiles);
                            matchingFiles.ForEach(filePath => File.Delete(filePath));
                        }
                    }
                    if (!(selectedArchiveFiles?.Any() ?? false))
                    {
                        throw new Exception("ERROR: The archive is empty or no relevant files have been found.\n");
                    }
                    foreach (IArchiveEntry entry in selectedArchiveFiles)
                    {
                        string filename = entry.Key;
                        if (entry.Key.Contains("/")) { filename = entry.Key.Split('/').Last(); }
                        string fullDestinationPath = Path.GetFullPath(Path.Combine(destinationPath, filename));
                        installedFiles.Add(fullDestinationPath);
                        using (Stream stream = entry.OpenEntryStream())
                        using (FileStream writer = File.OpenWrite(fullDestinationPath))
                        {
                            stream.CopyTo(writer);
                        }
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nInstalled files:\n");
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine(string.Join('\n', installedFiles));
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nOverwritten files:\n");
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine(string.Join('\n', deletedFiles));
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