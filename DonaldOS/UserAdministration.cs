using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace DonaldOS
{
    internal class UserAdministration
    {
        const string configFileName = "userConfig.txt";

        public void userLogin()
        {
            if (!File.Exists(configFileName) || new FileInfo(configFileName).Length == 0)
            {
                Console.WriteLine("There are no users specified. You can now add a root user. \nChoose a username: ");
                string newUserName = Console.ReadLine();
                Console.WriteLine("Choose a password: ");
                string newPassword = Console.ReadLine();

                try
                {
                    using (StreamWriter writer = new StreamWriter(configFileName))
                    {
                        writer.WriteLine(newUserName + " " + newPassword);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Console.WriteLine("Successfully created root user. You are now logged in as " + newUserName);
            }
        }
    }
}
