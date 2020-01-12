using System;

namespace GZiper
{
    class Program
    {       
        public static int result = 0;

        static int Main(string[] args)
        {
            UserMenu.Launch(args);

            Console.WriteLine(result);
            return result;
        }
    }
}
