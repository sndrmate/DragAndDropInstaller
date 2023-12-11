/*
 * Copyright (c) 2023 smatthew
 * All rights reserved.
 */
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

    public ArchiveExtractor(string destinationPath)
    {

        this.destinationPath = destinationPath;
    }
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
            }
        }

        if (!DotIniFiles.Any() && !DotPyFiles.Any())
        {
            throw new Exception("ERROR: The archive is empty or no relevant files have been found.\n"); //Need to improve error handling
        }

        HandleMultipleProfiles(DotIniFiles);
        HandleMultipleProfiles(DotPyFiles);

        foreach (IArchiveEntry entry in toExtract)
        {
            string filename = entry.Key.Contains('/', StringComparison.OrdinalIgnoreCase) ? entry.Key.Split('/')[^1] : entry.Key;
            string fullDestinationPath = Path.GetFullPath(Path.Combine(destinationPath, filename));
            installedFiles.Add(fullDestinationPath);
            using Stream stream = entry.OpenEntryStream();
            using FileStream writer = File.OpenWrite(fullDestinationPath);
            stream.CopyTo(writer);
        }
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
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("ATTENTION! Multiple profiles detected!\nPlease choose one.");
        Console.WriteLine($"\n{list[0].Key.Split('.')[^1].ToUpperInvariant()} files:");
        for (int i = 0; i < list.Count; i++)
        {
            Console.WriteLine($"[{i}] {list[i].Key}");
        }
        Console.Write("Enter your choice: ");
        Console.ForegroundColor = defaultColor;

        bool valid = false;
        short choice = -1;
        while (!valid)
        {
            valid = short.TryParse(Console.ReadLine(), out choice) && choice >= 0 && choice < list.Count;
        }
        toExtract.Add(list[choice]);
    }
    public string DisplayInstalledFiles()
    {
        return string.Join('\n', installedFiles);
    }
    public string DisplayRemovedFiles()
    {
        return string.Join('\n', deletedFiles);
    }

    private static string GetICAOcode(string fileName)
    {
        return fileName.Contains('/', StringComparison.OrdinalIgnoreCase)
            ? fileName.Split('/')[^1].Split('-')[0]
            : fileName.Split('-')[0];
    }
}
