/*
 * Copyright (c) 2023 smatthew
 * All rights reserved.
 */
using System.Globalization;

namespace DragAndDropInstaller;

public static class Program
{
    public static void Main(string[] args)
    {
        string version = "1.1-dev";
        Console.Title = $"Drag&Drop Installer v{version}";
        string destinationPath = Path.Combine(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)), "Virtuali", "GSX", "MSFS");

        if (args.Length == 0)
        {
            UserInterface.ArgsNull();
            UserInterface.KeyToExit();
            Console.ReadKey();
            return;
        }

        try
        {
            VersionChecker.CheckVersion(version);

            UserInterface.InitiateInstall(args[0]);

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
            UserInterface.KeyToExit();
            Console.ReadKey();
        }
    }
}