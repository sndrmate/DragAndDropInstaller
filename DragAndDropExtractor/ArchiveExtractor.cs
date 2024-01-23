/*
 * Copyright (c) 2023 smatthew
 * All rights reserved.
 */
using System.Diagnostics;
using SharpCompress;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace DragAndDropInstaller;

internal class ArchiveExtractor()
{
    private readonly List<IArchiveEntry> toExtract = [];
    private readonly List<string> previousInstalls = [];
    private readonly List<IArchiveEntry> DotPyFiles = [];
    private readonly List<IArchiveEntry> DotIniFiles = [];
    private readonly List<IArchiveEntry> DotCfgFiles = [];
    private readonly List<string> deletedFiles = [];
    private readonly List<string> installedFiles = [];
    private readonly string profilesPath = Path.Combine(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)), "Virtuali", "GSX", "MSFS");
    private readonly string airplanesPath = Path.Combine(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)), "Virtuali", "Airplanes");
    private bool multipleProfileFound;
    public void StartArchiveProcessing(string[] archivePaths)
    {
        for (int i = 0; i < archivePaths.Length; i++)
        {
            if (File.Exists(archivePaths[i]))
            {
                if (i < 1)
                {
                    UserInterface.InitiateInstall(archivePaths[i], i + 1, archivePaths.Length);
                    ExtractFiles(archivePaths[i]);
                    UserInterface.BeforeNextInstall(i, archivePaths.Length);       
                }
                else
                {
                    UserInterface.SummaryOfPreviousInstall(previousInstalls ,archivePaths.Length);
                    DotCfgFiles.Clear();
                    DotIniFiles.Clear();
                    DotPyFiles.Clear();
                    installedFiles.Clear();
                    deletedFiles.Clear();
                    multipleProfileFound = false;
                    toExtract.Clear();
                    UserInterface.InitiateInstall(archivePaths[i], i + 1, archivePaths.Length);
                    ExtractFiles(archivePaths[i]);
                    UserInterface.BeforeNextInstall(i, archivePaths.Length);
                }
                
            }
            else
            {
                UserInterface.ArgsNull();
                UserInterface.KeyToExit();
                Console.ReadKey();
                throw new Exception();
            }

        }
    }
    private void ExtractFiles(string archivePath)
    {
        previousInstalls.Add(archivePath.Split('\\')[^1]);
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
                default:
                    throw new Exception();
                    break;
            }
        }

        if (DotIniFiles.Count == 0 && DotPyFiles.Count == 0 && DotCfgFiles.Count == 0)
        {
            throw new Exception("ERROR: The archive is empty or no relevant files have been found.\n"); //Need to improve error handling
        }

        HandleMultipleProfiles(DotIniFiles);
        HandleMultipleProfiles(DotPyFiles);
        HandleMultipleProfiles(DotCfgFiles);
        Stopwatch sw = Stopwatch.StartNew();
        Extract();
        sw.Stop();
        UserInterface.DisplayChanges(installedFiles, deletedFiles);
        UserInterface.DisplayElapsedTime(sw);
    }
    private void Extract()
    {
        //needs to be modified for cfg files (cfg files needs to be extract with its folder)
        foreach (IArchiveEntry entry in toExtract)
        {
            if (entry.Key.EndsWith(".cfg", StringComparison.OrdinalIgnoreCase))
            {
                ExtractCfgFile(entry);
            }
            else
            {
                string fullDestinationPath = Path.GetFullPath(Path.Combine(profilesPath, GetFileName(entry.Key)));
                installedFiles.Add(fullDestinationPath);
                using Stream stream = entry.OpenEntryStream();
                using FileStream writer = File.OpenWrite(fullDestinationPath);
                stream.CopyTo(writer);
            }
        }
    }
    private void HandleSupportedFile(List<IArchiveEntry> list, IArchiveEntry entry)
    {
        //needs to be modified for cfg files, there should be no deletion if its a .cfg file. But somehow we should indicate, if the profile is overwritten?
        if (entry.Key.EndsWith(".cfg", StringComparison.OrdinalIgnoreCase))
        {

            list.Add(entry);
            return;
        }
        list.Add(entry);
        string[] matchingFiles = Directory.GetFiles(profilesPath, $"*{GetICAOcode(entry.Key)}*");
        deletedFiles.AddRange(matchingFiles);
        matchingFiles.ForEach(File.Delete);
    }

    private void HandleMultipleProfiles(List<IArchiveEntry> list)
    {
        //Multiple selection for .CFG files
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
        List<string> choices = [];
        choices.AddRange(UserInterface.MultipleProfilesChoice(list));
        foreach (string choice in choices)
        {
            toExtract.Add(list.Find(entry => entry.Key == choice));
        }
    }

    private static string GetFileName(string fileName)
    {
        return fileName switch
        {
            string fileN when fileN.Contains('/', StringComparison.Ordinal) => fileName.Split('/')[^1],
            string fileN when fileN.Contains('\\', StringComparison.Ordinal) => fileName.Split('\\')[^1],
            _ => fileName,
        };
    }
    private void ExtractCfgFile(IArchiveEntry entry)
    {
        string ContainingDirectory;
        ContainingDirectory = entry.Key.Contains('\\', StringComparison.Ordinal) ? entry.Key.Split('\\')[^2] : entry.Key.Split('/')[^2];
        string PathToDirectory = Path.GetFullPath(Path.Combine(airplanesPath, ContainingDirectory));
        string fullDestinationPath = Path.GetFullPath(Path.Combine(airplanesPath, ContainingDirectory, GetFileName(entry.Key)));
        if (!Directory.Exists(PathToDirectory))
        {
            Directory.CreateDirectory(PathToDirectory);
            
        }
        else
        {
            deletedFiles.Add(fullDestinationPath);
        }
        installedFiles.Add(fullDestinationPath);
        using Stream stream = entry.OpenEntryStream();
        using FileStream writer = File.OpenWrite(fullDestinationPath);
        stream.CopyTo(writer);
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
