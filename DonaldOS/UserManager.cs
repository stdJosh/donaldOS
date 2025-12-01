using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DonaldOS
{

    internal class User
    {
        public string Username { get; }
        public string Password { get; private set; }
        public string Role { get; private set; }

        public User(string username, string password, string role)
        {
            Username = username;
            Password = password;
            Role = role;
        }

        public override string ToString()
        {
            return $"{Username};{Password};{Role}";
        }

    }

    internal class UserManager
    {
        private const string UserFile = @"0:\SYSTEM\USERS.CONFIG";
        private List<User> users = new List<User>();

        public User CurrentUser { get; private set; }

        public bool HasPermission(string requiredRole)
        {
            if (CurrentUser == null)
                return false;

            // Admin darf alles
            if (CurrentUser.Role.Equals("admin", StringComparison.OrdinalIgnoreCase))
                return true;

            // Write darf lesen & schreiben, aber nicht admin-sachen
            if (requiredRole == "write")
                return CurrentUser.Role.Equals("write", StringComparison.OrdinalIgnoreCase);

            // Read darf nur lesen
            if (requiredRole == "read")
                return CurrentUser.Role.Equals("read", StringComparison.OrdinalIgnoreCase)
                    || CurrentUser.Role.Equals("write", StringComparison.OrdinalIgnoreCase);

            return false;
        }


        public UserManager()
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            users.Clear();
            try
            {
                if (!File.Exists(UserFile))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(UserFile));
                    File.WriteAllText(UserFile, "");
                    return;
                }

                using (var fs = File.Open(UserFile, FileMode.Open, FileAccess.Read))
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        var parts = line.Split(';');
                        if (parts.Length != 3) continue;
                        string u = parts[0];
                        string p = parts[1];
                        string r = parts[2];
                        users.Add(new User(u, p, r));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("UserManager: Fehler beim Laden: " + e.Message);
            }
        }

        private void SaveUsers()
        {
            try
            {
                using (var fs = File.Open(UserFile, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    foreach (var u in users)
                    {
                        sw.WriteLine(u.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("UserManager: Fehler beim Speichern: " + e.Message);
            }
        }

        public bool CreateUser(string username, string password, string role)
        {
            if (users.Exists(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                return false;
            users.Add(new User(username, password, role));
            SaveUsers();
            return true;
        }

        public bool DeleteUser(string username)
        {
            var u = users.Find(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (u == null) return false;
            users.Remove(u);
            SaveUsers();
            return true;
        }

        public bool Login(string username, string password)
        {
            var u = users.Find(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (u == null) return false;
            if (u.Password == password) // **Einfach**es Passwortvergleich; später kann Hash kommen
            {
                CurrentUser = u;
                return true;
            }
            return false;
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public IEnumerable<User> ListUsers()
        {
            return users;
        }

        public void InitializeAdminInteractive()
        {
            Console.WriteLine("Keine Benutzer gefunden – Initialen Admin anlegen.");
            Console.Write("Neuer Admin-Benutzername: ");
            string username = Console.ReadLine();

            Console.Write("Passwort festlegen: ");
            string password = Console.ReadLine();

            users.Add(new User(username, password, "admin"));
            SaveUsers();

            Console.WriteLine("Initialer Admin wurde erstellt.");
        }

    }
}
