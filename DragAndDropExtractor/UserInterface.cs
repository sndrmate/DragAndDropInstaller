using System;
using System.Diagnostics;
using System.Runtime.Intrinsics;
using SharpCompress.Archives;
using SharpCompress.Common;
using Spectre.Console;


namespace DragAndDropInstaller;

internal class UserInterface
{

    public UserInterface() {}
    public static void UpdateReminder(string currentVersion, string localVersion)
    {
        Console.Title = $"Drag&Drop Installer v{localVersion} OUTDATED";
        var rule = new Rule($"[red]This version of the Drag&Drop Installer is outdated. Please download the {currentVersion} on flightsim.to![/]");
        rule.Justification = Justify.Left;
        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();
    }
    public static void InitiateInstall(string installPath)
    {
        var rule = new Rule("[deepskyblue1]You initiated the installation process from this archive:[/]");
        rule.Justification = Justify.Left;
        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();
        //AnsiConsole.MarkupLine("[blue]You initiated the installation process from this archive:[/]");
        AnsiConsole.WriteLine($"{installPath}\n");
    }
    public static void AttentionMultipleProfiles()
    {
        var rule = new Rule("[lightgoldenrod2_1]ATTENTION! Multiple profiles detected![/]");
        rule.Justification = Justify.Left;
        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();
        //AnsiConsole.MarkupLine("[gold1]ATTENTION! Multiple profiles detected![/] ");
    }
    public static string MultipleProfilesChoice(List<IArchiveEntry> entries)
    {
        string selectedEntry;

        SelectionPrompt<string> prompt = new SelectionPrompt<string>();
        foreach (IArchiveEntry entry in entries)
        {
            prompt.AddChoice(entry.Key);
        }
        string listFileTypes = entries[0].Key.Split('.')[^1].ToUpperInvariant();
        prompt.Title($"\nPlease select one of the {listFileTypes} files. [grey](Use up and down arrow keys)[/]");
        prompt.HighlightStyle(new Style().Foreground(Color.LightGoldenrod2_1));
        prompt.PageSize(5);
        prompt.MoreChoicesText("[grey](Move up and down to reveal more choices)[/]");
        selectedEntry = AnsiConsole.Prompt(prompt);
        AnsiConsole.MarkupLine($"[lightgoldenrod2_1]File selected:[/]\n[white] {selectedEntry}[/]");
        AnsiConsole.WriteLine();
        return selectedEntry;
    }
    public static void DisplayChanges(List<string> installedFiles, List<string> deletedFiles)
    {
        var rule = new Rule("[deepskyblue1]Installation results:[/]");
        rule.Justification = Justify.Left;
        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[green]Profile(s) installed:[/]");
        AnsiConsole.WriteLine(string.Join('\n', installedFiles));
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[red]Profile(s) overwritten:[/]");
        AnsiConsole.WriteLine(string.Join('\n', deletedFiles));
    }
    public static void DisplayElapsedTime(Stopwatch sw)
    {
        TimeSpan ts = sw.Elapsed;
        string elapsedTime = String.Format(System.Globalization.CultureInfo.InvariantCulture,
                                    "{0:00}.{1:00}",
                                    ts.Seconds,
                                    ts.Milliseconds / 10);

        AnsiConsole.MarkupLine($"\n[darkseagreen4]The installation process took {elapsedTime} seconds![/]");
    }
    public static void KeyToExit()
    {
        AnsiConsole.MarkupLine("\n[grey]Press any key to exit.[/]");
    }
    public static void ArgsNull()
    {
        AnsiConsole.MarkupLine("[red]ERROR: To install a GSX Pro Profile, please drag and drop the archive onto the executable.[/]");
    }
}
