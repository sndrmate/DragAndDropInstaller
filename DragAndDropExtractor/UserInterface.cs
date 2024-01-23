using System.Diagnostics;
using SharpCompress.Archives;
using Spectre.Console;


namespace DragAndDropInstaller;

internal class UserInterface
{
    private UserInterface() { }
    public static async Task UpdateReminderAsync(string currentVersion, string localVersion)
    {
        Console.Title = $"Drag&Drop Installer v{localVersion} !!!THIS VERSION IS OUTDATED, PLEASE DOWNLOAD THE NEWEST VERSION FROM FLIGHTSIM.TO!!!";
        //var rule = new Rule($"[red]This version of the Drag&Drop Installer is outdated. Please download the v{currentVersion} on flightsim.to![/]")
        //{
        //    Justification = Justify.Left
        //};
        //AnsiConsole.Write(rule);
        //AnsiConsole.WriteLine();
    }
    public static void InitiateInstall(string installPath, int serial, int length)
    {
        if (serial > 1)
        {
            AnsiConsole.WriteLine();
        }
        if (length > 1)
        {
            var rule = new Rule($"[deepskyblue1]({serial}/{length}) You initiated the installation process from this archive:[/]")
            {
                Justification = Justify.Left
            };
            AnsiConsole.Write(rule);
        }
        else
        {
            var rule = new Rule("[deepskyblue1]You initiated the installation process from this archive:[/]")
            {
                Justification = Justify.Left
            };
            AnsiConsole.Write(rule);
        }
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine($"{installPath}\n");
    }
    public static void BeforeNextInstall(int i, int length)
    {
        if (length > 1)
        {
            if (length - 1 == i)
            {
                AnsiConsole.MarkupLine("\n[seagreen3]All profile installations complete.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("\n[seagreen3]This profile has been installed, press any key to continue.[/]");
                Console.ReadKey();
            }
        }
    }
    public static void SummaryOfPreviousInstall(List<string> previousInstalls, int lenght)
    {
        AnsiConsole.Clear();
        for (int i = 0; i < previousInstalls.Count; i++)
        {
            AnsiConsole.MarkupLine($"[steelblue3]({i+1}/{lenght}) Installed profile from this archive:[/] {previousInstalls[i]} ");
        }     
    }
    public static void AttentionMultipleProfiles()
    {
        var rule = new Rule("[lightgoldenrod2_1]ATTENTION! Multiple profiles detected![/]")
        {
            Justification = Justify.Left
        };
        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();
    }
    public static List<string> MultipleProfilesChoice(List<IArchiveEntry> entries)
    {
        List<string> selectedEntries = [];
        if (entries.First().Key.EndsWith(".cfg"))
        {
            selectedEntries.AddRange(MultipleAircraftChoice(entries));
        }
        else
        {
            SelectionPrompt<string> prompt = new();
            foreach (IArchiveEntry entry in entries)
            {
                prompt.AddChoice(entry.Key);
            }
            string listFileTypes = entries[0].Key.Split('.')[^1].ToUpperInvariant();
            prompt.Title($"\nPlease select one of the {listFileTypes} files to install the profile. [grey](Use up and down arrow keys)[/]");
            prompt.HighlightStyle(new Style().Foreground(Color.LightGoldenrod2_1));
            prompt.PageSize(5);
            prompt.MoreChoicesText("[grey](Move up and down to reveal more choices)[/]");
            selectedEntries.Add(AnsiConsole.Prompt(prompt));
            AnsiConsole.MarkupLine($"[lightgoldenrod2_1]Profile selected:[/]\n[white] {selectedEntries.First()}[/]");
            AnsiConsole.WriteLine();
        }
        return selectedEntries;
    }
    public static List<string> MultipleAircraftChoice(List<IArchiveEntry> entries)
    {
        MultiSelectionPrompt<string> prompt = new();
        foreach (IArchiveEntry entry in entries)
        {
            prompt.AddChoice(entry.Key);
        }
        prompt.Title($"\nPlease select which profile(s) to install. [grey](Use up and down arrow keys)[/]");
        prompt.HighlightStyle(new Style().Foreground(Color.LightGoldenrod2_1));
        prompt.PageSize(5);
        prompt.MoreChoicesText("[grey](Move up and down to reveal more choices)[/]");
        List<string> selectedEntries = AnsiConsole.Prompt(prompt);
        foreach (string selectedEntry in selectedEntries)
        {
            AnsiConsole.MarkupLine($"[lightgoldenrod2_1]Profile selected:[/]\n[white] {selectedEntry}[/]");
        }     
        AnsiConsole.WriteLine();
        return selectedEntries;
    }
    public static void DisplayChanges(List<string> installedFiles, List<string> deletedFiles)
    {
        var rule = new Rule("[deepskyblue1]Installation results:[/]")
        {
            Justification = Justify.Left
        };
        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[green]Profile(s) installed:[/]");
        AnsiConsole.WriteLine(string.Join('\n', installedFiles));
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[red]Profile(s) overwritten:[/]");
        if (deletedFiles.Count == 0)
        {
            AnsiConsole.WriteLine("None");
        }
        else
        {
            AnsiConsole.WriteLine(string.Join('\n', deletedFiles));
        }
    }
    public static void DisplayElapsedTime(Stopwatch sw)
    {
        TimeSpan ts = sw.Elapsed;
        string elapsedTime = String.Format(System.Globalization.CultureInfo.InvariantCulture,
                                    "{0:00}.{1:00}",
                                    ts.Seconds,
                                    ts.Milliseconds);

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
