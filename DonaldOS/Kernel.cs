using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;

namespace DonaldOS
{
    public class Kernel : Sys.Kernel
    {

        protected override void BeforeRun()
        {
            Console.WriteLine("BOOTED");
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
