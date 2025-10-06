using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;

namespace DonaldOS
{
    public class Kernel : Sys.Kernel
    {
        private CosmosVFS vfs;
        protected override void BeforeRun()
        {
            Console.WriteLine("BOOTED");

            vfs = new CosmosVFS();
            Cosmos.System.FileSystem.VFS.VFSManager.RegisterVFS(vfs);

            UserAdministration userAdministration = new UserAdministration();
            userAdministration.userLogin();
        }

        protected override void Run()
        {
            Console.Write("Input: ");
            var input = Console.ReadLine();
            Console.Write("Text typed: ");
            Console.WriteLine(input);
        }
    }
}
