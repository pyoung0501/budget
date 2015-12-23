using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTQ
{
    class Program
    {
        private static List<Profile> _profiles = new List<Profile>();

        static void Main(string[] args)
        {
            bool done = false;

            do
            {
                Console.Clear();

                Console.WriteLine("--- Commands ----------");
                Console.WriteLine("  create [profile|account]");
                Console.WriteLine("  list [profiles|accounts]");
                Console.WriteLine("  quit");
                Console.WriteLine("-----------------------");

                string command = Console.ReadLine();
                done = ProcessCommand(command);

            } while (!done);
        }

        private static bool ProcessCommand(string command)
        {
            if(command.ToLower() == "q" || command.ToLower() == "quit")
            {
                return true;
            }

            List<string> commandArgs = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if(commandArgs.Count == 0)
            {
                return false;
            }

            string commandName = commandArgs[0];
            List<string> parameters = commandArgs.GetRange(1, commandArgs.Count - 1);

            if (commandName == "create")
            {
                CreateObject(parameters);
            }

            if(commandName == "list")
            {
                ListObject(parameters);
            }

            return false;
        }

        private static void ListObject(List<string> parameters)
        {
            if (parameters.Count == 0)
                return;

            if(parameters[0] == "profiles")
            {
                Console.Clear();
                
                if(_profiles.Count == 0)
                {
                    Console.WriteLine("<No profiles have been created>");
                }
                else
                {
                    foreach(Profile profile in _profiles)
                    {
                        Console.WriteLine("{0}: {1}", _profiles.IndexOf(profile), profile.Name);
                    }
                }

                Console.Write("Press any key to continue...");
                Console.ReadLine();
            }
            else if(parameters[0] == "accounts")
            {
                Console.Clear();

                if (_profiles.Count == 0)
                {
                    Console.WriteLine("<No profiles have been created>");
                }
                else
                {
                    foreach (Profile profile in _profiles)
                    {
                        Console.WriteLine("{0}: {1}", _profiles.IndexOf(profile), profile.Name);

                        if(profile.Accounts.Count == 0)
                        {
                            Console.WriteLine("  <No accounts have been created in this profile>");
                        }
                        else
                        {
                            foreach(Account account in profile.Accounts)
                            {
                                Console.WriteLine("  {0}\t{1}\t{2}", account.Name, account.Institution, account.ID);
                            }
                        }
                    }
                }

                Console.Write("Press any key to continue...");
                Console.ReadLine();
            }
        }

        private static void CreateObject(List<string> parameters)
        {
            if (parameters.Count == 0)
                return;

            if(parameters[0] == "profile")
            {
                Console.Write("Enter Name: ");
                string profileName = Console.ReadLine();

                Profile profile = new Profile() { Name = profileName };
                _profiles.Add(profile);
            }
            else if(parameters[0] == "account")
            {
                int selectedProfileIndex = DrawProfileSelection();

                if (selectedProfileIndex >= 0 && selectedProfileIndex < _profiles.Count)
                {
                    Profile selectedProfile = _profiles[selectedProfileIndex];

                    Console.Clear();
                    Console.WriteLine(selectedProfile.Name);

                    string name;
                    string institution;
                    string id;

                    DrawFieldInput("Name", out name);
                    DrawFieldInput("Institution", out institution);
                    DrawFieldInput("ID", out id);

                    selectedProfile.CreateAccount(id);
                    Account account = selectedProfile.GetAccount(id);
                    account.Name = name;
                    account.Institution = institution;
                }
            }
        }

        private static int DrawProfileSelection()
        {
            int selection = -1;

            Console.WriteLine("Select Profile:");
            foreach (Profile profile in _profiles)
            {
                Console.WriteLine("  {0}: {1}", _profiles.IndexOf(profile), profile.Name);
            }
            Console.WriteLine(":");

            string selectionText = Console.ReadLine();
            int.TryParse(selectionText, out selection);

            return selection;
        }

        private static void DrawFieldInput(string label, out string field)
        {
            Console.Write(label + ": ");
            field = Console.ReadLine();
        }
    }
}
