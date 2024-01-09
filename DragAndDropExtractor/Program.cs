/*
 * Copyright (c) 2023 smatthew
 * All rights reserved.
 */
namespace DragAndDropInstaller;

public static class Program
{
    public static void Main(string[] args)
    {
        string version = "2.0-dev";
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
            VersionChecker.CheckVersionAsync(version);
            ArchiveExtractor EX = new();
            EX.StartArchiveProcessing(args);
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