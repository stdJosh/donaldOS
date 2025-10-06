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
            if (!File.Exists(configFileName))
            {
                File.Create(configFileName);
            }

            StreamReader configFile = File.OpenText(configFileName);

            if (new FileInfo(configFileName).Length == 0)
            {
                Console.WriteLine("File empty");
            }
        }
    }
}
