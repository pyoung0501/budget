using System.Collections.Generic;


namespace BTQLib
{
    /// <summary>
    /// Profile associated with a user.  A profile will contain or be
    /// linked to accounts.
    /// </summary>
    public class Profile
    {
        /// <summary>
        /// Name of the profile.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Accounts contained in the profile.
        /// </summary>
        private List<Account> _accounts = new List<Account>();

        /// <summary>
        /// Accounts contained in the profile.
        /// </summary>
        public IList<Account> Accounts { get { return _accounts; } }

        /// <summary>
        /// Object managing the transaction categories.
        /// </summary>
        private Categories _categories = new Categories();

        /// <summary>
        /// Object managing the transaction categories.
        /// </summary>
        public Categories Categories { get { return _categories; } }
        

        /// <summary>
        /// Constructs a new instance of Profile.
        /// </summary>
        public Profile()
        {
            Name = "";
        }

        /// <summary>
        /// Creates an account with the given name.
        /// </summary>
        /// <param name="accountName">Account name.</param>
        public void CreateAccount(string accountName)
        {
            if(GetAccount(accountName) != null)
            {
                return;
            }

            _accounts.Add(new Account(accountName));
        }

        /// <summary>
        /// Removes the account with the given name from the profile.
        /// </summary>
        /// <param name="accountName">Name of account to remove.</param>
        public void RemoveAccount(string accountName)
        {
            _accounts.RemoveAll(account => account.Name == accountName);
        }

        /// <summary>
        /// Gets the account with the given name.
        /// </summary>
        /// <param name="accountName">Name of account to get.</param>
        /// <returns>Account with the given name or null if non-existent.</returns>
        public Account GetAccount(string accountName)
        {
            return _accounts.Find(account => account.Name == accountName);
        }
    }
}
