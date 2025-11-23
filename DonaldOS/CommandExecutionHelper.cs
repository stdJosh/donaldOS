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
            string[] args = input.Split(' ');
            switch (args[0])
            {
                case "touch":
                    {
                        fs.createFile(currentPath, args[1]);
                        break;
                    }
                case "ls":
                    {
                        string path = currentPath;
                        bool recursive = false;
                        FileSystemElementTypes elementTypes = FileSystemElementTypes.All;
                        string filterString = "";
                        for (int i = 1; i < args.Length; i++)
                        {
                            switch (args[i])
                            {
                                case "--path":
                                    {
                                        path = args[i + 1];
                                        i++;
                                        break;
                                    }
                                case "--recursive":
                                    {
                                        recursive = true;
                                        break;
                                    }
                                case "--dirs":
                                    {
                                        elementTypes = FileSystemElementTypes.Dirs;
                                        break;
                                    }
                                case "--files":
                                    {
                                        elementTypes = FileSystemElementTypes.Files;
                                        break;
                                    }
                                case "--filter":
                                    {
                                        filterString = args[i + 1];
                                        i++;
                                        break;
                                    }
                                default:
                                    {
                                        Console.WriteLine("ls: Unknown flag " + args[i] + "\nUse ls -h for help.");
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
                            Console.WriteLine("ls: " + e.ToString());
                        }
                        break;
                    }
                case "rm":
                    {
                        fs.remove(@"0:\" + args[1]);
                        break;
                    }
                case "test":
                    {
                        Console.reprint();
                        break;
                    }
                case "edit":
                    {
                        string path = "";

                        if (args.Length > 1)
                        {
                            path = args[1];
                        }
                        else
                        {
                            path = currentPath;
                        }

                        Editor editor = new Editor(path);

                        try
                        {

                            editor.ReadFile();

                            editor.editmode();

                        }
                        catch (Sys.Exception ex){
                            Console.WriteLine(ex.Message);
                        }
                        Console.reprint();
                        break;
                    }
                default:
                    {
                        Console.WriteLine(input);
                        break;
                    }
            }
        }
    }
}
