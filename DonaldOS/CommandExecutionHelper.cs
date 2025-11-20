using Sys = System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaldOS
{
    internal static class CommandExecutionHelper
    {
        private static FileSystem fs = new FileSystem();

        private static string currentPath = @"0:\";
        public static void executeCommand(string input)
        {
            Console.WriteLine("");
            string[] args = input.Split(' ');
            switch (args[0])
            {
                case "TOUCH":
                    {
                        fs.createFile(currentPath, args[1]);
                        break;
                    }
                case "LS":
                    {
                        string path = currentPath;
                        bool recursive = false;
                        FileSystemElementTypes elementTypes = FileSystemElementTypes.All;
                        string filterString = "";
                        for (int i = 1; i < args.Length; i++)
                        {
                            switch (args[i])
                            {
                                case "--PATH":
                                    {
                                        path = args[i + 1];
                                        i++;
                                        break;
                                    }
                                case "--RECURSIVE":
                                    {
                                        recursive = true;
                                        break;
                                    }
                                case "--DIRS":
                                    {
                                        elementTypes = FileSystemElementTypes.Dirs;
                                        break;
                                    }
                                case "--FILES":
                                    {
                                        elementTypes = FileSystemElementTypes.Files;
                                        break;
                                    }
                                case "--FILTER":
                                    {
                                        filterString = args[i + 1];
                                        i++;
                                        break;
                                    }
                                default:
                                    {
                                        Console.WriteLine("LS: Unknown flag " + args[i] + "\nUse ls -h for help.");
                                        return;
                                    }
                            }
                        }
                        try
                        {
                            fs.listDir(path, 0, recursive, elementTypes, filterString);
                        }
                        catch (Sys.Exception e)
                        {
                            Console.WriteLine("LS: " + e.ToString());
                        }
                        break;
                    }
                case "RM":
                    {
                        fs.remove(@"0:\" + args[1]);
                        break;
                    }
                default:
                    {
                        Console.WriteLine("If you've read anywhere that \"" + args[0] + "\" was a valid command, these where FAKE NEWS!!! Use HELP to get rid of these silly, silly ideas.");
                        break;
                    }
            }

            Console.WriteLine("");
        }
    }
}
