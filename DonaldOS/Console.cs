using Cosmos.System.FileSystem;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CosmosSys = Cosmos.System;
using Sys = System;

namespace DonaldOS
{
    static internal class Console
    {

    public static int WindowWidth = Sys.Console.WindowWidth;
    public static int WindowHeight = Sys.Console.WindowHeight;

    private static List<string> consoleLines = new List<string>();
    private static List<string> previousCommands = new List<string>();
    private static int currentCommandIndex = -1;
    private static char bufferedChar = '\0';

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
            if (bufferedChar != '\0')
            {
                input = input.Insert(0, bufferedChar.ToString());
                bufferedChar = '\0';
            }

            consoleLines[0] += input;
            consoleLines.Insert(0, "");
            previousCommands.Insert(0, input);
            return input;
        }

        public static Sys.ConsoleKeyInfo PeekKey()
        {
            var key = Sys.Console.ReadKey();
            bufferedChar = key.KeyChar;
            return key;
        }


        public static void checkScrolling()
        {
            int scrollDiff = (int)CosmosSys.MouseManager.Y - 1000;
            if (Sys.Math.Abs(scrollDiff) >= scrollThreshold)
            {
                int minScrollOffset = Sys.Math.Max(0, consoleLines.Count - 25) * -1;
                int maxScrollOffset = 0;
                //if ((scrollDiff > 0 && currentScrollOffset >= maxScrollOffset) || (scrollDiff < 0 && currentScrollOffset <= minScrollOffset))
                //{
                //    Sys.Console.Write("Min: " + minScrollOffset + "; Max: " + maxScrollOffset + "; Current: " + currentScrollOffset + " | ");
                //    CosmosSys.MouseManager.Y = 1000;
                //    return;
                //}

                currentScrollOffset += scrollDiff / -100;
                CosmosSys.MouseManager.Y = 1000;

                reprint();
                currentCommandIndex = -1;
            }
        }

        // negative n scrolls up, positive n scrolls down
        public static void scrollNLines(int n)
        {
            currentScrollOffset += n;
            reprint();
        }

        public static void reprint()
        {
            Sys.Console.Clear();
            for (int i = currentScrollOffset + 24; i > 0 && i >= currentScrollOffset; i--)
            {
                if (i >= consoleLines.Count)
                {
                    Sys.Console.WriteLine("");
                    continue;
                }
                Sys.Console.WriteLine(consoleLines[i]);
            }

            Sys.Console.Write(consoleLines[0]);
        }

        public static void previousCommand()
        {
            if (currentCommandIndex >= previousCommands.Count - 1)
            {
                return;
            }
            currentCommandIndex++;
            reprint();
            Sys.Console.Write(previousCommands[currentCommandIndex]);
        }

        public static void nextCommand()
        {
            if (currentCommandIndex == -1)
            {
                return;
            }

            if (currentCommandIndex == 0)
            {
                currentCommandIndex = -1;
                reprint();
                return;
            }

            currentCommandIndex--;
            reprint();
            Sys.Console.Write(previousCommands[currentCommandIndex]);
        }

        public static string getCurrentCommmand()
        {
            if (currentCommandIndex == -1)
            {
                return null;
            }

            int index = currentCommandIndex;
            currentCommandIndex = -1; // reset to default

            previousCommands.Add(previousCommands[index]);
            consoleLines[0] += previousCommands[index];
            consoleLines.Insert(0, "");
            return previousCommands[index];
        }

        public static void printBootScreen()
        {
            Sys.Console.Write(DonaldHimself.ascii);
            Sys.Threading.Thread.Sleep(500);
            Sys.Console.Clear();
            WriteLine(DonaldHimself.name);
            Sys.Console.ForegroundColor = Sys.ConsoleColor.Green;
            WriteLine("If you are an old white man: Welcome to DonaldOS! Type HELP to learn what you can do with this GREAT BEAUTIFUL SYSTEM!\n\n");
            Sys.Console.ForegroundColor = Sys.ConsoleColor.White;
        }
        
        public static void Clear()
        {
            Sys.Console.Clear();
        }

        public static void ClearAndReprint(List<string> lineBuffer)
        {
            Sys.Console.Clear();
            consoleLines.Clear();
            foreach (string line in lineBuffer)
            {
                consoleLines.Insert(0, line);
            }
            reprint();
        }

        public static Sys.ConsoleKeyInfo ReadKey()
        {
            return Sys.Console.ReadKey();
        }

        public static void SetCursorPosition(int x, int y)
        {
            Sys.Console.SetCursorPosition(x, y);
        }
    }
}
