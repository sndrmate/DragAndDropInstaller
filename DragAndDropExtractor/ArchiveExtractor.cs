/*
 * Copyright (c) 2023 smatthew
 * All rights reserved.
 */
using SharpCompress;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace DragAndDropInstaller;

class ArchiveExtractor
{
    string destinationPath;
    List<IArchiveEntry> extractQueue = new List<IArchiveEntry>();
    List<IArchiveEntry> installQueue1 = new List<IArchiveEntry>();
    List<IArchiveEntry> installQueue2 = new List<IArchiveEntry>();
    List<string> deletedFiles = new List<string>();
    List<string> installedFiles = new List<string>();

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
            if (IsSupportedFile(entry.Key))
            {
                Console.WriteLine($"DEBUG: FILE FOUND AND PROCESSING BEGINS: {entry.Key}"); //remove this line before merge to master
                extractQueue.Add(entry);
                string[] matchingFiles = Directory.GetFiles(destinationPath, $"*{GetICAOcode(entry.Key)}*");
                deletedFiles.AddRange(matchingFiles);
                matchingFiles.ForEach(filePath => File.Delete(filePath));
            }
        }
        if (extractQueue?.Any() == false)
        {
            throw new Exception("ERROR: The archive is empty or no relevant files have been found.\n"); //Need to improve error handling
        }
        foreach (IArchiveEntry entry in extractQueue)
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
    bool IsSupportedFile(string fileName)
    {
        if (fileName.EndsWith(".py", StringComparison.OrdinalIgnoreCase)
                    || fileName.EndsWith(".ini", StringComparison.OrdinalIgnoreCase))
        { return true; }
        return false;
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
