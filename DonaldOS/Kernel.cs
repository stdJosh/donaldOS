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
        internal static UserManager SharedUserManager = new UserManager();

        CosmosSys.FileSystem.CosmosVFS vfs = new CosmosSys.FileSystem.CosmosVFS();

        protected override void BeforeRun()
        {
            
            CosmosSys.FileSystem.VFS.VFSManager.RegisterVFS(vfs);

            var um = SharedUserManager;

            if (!um.ListUsers().Any())
            {
                um.InitializeAdminInteractive();
            }

            CosmosSys.KeyboardManager.SetKeyLayout(new DE_Standard());

            CosmosSys.MouseManager.ScreenWidth = 200;
            CosmosSys.MouseManager.ScreenHeight = 2000;
            CosmosSys.MouseManager.Y = 1000;

            Console.printBootScreen();
        }

        protected override void Run()
        {
            var um = SharedUserManager;
            if (um.CurrentUser == null)
            {
                Console.WriteLine("Not even the FBI can tell me who you are. Login! NOW!");
                Console.Write("Username: ");
                string u = Console.ReadLine();
            
                Console.Write("Password: ");
                string p = Console.getPasswordFromUser();
            
                if (!um.Login(u, p))
                {
                    Console.WriteLine("ARE YOU TOO STUPID TO LOG IN??? I will give you one last chance...\n\n");
                    return;
                }
            
                Console.WriteLine("You are logged in now. Consider linking your TRUTH SOCIAL account\n\n");

                Console.printPrompt();
            }


            Sys.Threading.Thread.Sleep(5);
            if (!Sys.Console.KeyAvailable)
            {
                Console.checkScrolling();
                return;
            }

            try
            {
                Console.getAndHandleKey();
            }
            catch (Sys.Exception e)
            {
                Console.WriteLine(e.ToString());
                Sys.Threading.Thread.Sleep(5000);
            }
        }
    }
}
