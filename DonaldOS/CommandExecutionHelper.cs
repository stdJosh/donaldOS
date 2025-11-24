using Sys = System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using CosmosSys = Cosmos.System;

namespace DonaldOS
{
    internal static class CommandExecutionHelper
    {
        private static FileSystem fs = new FileSystem();

        private static string currentPath = @"0:\";

        private static Dictionary<string, (Sys.Action<string[]>, string, string)> commands = new Dictionary<string, (Sys.Action<string[]> action, string possibleArgs, string helpText)>
        {
            { "HELP", (args => help(), "", "Prints this help if you aren't as intelligent as I am ... of course you aren't.") },
            { "TOUCH", (args => touch(args), "<filename>", "Creates a new file in the current working directory respectively in the specified absolute path and tries to get it through the congress - well, at least the latter is what it SHOULD do, but the developers are too dumb...") },
            { "LS", (args => ls(args), "<[flags]>", "Lists all decrees ... ehhh files. Use LS --H for more details.") },
            { "RM", (args => rm(args), "<filename | directoryname>", "FIRES the element instantly --- :D") },
            { "SHUTDOWN", (args => shutdown(), "", "No no no this has nothing to do with a government shutdown, it's just about this system.") },
            { "NUKE", (args => nuke(), "<destination>", "Starts a nuclear war with the given country. Be careful!") }
        };
        public static void executeCommand(string input)
        {
            Console.WriteLine("");

            string[] args = input.Split(' ');

            if (!commands.ContainsKey(args[0]))
            {
                Console.WriteLine("If you've read anywhere that \"" + args[0] + "\" was a valid command, these where FAKE NEWS!!! Use HELP to get rid of these silly, silly ideas.\n");
                return;
            }

            try
            {
                commands[args[0]].Item1(args);
            }
            catch (Sys.Exception e)
            {
                Console.WriteLine("Something went wrong executing your command " + input + ": " + e.ToString()
                    + "\nSomeone gonna get fired I swear to you...");
            }

            Console.WriteLine("");
        }

        private static void help()
        {
            foreach (var (name, (_, possibleArgs, helpText)) in commands)
            {
                Console.WriteLine(name + " " + possibleArgs + " - " + helpText + "\n");
            }
        }

        private static void touch(string[] args)
        {
            fs.createFile(currentPath, args[1]);
        }

        private static void ls(string[] args)
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
                    case "--RECURSIVE":
                        {
                            recursive = true;
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
                            Console.WriteLine("LS: Unknown flag " + args[i] + "\nUse LS -H for help.");
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
        }

        private static void rm(string[] args)
        {
            fs.remove(@"0:\" + args[1]);
        }

        private static void shutdown()
        {
            CosmosSys.Power.Shutdown();
        }

        private static void nuke()
        {
            Console.WriteLine("Nahhh don't do it, I wanna and I WILL get the Nobel Peace Prize!");
        }
    }
}
