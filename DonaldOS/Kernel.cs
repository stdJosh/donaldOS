using Cosmos.Core.IOGroup;
using Cosmos.System.ScanMaps;
using Sys = System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CosmosSys = Cosmos.System;

namespace DonaldOS
{
    public class Kernel : CosmosSys.Kernel
    {
        CosmosSys.FileSystem.CosmosVFS vfs = new CosmosSys.FileSystem.CosmosVFS();


        protected override void BeforeRun()
        {
            
            CosmosSys.FileSystem.VFS.VFSManager.RegisterVFS(vfs);

            
            var um = new UserManager();

            
            if (!um.ListUsers().Any())
            {
                um.CreateUser("admin", "admin", "admin");
                Console.WriteLine("Default admin user 'admin' erstellt (Passwort: admin).");
            }

            
            CosmosSys.KeyboardManager.SetKeyLayout(new DE_Standard());

            CosmosSys.MouseManager.ScreenWidth = 200;
            CosmosSys.MouseManager.ScreenHeight = 2000;
            CosmosSys.MouseManager.Y = 1000;

            
            Console.printBootScreen();
        }

        protected override void Run()
        {
            Sys.Threading.Thread.Sleep(5);
            if (!Sys.Console.KeyAvailable)
            {
                Console.checkScrolling();
                return;
            }

            var pressedKey = Console.PeekKey();

            string input;

            if (pressedKey.Key == Sys.ConsoleKey.UpArrow)
            {
                Console.previousCommand();
                return;
            }
            else if (pressedKey.Key == Sys.ConsoleKey.DownArrow)
            {
                Console.nextCommand();
                return;
            }
            else if (pressedKey.Key == Sys.ConsoleKey.Enter)
            {
                input = Console.getCurrentCommmand();
                Console.WriteLine("");
                if (input == null)
                {
                    return;
                }
            }
            else
            {
                input = Console.ReadLine();
            }
            CommandExecutionHelper.executeCommand(input);
        }
    }
}
