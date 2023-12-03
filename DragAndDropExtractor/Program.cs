/*
 * Copyright (c) 2023 smatthew
 * All rights reserved.
 */
using SharpCompress;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace DragAndDropInstaller;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "Drag&Drop Installer-dev v1.0";
        ConsoleColor DefaultColor = Console.ForegroundColor;
        string destinationPath = Path.Combine(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)), "Virtuali", "GSX", "MSFS");

        if (args.Length == 0)
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
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("You initiated the installation process from this archive:");
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine(args[0]);
            Console.WriteLine();

            ArchiveExtractor extract = new(destinationPath);
            extract.ExtractFiles(args[0]);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nInstalled files:\n");
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine(extract.DisplayInstalledFiles());
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nOverwritten files:\n");
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine(extract.DisplayRemovedFiles()); 
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