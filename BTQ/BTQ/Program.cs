using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
                Console.WriteLine("  create [profile|account|transaction]");
                Console.WriteLine("  list [profiles|accounts|transactions]");
                Console.WriteLine("  save");
                Console.WriteLine("  load");
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

            if(commandName == "save")
            {
                SaveProfiles();
            }

            if(commandName == "load")
            {
                LoadProfiles();
            }

            return false;
        }

        private static void SaveProfiles()
        {
            using (StreamWriter stream = new StreamWriter("Profiles"))
            {
                stream.Write(JsonConvert.SerializeObject(_profiles, Formatting.Indented));
            }
        }

        private static void LoadProfiles()
        {
            using (StreamReader stream = new StreamReader("Profiles"))
            {
                _profiles = JsonConvert.DeserializeObject(stream.ReadToEnd(), typeof(List<Profile>)) as List<Profile>;
            }
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
            else if(parameters[0] == "transactions")
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

                        if (profile.Accounts.Count == 0)
                        {
                            Console.WriteLine("  <No accounts have been created in this profile>");
                        }
                        else
                        {
                            foreach (Account account in profile.Accounts)
                            {
                                Console.WriteLine("  {0}\t{1}\t{2}", account.Name, account.Institution, account.ID);

                                if(account.Transactions.Count == 0)
                                {
                                    Console.WriteLine("    <No transactions have been created in this account>");
                                }
                                else
                                {
                                    foreach(Transaction transaction in account.Transactions)
                                    {
                                        Console.WriteLine("    {0}\t{1}\t{2}\t{3}\t{4}",
                                            transaction.Date, transaction.Payee, transaction.Description, transaction.Category, transaction.Amount);
                                    }
                                }
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
            else if(parameters[0] == "transaction")
            {
                int selectedProfileIndex = DrawProfileSelection();
                if (selectedProfileIndex >= 0 && selectedProfileIndex < _profiles.Count)
                {
                    Profile selectedProfile = _profiles[selectedProfileIndex];

                    Console.Clear();
                    Console.WriteLine(selectedProfile.Name);

                    int selectedAccountIndex = DrawAccountSelection(selectedProfile);
                    if(selectedAccountIndex >= 0 && selectedAccountIndex < selectedProfile.Accounts.Count)
                    {
                        Account selectedAccount = selectedProfile.Accounts[selectedAccountIndex];

                        Console.Clear();
                        Console.WriteLine(selectedProfile.Name);
                        Console.WriteLine(selectedAccount.Name);

                        string payee;
                        string description;
                        decimal amount;
                        string category;
                        DateTime date;

                        DrawFieldInput("Payee", out payee);
                        DrawFieldInput("Description", out description);
                        DrawFieldInput("Amount", out amount);
                        DrawFieldInput("Category", out category);
                        DrawFieldInput("Date", out date);

                        Transaction transaction = new Transaction()
                        {
                            Payee = payee,
                            Description = description,
                            Amount = amount,
                            Category = category,
                            Date = date
                        };

                        selectedAccount.AddTransaction(transaction);
                    }
                }
            }
        }

        private static int DrawAccountSelection(Profile selectedProfile)
        {
            int selection = -1;

            Console.WriteLine("Select Account:");
            for(int i = 0; i < selectedProfile.Accounts.Count; ++i)
            {
                Account account = selectedProfile.Accounts[i];
                Console.WriteLine("  {0}: {1}", i, account.Name);
            }
            Console.WriteLine(":");

            string selectionText = Console.ReadLine();
            int.TryParse(selectionText, out selection);

            return selection;
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

        private static void DrawFieldInput(string label, out decimal amount)
        {
            string input;
            DrawFieldInput(label, out input);
            
            decimal.TryParse(input, out amount);
        }

        private static void DrawFieldInput(string label, out DateTime date)
        {
            string input;
            DrawFieldInput(label, out input);
            
            DateTime.TryParse(input, out date);
        }
    }
}
