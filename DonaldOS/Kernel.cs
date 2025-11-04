using Cosmos.Core.IOGroup;
using Cosmos.System.ScanMaps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sys = Cosmos.System;

namespace DonaldOS
{
    public class Kernel : Sys.Kernel
    {
        private FileSystem fs = new FileSystem();
        Sys.FileSystem.CosmosVFS vfs = new Cosmos.System.FileSystem.CosmosVFS();

        private string currentPath = @"0:\";
        protected override void BeforeRun()
        {
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(vfs);
            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");
            Sys.KeyboardManager.SetKeyLayout(new DE_Standard());
        }

        protected override void Run()
        {
            //Console.Write("Input: ");
            var input = Console.ReadLine();
            
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
                        catch (Exception e)
                        {
                            Console.WriteLine("ls: " + e.ToString());
                        }
                        break;
                    }
                case "rm":
                    {
                        fs.remove(@"0:\" +  args[1]);
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
