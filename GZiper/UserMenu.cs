using GZiper.Archivator;
using System;

namespace GZiper
{
    internal static class UserMenu
    {
        static GZip _archivator;
        internal static void Launch(string[] args)
        {
            InputPattern();

            //for debugging or testing
            #region TestInput  
            //args = new string[3];
            //args[0] = "compress";
            //args[1] = "SomeMovie.mkv";
            //args[2] = "compressed.mkv";
            #endregion

            try
            {
                ErrorHandler.CheckForInputError(args);

                switch (args[0])
                {
                    case "compress":
                        _archivator = new Compressor(args[1], args[2]);
                        break;
                    case "decompress":
                        _archivator = new Decompressor(args[1], args[2]);
                        break;
                }
                _archivator.Start();
                Program.result = _archivator.ReturnResult();
            }
            catch (Exception ex)
            {
                #region ExceptionView
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("\n***ERROR!***");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("--- ERROR MESSAGE--- \n");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n--- Additional information--- ");
                Console.ResetColor();
                Console.WriteLine("\n--- Error Source ---\n{0}", ex.Source);
                Console.WriteLine("\n--- StackTrace ---\n{0}", ex.StackTrace);
                Console.WriteLine("\n--- TargetSite ---\n{0}", ex.TargetSite);
                Program.result = 1; 
                #endregion
            }
            Console.ResetColor();
        }

        private static void InputPattern()
        {
            #region MenuView
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("***Archivator***");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("For file compression   (without []): ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("GZipTest.exe compress [имя исходного файла] [имя результирующего файла]");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("For file decompression (without []): ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("GZipTest.exe decompress [имя исходного файла] [имя результирующего файла]");
            Console.ResetColor(); 
            #endregion
        }
    }
}