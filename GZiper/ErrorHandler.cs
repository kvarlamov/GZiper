using System;
using System.IO;

namespace GZiper
{
    internal static class ErrorHandler
    {
        public static void CheckForInputError(string[] args)
        {
            if (args.Length == 0 || args.Length > 3)
            {
                
                throw new Exception($"Incorrect input.\nPlease follow the pattern:\n" +
                                    $"  for compress  : GZiper.exe compress [имя исходного файла] [имя результирующего файла]\n" +
                                    $"  for decompress: GZiper.exe decompress [имя исходного файла] [имя результирующего файла]");
            }
            
            if (args[0] != "compress" && args[0] != "decompress")
            {
                throw new Exception("First argument should be: \"compress\" for compressing\n" +
                                                              "\"decompress\" for decompressing\n");
            }

            if (args[1].Length == 0)
            {
                throw new Exception("You forgot to print source file name");
            }
            if (args[2].Length == 0)
            {
                throw new Exception("You forgot to print final file name");
            }

            FileInfo inputFile  = new FileInfo(args[1]);
            FileInfo outputFile = new FileInfo(args[2]);

            if (!inputFile.Exists)
            {
                throw new Exception("File not found. \nCheck the file name. \nDon't forget print file extension");
            }

            if (outputFile.Exists || File.Exists(outputFile.Name +".gz"))
            {
                throw new Exception("Final file is already exist. Please choose another file name for final file");
            }

            if (inputFile.Name == outputFile.Name)
            {
                throw new Exception("Source file and final file should have different names");
            }

            if (outputFile.Extension == "")
            {
                throw new Exception("You don't print final file extension. \nPlease insert extension of both files");
            }

            if (inputFile.Extension == ".gz" && args[0] == "compress")
            {
                throw new Exception("File is already compressed");
            }

            if (inputFile.Extension != ".gz" && args[0] == "decompress")
            {
                throw new Exception("File for decompression must have \".gz\" extension");
            }
        }
    }
}