// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.IO.Compression;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            string filePath = args[0];
            Console.WriteLine($"You dropped the file: {filePath}");
            string extractPath = @".\\extract";
            ZipFile.ExtractToDirectory(filePath, extractPath);
        }
        else
        {
            Console.WriteLine("Please drag and drop a file onto the executable.");
        }
        Console.ReadKey();
    }
}