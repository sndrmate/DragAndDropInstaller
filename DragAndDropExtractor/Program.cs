/*
  Copyright (c) smatthew 2023

  All rights reserved. 

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
  of the Software, and to permit persons to whom the Software is furnished to do so, 
  subject to the following conditions:

  1. The above copyright notice and this permission notice shall be included in all 
  copies or substantial portions of the Software.

  2. No part of this software may be modified, sold, or resold without the explicit
  written permission of the copyright holder.

  THE SOFTWARE IS PROVIDED "AS IS," WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
  INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
  PARTICULAR PURPOSE, AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
  HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES, OR OTHER LIABILITY, WHETHER IN AN ACTION
  OF CONTRACT, TORT, OR OTHERWISE, ARISING FROM, OUT OF, OR IN CONNECTION WITH THE
  SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "GSX Pro Profile v1.0 | by smatthew & FatGingerHead";
        var DefaultColor = Console.ForegroundColor;
        string extractPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+ "\\Virtuali\\GSX\\MSFS");
        if (args.Length > 0)
        {
            try
            {
                string ZipPath = args[0];
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"You dropped the file: {ZipPath}");
                Console.ForegroundColor = DefaultColor;

                if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                    extractPath += Path.DirectorySeparatorChar;

                using (ZipArchive archive = ZipFile.OpenRead(ZipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string icao_code = entry.FullName.Split('-')[0];
                        string[] matchingFiles = Directory.GetFiles(extractPath, $"*{icao_code}*");

                        if (matchingFiles.Any())
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\nFiles overwritten:");
                            Console.ForegroundColor = DefaultColor;
                            foreach (string file in matchingFiles)
                            {
                                Console.WriteLine(file);
                                File.Delete(file);
                            }
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nExtracted files:");
                    Console.ForegroundColor = DefaultColor;
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(".py", StringComparison.OrdinalIgnoreCase) || entry.FullName.EndsWith(".ini", StringComparison.OrdinalIgnoreCase))
                        {   
                            // Gets the full path to ensure that relative segments are removed.
                            string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

                            // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                            // are case-insensitive.
                            if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                            {
                                Console.WriteLine(destinationPath);
                                entry.ExtractToFile(destinationPath);
                            }
                                
                        }
                    }
                }
                Console.WriteLine("\nPress any key to exit.");
            }
            catch (Exception e)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ForegroundColor = DefaultColor;
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
            }
            
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Please drag and drop a file onto the DragAndDropExtractor.exe.");
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine("\nPress any key to exit.");
        }
        Console.ReadKey();
    }
}