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

        private static UserManager um = Kernel.SharedUserManager;

        public static string currentPath = @"0:\";

        private static Dictionary<string, (Sys.Action<string[]>, string, string)> commands = new Dictionary<string, (Sys.Action<string[]> action, string possibleArgs, string helpText)>
        {
            { "HELP", (args => help(), "", "Prints this help if you aren't as intelligent as I am ... of course you aren't.") },
            { "TOUCH", (args => touch(args), "<filename>", "Creates a new file in the current working directory respectively in the specified absolute path and tries to get it through the congress - well, at least the latter is what it SHOULD do, but the developers are too dumb...") },
            { "LS", (args => ls(args), "<[flags]>", "Lists all decrees ... ehhh files. Use LS --HELP for more details.") },
            { "RM", (args => rm(args), "<filename | directoryname>", "FIRES the element instantly --- :D") },
            { "MKDIR", (args => mkdir(args), "<directoryname>", "Creates a new folder with some Epstein papers inside.") },
            { "CD", (args => cd(args), "<target directory>", "Uses my private jet to fly to your desired destination working directory. Use CD .. to go up and CD \\ to go to the root.") },
            { "COPY", (args => copy(args), "<filename>", "Copies the file to the clipboard so you can then PASTE it easily somewhere else.") },
            { "CUT", (args => cut(args), "<filename>", "Doges ehhh cuts the desired file to the clipboard so you can then PASTE it easily somewhere else.") },
            { "PASTE", ((args => paste(), "", "PASTE the clipboard into the current working directory. NO, THIS APPROACH IS NOT UNNECESSARILY COMPLICATED AT ALL!")) },
            { "MOVE", (args => move(args), "<source>, <destination>", "I like to MOVE IT MOVE IT...") },
            { "LOGIN", (args => login(args), "<username>, <password>", "Logs you in as this user, but only if the password is correct. Otherwise you will be sent to my friend Kim.") },
            { "LOGOUT", (args => logout(), "", "If you have worked hard enough for your country, you may leave.") },
            { "ADDUSER", (args => adduser(args), "<username> <password> <role>", "Adds a user with the given properties, but only if he is Republican.") },
            { "DELUSER", (args => deluser(args), "<username>", "FIRES the given user.") },
            { "LISTUSERS", (args => listusers(), "", "It's self-explanatory - I've better things to do than explain that to you...") },
            { "WHOAMI", (args => whoami(), "", "I  A M  D O N A L D  J.  T R U M P, AND IT DOES NOT MATTER WHO YOU ARE!!!") },
            { "EDIT", (args => edit(args), "<absolutePathToAFile>", "Edit the given file and SIGN IT at the end with an edding :D") },
            { "SHUTDOWN", (args => shutdown(), "", "No no no this has nothing to do with a government shutdown, it's just about this system.") },
            { "NUKE", (args => nuke(), "<destination>", "Starts a nuclear war with the given country, using our BIG, BEAUTIFUL nukes. Be careful!") }
        };
        public static void executeCommand(string input)
        {
            Console.WriteLine("");

            string[] args = input.Split(' ');

            if (!commands.ContainsKey(args[0]))
            {
                Console.WriteLine("If you've read anywhere that \"" + args[0] + "\" was a valid command, these where FAKE NEWS!!! Use HELP to get rid of these silly, silly ideas.\n");
                Console.printPrompt();
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

            if (args[0] != "LOGOUT")
            {
                Console.printPrompt();
            }
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
            if (!um.HasPermission("write"))
            {
                Console.WriteLine("YOU ARE NOT ALLOWED TO DO THIS!!!");
                return;
            }
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: TOUCH <filename>");
                return;
            }
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
                            if (i + 1 >= args.Length)
                            {
                                Console.WriteLine("LS: --PATH requires a parameter. Use LS --HELP for help");
                                return;
                            }
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
                            if (i + 1 >= args.Length)
                            {
                                Console.WriteLine("LS: --FILTER requires a parameter. Use LS --HELP for help");
                                return;
                            }
                            filterString = args[i + 1];
                            i++;
                            break;
                        }
                    case "--HELP":
                        {
                            Console.WriteLine("By default, LS lists the content of the current working directory. You can configure its behaviour by using the following optional flags:");
                            Console.WriteLine("--PATH <path>: Show the content of the specified path");
                            Console.WriteLine("--FILTER <string>: Only show elements whose name contains the filter string");
                            Console.WriteLine("--DIRS: Show directories only");
                            Console.WriteLine("--FILES: Show files only");
                            Console.WriteLine("--RECURSIVE: Resolve directories recursively");
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("LS: Unknown flag " + args[i] + "\nUse LS --HELP for help.");
                            return;
                        }
                }
            }

            // Versuche, relativen Pfad zu normalisieren (falls FileSystem.NormalizePath genutzt werden soll)
            string normalized = fs.NormalizePath(currentPath, path);
            fs.listDir(normalized, 0, recursive, elementTypes, filterString);
        }

        private static void rm(string[] args)
        {
            if (!um.HasPermission("write"))
            {
                Console.WriteLine("YOU ARE NOT ALLOWED TO DO THIS!!!");
                return;
            }
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: RM <path>");
                return;
            }

            // rm kann absolute oder relative Pfade akzeptieren
            string target = fs.NormalizePath(currentPath, args[1]);
            fs.remove(target);
        }

        private static void mkdir(string[] args)
        {
            if (!um.HasPermission("write"))
            {
                Console.WriteLine("YOU ARE NOT ALLOWED TO DO THIS!!!");
                return;
            }

            if (args.Length < 2)
            {
                Console.WriteLine("Usage: MKDIR <name>");
                return;
            }
            string name = args[1];
            string target = fs.NormalizePath(currentPath, name);
            fs.MakeDir(target);
        }

        private static void cd(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: CD <path | \\>");
                return;
            }

            string requested = args[1];

            // Root
            if (requested == @"\" || requested == "/")
            {
                currentPath = @"0:\";
                Console.WriteLine("Dir changed to " + currentPath);
                return;
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
                return;
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
                Console.WriteLine("There is no directory like: " + newPath + ", you silly idiot");
            }
        }

        private static void copy(string[] args)
        {
            if (!um.HasPermission("write"))
            {
                Console.WriteLine("YOU ARE NOT ALLOWED TO DO THIS!!!");
                return;
            }
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: COPY <file>");
                return;
            }

            string source = fs.NormalizePath(currentPath, args[1]);

            if (!source.Contains(@":\"))
                source = fs.NormalizePath(currentPath, args[1]);

            fs.CopyBufferSet(source, false);
        }

        private static void cut(string[] args)
        {
            if (!um.HasPermission("write"))
            {
                Console.WriteLine("YOU ARE NOT ALLOWED TO DO THIS!!!");
                return;
            }
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: CUT <file>");
                return;
            }

            string source = fs.NormalizePath(currentPath, args[1]);
            fs.CopyBufferSet(source, true);
        }

        private static void paste()
        {
            if (!um.HasPermission("write"))
            {
                Console.WriteLine("YOU ARE NOT ALLOWED TO DO THIS!!!");
                return;
            }
            fs.PasteIntoDir(currentPath);
        }

        private static void move(string[] args)
        {
            if (!um.HasPermission("write"))
            {
                Console.WriteLine("YOU ARE NOT ALLOWED TO DO THIS!!!");
                return;
            }
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: MOVE <file> <newname or path>");
                return;
            }

            string src = fs.NormalizePath(currentPath, args[1]);
            string dest = fs.NormalizePath(currentPath, args[2]);
            fs.MoveFile(src, dest);
        }

        private static void login(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: LOGIN <username> <password>");
                return;
            }
            if (um.Login(args[1], args[2]))
            {
                Console.WriteLine("Logged in as " + args[1]);
            }
            else
            {
                Console.WriteLine("Login failed.");
            }
        }

        private static void logout()
        {
            um.Logout();
            Console.WriteLine("Logged out. You can go and play golf now.");
        }

        private static void adduser(string[] args)
        {
            if (um.CurrentUser == null || um.CurrentUser.Role != "admin")
            {
                Console.WriteLine("YOU ARE NOT ALLOWED TO DO THIS!!! YOU HAVE TO BE AN ADMIN!!!");
                return;
            }
            if (args.Length < 4)
            {
                Console.WriteLine("Usage: ADDUSER <username> <password> <role>");
                return;
            }
            string name = args[1], pass = args[2], role = args[3];
            if (um.CreateUser(name, pass, role))
            {
                Console.WriteLine("User created: " + name);
            }
            else
            {
                Console.WriteLine("User already exists");
            }
        }

        private static void deluser(string[] args)
        {
            if (um.CurrentUser == null || um.CurrentUser.Role != "admin")
            {
                Console.WriteLine("YOU ARE NOT ALLOWED TO DO THIS!!! YOU HAVE TO BE AN ADMIN!!!");
                return;
            }
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: DELUSER <username>");
                return;
            }
            if (um.DeleteUser(args[1]))
            {
                Console.WriteLine("User deleted: " + args[1]);
            }
            else
            {
                Console.WriteLine("User not found: " + args[1]);
            }
        }

        private static void listusers()
        {
            if (um.CurrentUser == null || um.CurrentUser.Role != "admin")
            {
                Console.WriteLine("YOU ARE NOT ALLOWED TO DO THIS!!! YOU HAVE TO BE AN ADMIN!!!");
                return;
            }
            foreach (var u in um.ListUsers())
            {
                Console.WriteLine($"- {u.Username} (role: {u.Role})");
            }
        }

        private static void whoami()
        {
            if (um.CurrentUser == null)
            {
                Console.WriteLine("No user logged in. How did you even manage to accomplish this...");
            }
            else
            {
                Console.WriteLine("Current user: " + um.CurrentUser.Username + " (role: " + um.CurrentUser.Role + ")");
            }
        }

        private static void edit(string[] args)
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
            catch (Sys.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.reprint();
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
