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
                Console.WriteLine("Bitte einloggen, bevor Sie das System nutzen.");
                Console.Write("Username: ");
                string u = Console.ReadLine(); // TODO: dafür ne eigene Methode in Console schreiben, wo das Passwort unkenntlich gemacht wird
            
                Console.Write("Passwort: ");
                string p = Console.getPasswordFromUser();
            
                if (!um.Login(u, p))
                {
                    Console.WriteLine("Login fehlgeschlagen.");
                    return;
                }
            
                Console.WriteLine("Erfolgreich eingeloggt.");

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
                Sys.Threading.Thread.Sleep(10000);
            }
        }
    }
}
