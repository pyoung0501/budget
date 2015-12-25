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

        private static Profile _currProfile;

        private static Account _currAccount;
        
        private static List<Transaction> _sortedTransactions;

        static void Main(string[] args)
        {
            bool done = false;

            do
            {
                Console.Clear();

                if(_currProfile == null)
                {
                    Console.WriteLine("--- Commands ----------");
                    Console.WriteLine("  create profile");
                    Console.WriteLine("  select profile");
                    Console.WriteLine("  save/load/quit");
                    Console.WriteLine("-----------------------");
                }
                else
                {
                    Console.WriteLine("+++++  {0} Profile  +++++", _currProfile.Name);

                    if(_currAccount == null)
                    {
                        Console.WriteLine("--- Commands ----------");
                        Console.WriteLine("  create account");
                        Console.WriteLine("  select account");
                        Console.WriteLine("  back/save/load/quit");
                        Console.WriteLine("-----------------------");
                    }
                    else
                    {
                        Console.WriteLine("+++++  {0} Account  +++++", _currAccount.Name);

                        if (_sortedTransactions == null)
                        {
                            Console.WriteLine("--- Commands ----------");
                            Console.WriteLine("  create transaction");
                            Console.WriteLine("  view transactions");
                            Console.WriteLine("  back/save/load/quit");
                            Console.WriteLine("-----------------------");
                        }
                        else
                        {
                            if(_sortedTransactions.Count == 0)
                            {
                                Console.WriteLine("  <No transactions>");
                            }
                            else
                            {
                                for (int iTrans = 0; iTrans < _sortedTransactions.Count; iTrans++)
                                {
                                    DrawTransaction(_sortedTransactions[iTrans]);
                                }
                            }

                            Console.WriteLine("--- Commands ----------");
                            Console.WriteLine("  sort [Field] [+|-]");
                            Console.WriteLine("  clear sort");
                            Console.WriteLine("  back/save/load/quit");
                            Console.WriteLine("-----------------------");
                        }
                    }
                }

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

            if (commandName == "select")
            {
                SelectObject(parameters);
            }

            if (commandName == "view")
            {
                _sortedTransactions = new List<Transaction>(_currAccount.Transactions);
            }

            if(commandName == "clear")
            {
                _sortedTransactions = null;
            }

            if(commandName == "sort")
            {
                SortTransactions(parameters);
            }

            if(commandName == "save")
            {
                SaveProfiles();
            }

            if(commandName == "load")
            {
                LoadProfiles();
            }

            if(commandName == "back")
            {
                if(_sortedTransactions != null)
                {
                    _sortedTransactions = null;
                }
                else if(_currAccount != null)
                {
                    _currAccount = null;
                }
                else if(_currProfile != null)
                {
                    _currProfile = null;
                }
            }

            return false;
        }

        private static void SortTransactions(List<string> parameters)
        {
            if(parameters.Count == 0)
            {
                return;
            }

            bool sortPayee = parameters[0].ToLower() == "payee";
            bool sortDescription = parameters[0].ToLower() == "description";
            bool sortAmount = parameters[0].ToLower() == "amount";
            bool sortCategory = parameters[0].ToLower() == "category";
            bool sortDate = parameters[0].ToLower() == "date";

            bool ascending = (parameters.Count == 0) || (parameters.Count > 1 && parameters[1] == "+");

            if (sortPayee)
            {
                _sortedTransactions.Sort((lhs, rhs) => { return lhs.Payee.CompareTo(rhs.Payee); });
            }
            else if (sortDescription)
            {
                _sortedTransactions.Sort((lhs, rhs) => { return lhs.Description.CompareTo(rhs.Description); });
            }
            else if (sortAmount)
            {
                _sortedTransactions.Sort((lhs, rhs) => { return lhs.Amount.CompareTo(rhs.Amount); });
            }
            else if (sortCategory)
            {
                _sortedTransactions.Sort((lhs, rhs) => { return lhs.Category.CompareTo(rhs.Category); });
            }
            else if (sortDate)
            {
                _sortedTransactions.Sort((lhs, rhs) => { return lhs.Date.CompareTo(rhs.Date); });
            }

            if(!ascending)
            {
                _sortedTransactions.Reverse();
            }
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
                Console.Clear();
                Console.WriteLine(_currProfile.Name);

                string name;
                string institution;
                string id;

                DrawFieldInput("Name", out name);
                DrawFieldInput("Institution", out institution);
                DrawFieldInput("ID", out id);

                _currProfile.CreateAccount(id);
                Account account = _currProfile.GetAccount(id);
                account.Name = name;
                account.Institution = institution;
            }
            else if(parameters[0] == "transaction")
            {
                Console.Clear();
                Console.WriteLine(_currProfile.Name);
                Console.WriteLine(_currAccount.Name);

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

                _currAccount.AddTransaction(transaction);
            }
        }

        private static void SelectObject(List<string> parameters)
        {
            if(parameters.Count == 0)
            {
                return;
            }

            if(parameters[0] == "profile")
            {
                Console.Clear();

                int profileIndex = DrawProfileSelection();
                if (profileIndex >= 0 && profileIndex < _profiles.Count)
                {
                    _currProfile = _profiles[profileIndex];
                }
            }
            else if(parameters[0] == "account")
            {
                Console.Clear();

                int accountIndex = DrawAccountSelection(_currProfile);
                if(accountIndex >= 0 && accountIndex < _currProfile.Accounts.Count)
                {
                    _currAccount = _currProfile.Accounts[accountIndex];
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
            Console.Write(":");

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

        private static void DrawTransaction(Transaction transaction)
        {
            Console.WriteLine("  {0}\t{1}\t{2}\t{3}\t{4}",
                transaction.Date, transaction.Payee, transaction.Description,
                transaction.Category, transaction.Amount);
        }
    }
}
