using Cosmos.System.FileSystem;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = System;
using CosmosSys = Cosmos.System;
using Microsoft.VisualBasic.FileIO;
using Cosmos.System;

namespace DonaldOS
{
    static internal class Console
    {

    public static int WindowWidth = Sys.Console.WindowWidth;
    public static int WindowHeight = Sys.Console.WindowHeight;

    private static List<string> consoleLines = new List<string>();
    private static List<string> previousCommands = new List<string>();
    private static int currentCommandIndex = -1;
    private static int cursorPosition = 0;

    private static int scrollThreshold = 100;
    private static int currentScrollOffset = 0;

    private static string prompt = "";
    private static int promptLength;

        static Console()
        {
            consoleLines.Add("");
            consoleLines.Add("and are in no way intended as an assessment.");
            consoleLines.Add("exclusively for the sake of originality and entertainment ");
            consoleLines.Add("All references to some actual existing person in a high political position are ");
        }
        public static void Write(string data)
        {
            foreach (string line in data.Split('\n'))
            {
                for (int i = 0; i <= line.Length; i += 80)
                {
                    consoleLines[0] += line.Substring(i, Sys.Math.Min(i + 79, line.Length - i));
                    consoleLines.Insert(0, "");
                }
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
                for (int i = 0; i <= line.Length; i += 80)
                {
                    consoleLines[0] += line.Substring(i, Sys.Math.Min(i + 79, line.Length - i));
                    consoleLines.Insert(0, "");
                }
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

            for (int i = 0; i < input.Length; i += 80)
            {
                consoleLines[0] += input.Substring(i, Sys.Math.Min(i + 79, input.Length - i));
                consoleLines.Insert(0, "");
            }
            previousCommands.Insert(0, input);
            return input;
        }

        public static void printPrompt()
        {
            prompt = Kernel.SharedUserManager.CurrentUser.Username + "!" + CommandExecutionHelper.currentPath + "!$$$ ";
            Write(prompt);
            promptLength = prompt.Length;
        }

        public static string getPasswordFromUser()
        {
            string password = "";

            while (true)
            {
                if (Sys.Console.KeyAvailable)
                {
                    CosmosSys.KeyEvent key = CosmosSys.KeyboardManager.ReadKey();
                    switch (key.Key.ToConsoleKey())
                    {
                        case Sys.ConsoleKey.Enter:
                            {
                                WriteLine("");
                                return password;
                            }
                        case Sys.ConsoleKey.Backspace:
                        case Sys.ConsoleKey.DownArrow:
                        case Sys.ConsoleKey.LeftArrow:
                        case Sys.ConsoleKey.RightArrow:
                        case Sys.ConsoleKey.UpArrow:
                            {
                                break;
                            }
                        default:
                            {
                                char keyChar = key.KeyChar;
                                if (!char.IsAscii(keyChar))
                                {
                                    continue;
                                }

                                consoleLines[0] += '*';
                                password += keyChar;
                                    
                                Sys.Console.Write('*');
                                break;
                            }

                    }
                    
                }
            }
        }

        public static void getAndHandleKey()
        {
            CosmosSys.KeyEvent key = CosmosSys.KeyboardManager.ReadKey();
            switch (key.Key.ToConsoleKey())
            {
                case Sys.ConsoleKey.Enter:
                    {
                        currentCommandIndex = -1;
                        cursorPosition = 0;
                        WriteLine("");
                        if (consoleLines[1] != null && consoleLines[1].Substring(promptLength) != "")
                        {
                            previousCommands.Insert(0, consoleLines[1].Substring(promptLength));
                            CommandExecutionHelper.executeCommand(consoleLines[1].Substring(promptLength));
                        }
                        else
                        {
                            WriteLine("You didn't say anything but pressed ENTER. WHAT? You wanna tell me I'm an old deaf man who doesn't understand your commands anymore? It's SCIENTIFICALLY PROVEN that I AM IN EXCELLENT HEALTH!!!\n");
                        }
                        break;
                    }
                case Sys.ConsoleKey.Backspace:
                    {
                        if (consoleLines[0].Length <= promptLength)
                        {
                            return;
                        }
                        string newCurrentLine = consoleLines[0].Substring(0, consoleLines[0].Length - 1 - cursorPosition);
                        if (cursorPosition != 0)
                        {
                            newCurrentLine += consoleLines[0].Substring(consoleLines[0].Length - cursorPosition);
                        }
                        consoleLines[0] = newCurrentLine;
                        reprint();
                        break;
                    }
                case Sys.ConsoleKey.UpArrow:
                    {
                        previousCommand();
                        break;
                    }
                case Sys.ConsoleKey.DownArrow:
                    {
                        nextCommand();
                        break;
                    }
                case Sys.ConsoleKey.RightArrow:
                    {
                        if (cursorPosition <= 0)
                        {
                            return;
                        }
                        cursorPosition--;
                        Sys.Console.SetCursorPosition(consoleLines[0].Length - cursorPosition, Sys.Console.GetCursorPosition().Top);
                        break;
                    }
                case Sys.ConsoleKey.LeftArrow:
                    {
                        if (cursorPosition >= consoleLines[0].Length - promptLength)
                        {
                            return;
                        }
                        cursorPosition++;
                        Sys.Console.SetCursorPosition(consoleLines[0].Length - cursorPosition, Sys.Console.GetCursorPosition().Top);
                        break;
                    }
                default:
                    {
                        char keyChar = key.KeyChar;
                        if (!char.IsAscii(keyChar))
                        {
                            return;
                        }
                        if (char.IsLower(keyChar))
                        {
                            keyChar = char.ToUpperInvariant(keyChar);
                        }
                        else if (char.IsUpper(keyChar))
                        {
                            keyChar = char.ToLowerInvariant(keyChar);
                        }
                        consoleLines[0] = consoleLines[0].Insert(consoleLines[0].Length - cursorPosition, keyChar.ToString());
                        
                        if (cursorPosition != 0)
                        { 
                            reprint();
                        }
                        else
                        {
                            Sys.Console.Write(keyChar);
                        }
                        break;
                    }
            }
        }

        public static void checkScrolling()
        {
            int scrollDiff = (int)CosmosSys.MouseManager.Y - 1000;
            if (Sys.Math.Abs(scrollDiff) >= scrollThreshold)
            {
                int maxScrollOffset = Sys.Math.Max(3, consoleLines.Count - 25);
                int minScrollOffset = 0;
                if (scrollDiff > 0 && currentScrollOffset <= minScrollOffset) // user scrolls down
                {
                    CosmosSys.MouseManager.Y = 1000;
                    currentScrollOffset = minScrollOffset;
                    return;
                }
                else if (scrollDiff < 0 && currentScrollOffset >= maxScrollOffset) // user scrolls up
                {
                    CosmosSys.MouseManager.Y = 1000;
                    currentScrollOffset = maxScrollOffset;
                    return;
                }

                currentScrollOffset += scrollDiff / -100;
                CosmosSys.MouseManager.Y = 1000;

                reprint();
                currentCommandIndex = -1;
            }
        }

        public static void reprint()
        {
            Sys.Console.Clear();
            for (int i = currentScrollOffset + 24; i > 0 && i > currentScrollOffset; i--)
            {
                if (i >= consoleLines.Count)
                {
                    Sys.Console.WriteLine("");
                    continue;
                }
                Sys.Console.WriteLine(consoleLines[i]);
            }

            Sys.Console.Write(consoleLines[0]);
            Sys.Console.SetCursorPosition(consoleLines[0].Length - cursorPosition, Sys.Console.GetCursorPosition().Top);
        }

        public static void previousCommand()
        {
            if (currentCommandIndex >= previousCommands.Count - 1)
            {
                return;
            }
            currentCommandIndex++;
            consoleLines[0] = prompt + previousCommands[currentCommandIndex];
            reprint();
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
                consoleLines[0] = "";
                reprint();
                return;
            }

            currentCommandIndex--;
            consoleLines[0] = prompt + previousCommands[currentCommandIndex];
            reprint();
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
            Sys.Threading.Thread.Sleep(2500);
            Sys.Console.Clear();
            WriteLine("\t\t^DISCLAIMER^");
            WriteLine(DonaldHimself.name);
            WriteLine("If you are an old white man: Welcome to DonaldOS! Type HELP to learn what you can do with this GREAT BEAUTIFUL SYSTEM!\n\n");
        }
        
        public static void Clear()
        {
            System.Console.Clear();
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
