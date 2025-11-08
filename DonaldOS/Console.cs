using Cosmos.System.FileSystem;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = System;
using CosmosSys = Cosmos.System;

namespace DonaldOS
{
    static internal class Console
    {
    private static List<string> consoleLines = new List<string>();

    private static int scrollThreshold = 100;
    private static int currentScrollOffset = 0;

        static Console()
        {
            consoleLines.Add("");
        }
        public static void Write(string data)
        {
            foreach (string line in data.Split('\n'))
            {
                consoleLines[0] += line;
                consoleLines.Insert(0, "");
            }
            consoleLines.RemoveAt(0);
            Sys.Console.Write(data);
        }

        public static void Write(int data)
        {
            consoleLines[0] = data.ToString();
            Sys.Console.Write(data);
        }

        public static void WriteLine(string data) 
        {
            foreach (string line in data.Split('\n'))
            {
                consoleLines[0] += line;
                consoleLines.Insert(0, "");
            }
            Sys.Console.WriteLine(data);
        }

        public static void WriteLine(int data)
        {
            consoleLines[0] += data.ToString();
            consoleLines.Insert(0, "");
            Sys.Console.WriteLine(data);
        }

        public static string ReadLine()
        {
            string input = Sys.Console.ReadLine();
            consoleLines[0] += input;
            consoleLines.Insert(0, "");
            return input;
        }


        public static void checkScrolling()
        {
            if (Sys.Math.Abs((int)CosmosSys.MouseManager.Y - 1000) >= scrollThreshold)
            {
                currentScrollOffset += ((int)CosmosSys.MouseManager.Y - 1000) / -100;
                CosmosSys.MouseManager.Y = 1000;
                // Sys.Console.WriteLine(currentScrollOffset);
                reprint();
            }
        }

        public static void reprint()
        {
            Sys.Console.Clear();
            for (int i = currentScrollOffset + 24; i >= 0 && i >= currentScrollOffset; i--)
            {
                if (i >= consoleLines.Count)
                {
                    Sys.Console.WriteLine("");
                    continue;
                }
                Sys.Console.WriteLine(consoleLines[i]);
            }
        }
    }
}
