/*
 * Copyright (c) 2023 smatthew
 * All rights reserved.
 */
namespace DragAndDropInstaller;

public static class Program
{
    public static void Main(string[] args)
    {
        UserInterface UI = new();
        Console.Title = "Drag&Drop Installer v1.1";
        string destinationPath = Path.Combine(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)), "Virtuali", "GSX", "MSFS");

        if (args.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: To install a GSX Pro Profile, please drag and drop the archive onto the executable.");
            UI.KeyToExit();
            Console.ReadKey();
            return;
        }

        try
        {
            UI.InitiateInstall(args[0]);

            ArchiveExtractor extract = new(destinationPath);
            extract.ExtractFiles(args[0]);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.ToString());
        }
        finally
        {
            UI.KeyToExit();
            Console.ReadKey();
        }
    }
}