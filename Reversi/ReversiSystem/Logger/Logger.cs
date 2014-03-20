using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.ReversiSystem.Debug
{
    class Logger
    {
        public static void String(bool b)
        {
            String(b.ToString());
        }

        public static void String(string s)
        {
            Console.WriteLine(s);
        }
    }
}
