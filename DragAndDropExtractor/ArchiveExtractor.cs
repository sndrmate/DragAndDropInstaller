/*
 * Copyright (c) 2023 smatthew
 * All rights reserved.
 */
using System.Diagnostics;
using SharpCompress;
using SharpCompress.Archives;

namespace DragAndDropInstaller;

internal class ArchiveExtractor
{
    private readonly string destinationPath;
    private readonly List<IArchiveEntry> toExtract = new();
    private readonly List<IArchiveEntry> DotPyFiles = new();
    private readonly List<IArchiveEntry> DotIniFiles = new();
    private readonly List<string> deletedFiles = new();
    private readonly List<string> installedFiles = new();
    private readonly ConsoleColor defaultColor = Console.ForegroundColor;
    private bool multipleProfileFound = false;
    UserInterface UI = new();

    public ArchiveExtractor(string destinationPath)
    {

        this.destinationPath = destinationPath;
    }
    public void ExtractFiles(string archivePath)
    {
        Stopwatch sw = new Stopwatch();

        //Error handling for unsupported archive types (which is not in this list: Rar, Zip, Tar, Tar.GZip, Tar.BZip2, Tar.LZip, Tar.XZ, GZip(single file), 7Zip)
        using IArchive archive = ArchiveFactory.Open(archivePath);
        foreach (IArchiveEntry entry in archive.Entries)
        {
            switch (entry.Key)
            {
                case string iniFile when iniFile.EndsWith(".ini", StringComparison.OrdinalIgnoreCase):
                    HandleSupportedFile(DotIniFiles, entry);
                    break;
                case string pyFile when pyFile.EndsWith(".py", StringComparison.OrdinalIgnoreCase):
                    HandleSupportedFile(DotPyFiles, entry);
                    break;
            }
        }

        if (!DotIniFiles.Any() && !DotPyFiles.Any())
        {
            throw new Exception("ERROR: The archive is empty or no relevant files have been found.\n"); //Need to improve error handling
        }

        HandleMultipleProfiles(DotIniFiles);
        HandleMultipleProfiles(DotPyFiles);
        sw.Start();
        foreach (IArchiveEntry entry in toExtract)
        {
            string fullDestinationPath = Path.GetFullPath(Path.Combine(destinationPath, GetFileName(entry.Key)));
            installedFiles.Add(fullDestinationPath);
            using Stream stream = entry.OpenEntryStream();
            using FileStream writer = File.OpenWrite(fullDestinationPath);
            stream.CopyTo(writer);
        }
        sw.Stop();
        UI.DisplayChanges(installedFiles, deletedFiles);
        UI.DisplayElapsedTime(sw);
    }
    private void HandleSupportedFile(List<IArchiveEntry> list, IArchiveEntry entry)
    {
        list.Add(entry);
        string[] matchingFiles = Directory.GetFiles(destinationPath, $"*{GetICAOcode(entry.Key)}*");
        deletedFiles.AddRange(matchingFiles);
        matchingFiles.ForEach(filePath => File.Delete(filePath));
    }

    private void HandleMultipleProfiles(List<IArchiveEntry> list)
    {
        if (list.Count <= 1)
        {
            if (list.Count < 1)
            {
                return;
            }
            toExtract.Add(list[0]);
            return;
        }
        if (!multipleProfileFound)
        {
            UI.AttentionMultipleProfiles();
            multipleProfileFound = true;
        }
        string choice = UI.MultipleProfilesChoice(list);
        foreach (IArchiveEntry entry in list)
        {
            if (entry.Key == choice)
            {
                toExtract.Add(entry);
            }
        }
    }

    private static string GetFileName(string fileName)
    {
        switch (fileName)
        {
            case string fileN when fileN.Contains('/', StringComparison.OrdinalIgnoreCase):
                return fileName.Split('/')[^1];

            case string fileN when fileN.Contains('\\', StringComparison.OrdinalIgnoreCase):
                return fileName.Split('\\')[^1];
            default:
                return fileName;
        }
    }
    private static string GetICAOcode(string fileName)
    {
        switch (fileName)
        {
            case string fileN when fileN.Contains('/', StringComparison.OrdinalIgnoreCase):
                return fileName.Split('/')[^1].Split('-')[0];

            case string fileN when fileN.Contains('\\', StringComparison.OrdinalIgnoreCase):
                return fileName.Split('\\')[^1].Split('-')[0];
            default:
                return fileName;
        }
    }
}
