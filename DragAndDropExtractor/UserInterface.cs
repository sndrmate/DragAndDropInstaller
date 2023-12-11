using System;
using System.Diagnostics;
using SharpCompress.Archives;
using SharpCompress.Common;
using Spectre.Console;


namespace DragAndDropInstaller;

internal class UserInterface
{

    public UserInterface() { }
    public void InitiateInstall(string installPath)
    {
        AnsiConsole.MarkupLine("[blue]You initiated the installation process from this archive:[/]");
        AnsiConsole.WriteLine($"{installPath}\n");
    }
    public void AttentionMultipleProfiles()
    {
        AnsiConsole.MarkupLine("[gold1]ATTENTION! Multiple profiles detected![/] ");
    }
    public string MultipleProfilesChoice(List<IArchiveEntry> entries)
    {
        string selectedEntry;

        SelectionPrompt<string> prompt = new SelectionPrompt<string>();
        foreach (IArchiveEntry entry in entries)
        {
            prompt.AddChoice(entry.Key);
        }
        string listFileTypes = entries[0].Key.Split('.')[^1].ToUpperInvariant();
        prompt.Title($"\nPlease select one of the {listFileTypes} files.");
        prompt.HighlightStyle(new Style().Foreground(Color.Gold1));
        prompt.PageSize(5);
        prompt.MoreChoicesText("[grey](Move up and down to reveal more choices)[/]");
        selectedEntry = AnsiConsole.Prompt(prompt);
        AnsiConsole.MarkupLine($"[gold1]File selected:[/]\n[white] {selectedEntry}[/]\n");
        return selectedEntry;
    }
    public void DisplayChanges(List<string> installedFiles, List<string> deletedFiles)
    {
        AnsiConsole.MarkupLine("[green]Profile(s) installed:[/]");
        AnsiConsole.WriteLine(string.Join('\n', installedFiles));
        AnsiConsole.MarkupLine("\n[red]Profile(s) overwritten:[/]");
        AnsiConsole.WriteLine(string.Join('\n', deletedFiles));
    }
    public void DisplayElapsedTime(Stopwatch sw)
    {
        TimeSpan ts = sw.Elapsed;
        string elapsedTime = String.Format("{0:00}.{1:00}",
                                            ts.Seconds,
                                            ts.Milliseconds / 10);

        AnsiConsole.MarkupLine($"\n[green]The installation process was just {elapsedTime} seconds![/]");
    }
    public void KeyToExit()
    {
        AnsiConsole.MarkupLine("\n[grey]Press any key to exit.[/]");
    }
}
