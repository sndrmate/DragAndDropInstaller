// See https://aka.ms/new-console-template for more information
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            string filePath = args[0];
            Console.WriteLine($"You dropped the file: {filePath}");
        }
        else
        {
            Console.WriteLine("Please drag and drop a file onto the executable.");
        }
    }
}