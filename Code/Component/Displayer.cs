using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Submitter
{
    internal class Displayer
    {
        static void setForgeColor(ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
        }
        static void setBackgroundColor(ConsoleColor color = ConsoleColor.Black)
        {
            Console.BackgroundColor = color;
        }
        public static void output(string x, ConsoleColor textColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black, char final = '\0')
        {
            setForgeColor(textColor);
            setBackgroundColor(backgroundColor);
            Console.Write(x);
            Console.Write(final);
            setForgeColor();
            setBackgroundColor();
        }
    }
}
