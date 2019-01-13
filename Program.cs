using System.IO;
using System;

namespace MarkdownImageFix
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("please provide a path to the file");
            }
            else
            {
                var filePath = args[0];
                var imageConverter = new MarkdownImageConverter();
                File.WriteAllText($"{filePath}.converted", imageConverter.Convert(File.ReadAllText(filePath)));
            }
        }
    }
}
