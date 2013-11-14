using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TuneRoboWPF.Utility
{
    public static class DebugHelper
    {
        public static void WriteLineDebug(object text)
        {
#if DEBUG
            Console.WriteLine(text.ToString());
#endif
        }
        public static void WriteDebug(object text)
        {
#if DEBUG
            Console.Write(text.ToString());
#endif 
        }
    }
}
