using System;
using System.Collections.Generic;
using System.Linq;

namespace BTQ
{
    /*
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
                Console.WriteLine("  create profile [id]");
                Console.WriteLine("  list profiles");
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
        }
    }
    */
}
