/*
 * Copyright (c) 2023 smatthew
 * All rights reserved.
 */
using System.Diagnostics;
using SharpCompress;
using SharpCompress.Archives;

namespace DragAndDropInstaller;

internal class ArchiveExtractor(string destinationPath)
{
    private readonly List<IArchiveEntry> toExtract = [];
    private readonly List<IArchiveEntry> DotPyFiles = [];
    private readonly List<IArchiveEntry> DotIniFiles = [];
    private readonly List<IArchiveEntry> DotCfgFiles = [];
    private readonly List<string> deletedFiles = [];
    private readonly List<string> installedFiles = [];
    private bool multipleProfileFound;

    public void ExtractFiles(string archivePath)
    {
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
                case string cfgFile when cfgFile.EndsWith(".cfg", StringComparison.OrdinalIgnoreCase):
                    HandleSupportedFile(DotCfgFiles, entry);
                    break;
            }
        }

        if (DotIniFiles.Count == 0 && DotPyFiles.Count == 0 && DotCfgFiles.Count == 0)
        {
            throw new Exception("ERROR: The archive is empty or no relevant files have been found.\n"); //Need to improve error handling
        }

        HandleMultipleProfiles(DotIniFiles);
        HandleMultipleProfiles(DotPyFiles);
        Stopwatch sw = Stopwatch.StartNew();
        foreach (IArchiveEntry entry in toExtract)
        {
            //needs to be modified for cfg files (cfg files needs to be extract with its folder)
            string fullDestinationPath = Path.GetFullPath(Path.Combine(destinationPath, GetFileName(entry.Key)));
            installedFiles.Add(fullDestinationPath);
            using Stream stream = entry.OpenEntryStream();
            using FileStream writer = File.OpenWrite(fullDestinationPath);
            stream.CopyTo(writer);
        }
        sw.Stop();
        UserInterface.DisplayChanges(installedFiles, deletedFiles);
        UserInterface.DisplayElapsedTime(sw);
    }
    private void HandleSupportedFile(List<IArchiveEntry> list, IArchiveEntry entry)
    {
        //needs to be modified for cfg files, there should be no deletion if its a .cfg file
        list.Add(entry);
        string[] matchingFiles = Directory.GetFiles(destinationPath, $"*{GetICAOcode(entry.Key)}*");
        deletedFiles.AddRange(matchingFiles);
        matchingFiles.ForEach(filePath => File.Delete(filePath));
    }

    private void HandleMultipleProfiles(List<IArchiveEntry> list)
    {
        if (list.Count <= 1)
        {
            if (list.Count == 1)
            {
                toExtract.Add(list[0]);
                return;
            }
            return;
        }
        if (!multipleProfileFound)
        {
            UserInterface.AttentionMultipleProfiles();
            multipleProfileFound = true;
        }
        string choice = UserInterface.MultipleProfilesChoice(list);
        IArchiveEntry selectedEntry = list.Find(entry => entry.Key == choice);
        toExtract.Add(selectedEntry);
    }

    private static string GetFileName(string fileName)
    {
        return fileName switch
        {
            string fileN when fileN.Contains('/', StringComparison.OrdinalIgnoreCase) => fileName.Split('/')[^1],
            string fileN when fileN.Contains('\\', StringComparison.OrdinalIgnoreCase) => fileName.Split('\\')[^1],
            _ => fileName,
        };
    }
    private static string GetICAOcode(string fileName)
    {
        return fileName switch
        {
            string fileN when fileN.Contains('/', StringComparison.OrdinalIgnoreCase) => fileName.Split('/')[^1].Split('-')[0],
            string fileN when fileN.Contains('\\', StringComparison.OrdinalIgnoreCase) => fileName.Split('\\')[^1].Split('-')[0],
            _ => fileName,
        };
    }
}
