/*
 * Copyright (c) 2023 smatthew
 * All rights reserved.
 */
using SharpCompress;
using SharpCompress.Archives;

namespace DragAndDropInstaller;

class ArchiveExtractor
{
    string destinationPath;
    List<IArchiveEntry> toExtract = new List<IArchiveEntry>();
    List<IArchiveEntry> DotPyFiles = new List<IArchiveEntry>();
    List<IArchiveEntry> DotIniFiles = new List<IArchiveEntry>();
    List<string> deletedFiles = new List<string>();
    List<string> installedFiles = new List<string>();
    ConsoleColor Default = Console.ForegroundColor;

    public ArchiveExtractor(string destinationPath)
    {

        this.destinationPath = destinationPath;
    }
    public void ExtractFiles(string archivePath)
    {
        //Error handling for unsupported archive types by (which is not in this list: Rar, Zip, Tar, Tar.GZip, Tar.BZip2, Tar.LZip, Tar.XZ, GZip(single file), 7Zip)
        using IArchive archive = ArchiveFactory.Open(archivePath);
        Console.ForegroundColor = ConsoleColor.Yellow; //remove this line before merge to master
        foreach (IArchiveEntry entry in archive.Entries)
        {
            if (IsDotIni(entry.Key))
            {
                Console.WriteLine($"DEBUG: FILE FOUND AND PROCESSING BEGINS: {entry.Key}\n"); //remove this line before merge to master
                DotIniFiles.Add(entry);
                string[] matchingFiles = Directory.GetFiles(destinationPath, $"*{GetICAOcode(entry.Key)}*");
                deletedFiles.AddRange(matchingFiles);
                matchingFiles.ForEach(filePath => File.Delete(filePath));
            }
            else if (IsDotPy(entry.Key))
            {
                Console.WriteLine($"DEBUG: FILE FOUND AND PROCESSING BEGINS: {entry.Key}\n"); //remove this line before merge to master
                DotPyFiles.Add(entry);
                string[] matchingFiles = Directory.GetFiles(destinationPath, $"*{GetICAOcode(entry.Key)}*");
                deletedFiles.AddRange(matchingFiles);
                matchingFiles.ForEach(filePath => File.Delete(filePath));
            }
        }
        if (DotIniFiles?.Any() == false && DotPyFiles?.Any() == false)
        {
            throw new Exception("ERROR: The archive is empty or no relevant files have been found.\n"); //Need to improve error handling
        }
        else if (DotIniFiles?.Count >= 2 && DotPyFiles?.Count == 1)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("ATTENTION! Multiple profiles detected!\nPlease choose one.");
            HandleMultipleProfiles(DotIniFiles);
            toExtract.AddRange(DotPyFiles);
        }
        else if (DotIniFiles?.Count >= 2 && DotPyFiles?.Count >= 2)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("ATTENTION! Multiple profiles detected!\nPlease choose one.");
            HandleMultipleProfiles(DotIniFiles);
            HandleMultipleProfiles(DotPyFiles);
        }
        else 
        {
            toExtract.AddRange(DotIniFiles);
            toExtract.AddRange(DotPyFiles);
        }
        foreach (IArchiveEntry entry in toExtract)
        {
            string filename = entry.Key;
            if (entry.Key.Contains('/')) { filename = entry.Key.Split('/').Last(); }
            string fullDestinationPath = Path.GetFullPath(Path.Combine(destinationPath, filename));
            installedFiles.Add(fullDestinationPath);
            using Stream stream = entry.OpenEntryStream();
            using FileStream writer = File.OpenWrite(fullDestinationPath);
            stream.CopyTo(writer);
        }
    }
    bool IsDotIni(string fileName)
    {
        if (fileName.EndsWith(".ini", StringComparison.OrdinalIgnoreCase))
        { return true; }
        return false;
    }
    bool IsDotPy(string fileName)
    {
        if (fileName.EndsWith(".py", StringComparison.OrdinalIgnoreCase))
        { return true; }
        return false;
    }
    void HandleMultipleProfiles(List<IArchiveEntry> list)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"\n{list.First().Key.Split('.').Last().ToUpper()} files:");
        for (int i = 0; i < list.Count; i++)
        {
            Console.WriteLine($"[{i}] {list[i].Key}");
        }
        Console.Write("Choice: ");
        Console.ForegroundColor = Default;
        toExtract.Add(list[int.Parse(Console.ReadLine())]);
    }
    public string DisplayInstalledFiles()
    {
        return string.Join('\n', installedFiles);
    }
    public string DisplayRemovedFiles()
    {
        return string.Join('\n', deletedFiles);
    }
    string GetICAOcode(string fileName)
    {
        if (fileName.Contains('/'))
        {
            return fileName.Split('/').Last().Split('-')[0];
        }
        return fileName.Split('-')[0];
    }
}
