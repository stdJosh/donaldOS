using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = System;

namespace DonaldOS
{
    internal static class CommandExecutionHelper
    {
        private static FileSystem fs = new FileSystem();
        private static string currentPath = @"0:\";

        public static void executeCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }

            // Sauber splitten (leere Einträge entfernen)
            string[] args = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 0) return;

            string cmd = args[0].ToLowerInvariant();

            try
            {
                switch (cmd)
                {
                    case "touch":
                        {
                            if (args.Length < 2)
                            {
                                Console.WriteLine("Usage: touch <filename>");
                                break;
                            }
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
                                            if (i + 1 >= args.Length)
                                            {
                                                Console.WriteLine("ls: --path requires a parameter");
                                                return;
                                            }
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
                                            if (i + 1 >= args.Length)
                                            {
                                                Console.WriteLine("ls: --filter requires a parameter");
                                                return;
                                            }
                                            filterString = args[i + 1];
                                            i++;
                                            break;
                                        }
                                    default:
                                        {
                                            Console.WriteLine("ls: Unknown flag " + args[i] + "\nUse ls --path <path> --recursive --dirs --files --filter <text>");
                                            return;
                                        }
                                }
                            }

                            // Versuche, relativen Pfad zu normalisieren (falls FileSystem.NormalizePath genutzt werden soll)
                            string normalized = fs.NormalizePath(currentPath, path);
                            fs.listDir(normalized, 0, recursive, elementTypes, filterString);
                            break;
                        }

                    case "rm":
                        {
                            if (args.Length < 2)
                            {
                                Console.WriteLine("Usage: rm <path>");
                                break;
                            }

                            // rm kann absolute oder relative Pfade akzeptieren
                            string target = fs.NormalizePath(currentPath, args[1]);
                            fs.remove(target);
                            break;
                        }

                    case "test":
                        {
                            Console.reprint();
                            break;
                        }

                    case "mkdir":
                        {
                            if (args.Length < 2)
                            {
                                Console.WriteLine("Usage: mkdir <name>");
                                break;
                            }

                            string name = args[1];
                            string target = fs.NormalizePath(currentPath, name);
                            fs.MakeDir(target);
                            break;
                        }

                    case "cd":
                        {
                            if (args.Length < 2)
                            {
                                Console.WriteLine("Usage: cd <path>");
                                break;
                            }

                            string requested = args[1];

                            // Root
                            if (requested == @"\" || requested == "/")
                            {
                                currentPath = @"0:\";
                                Console.WriteLine("Dir changed to " + currentPath);
                                break;
                            }

                            // up one
                            if (requested == "..")
                            {
                                // normalize currentPath to have no trailing slash (except root)
                                string cp = currentPath;
                                if (cp.EndsWith("\\") && cp.Length > 3) cp = cp.TrimEnd('\\');
                                int last = cp.LastIndexOf('\\');
                                if (last > 2)
                                {
                                    currentPath = cp.Substring(0, last) + "\\";
                                }
                                else
                                {
                                    currentPath = @"0:\";
                                }
                                Console.WriteLine("Dir changed to " + currentPath);
                                break;
                            }

                            // Normaler Wechsel (relativ oder absolut)
                            string newPath = fs.NormalizePath(currentPath, requested);

                            // Ensure trailing slash for directory representation
                            if (!newPath.EndsWith("\\") && fs.DirectoryExists(newPath))
                                newPath = newPath + "\\";

                            if (fs.DirectoryExists(newPath))
                            {
                                currentPath = newPath;
                                Console.WriteLine("Dir changed to " + currentPath);
                            }
                            else
                            {
                                Console.WriteLine("Directory not found: " + newPath);
                            }
                            break;
                        }

                    case "copy":
                        {
                            if (args.Length < 2)
                            {
                                Console.WriteLine("Usage: copy <file>");
                                break;
                            }

                            string source = fs.NormalizePath(currentPath, args[1]);

                            if (!source.Contains(@":\"))
                                source = fs.NormalizePath(currentPath, args[1]);

                            fs.CopyBufferSet(source, false);
                            break;
                        }

                    case "cut":
                        {
                            if (args.Length < 2)
                            {
                                Console.WriteLine("Usage: cut <file>");
                                break;
                            }

                            string source = fs.NormalizePath(currentPath, args[1]);
                            fs.CopyBufferSet(source, true);
                            break;
                        }

                    case "paste":
                        {
                            fs.PasteIntoDir(currentPath);
                            break;
                        }

                    case "move":
                        {
                            if (args.Length < 3)
                            {
                                Console.WriteLine("Usage: move <file> <newname or path>");
                                break;
                            }

                            string src = fs.NormalizePath(currentPath, args[1]);
                            string dest = fs.NormalizePath(currentPath, args[2]);
                            fs.MoveFile(src, dest);
                            break;
                        }

                    default:
                        {
                            Console.WriteLine(input);
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                // Falls doch mal etwas unvorhergesehenes passiert, nicht abstürzen lassen
                Console.WriteLine("Command error: " + e.Message);
            }
        }
    }
}
