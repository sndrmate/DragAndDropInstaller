/*
 * Copyright (c) 2023 smatthew
 * All rights reserved.
 */
namespace DragAndDropInstaller;

public static class Program
{
    public static void Main(string[] args)
    {
        string version = "1.1-dev";
        Console.Title = $"Drag&Drop Installer v{version}";

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

            ArchiveExtractor extract = new();
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