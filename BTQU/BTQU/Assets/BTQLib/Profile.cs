using System.Collections.Generic;

namespace BTQ
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

        public IList<Account> Accounts
        {
            get
            {
                return _accounts;
            }
        }

        /// <summary>
        /// Constructs a new instance of Profile.
        /// </summary>
        public Profile()
        {
            Name = "";
        }

        /// <summary>
        /// Creates and account with the given ID.
        /// </summary>
        /// <param name="accountID">Account ID.</param>
        public void CreateAccount(string accountID)
        {
            if(GetAccount(accountID) != null)
            {
                return;
            }

            _accounts.Add(new Account(accountID));
        }

        /// <summary>
        /// Removes the account with the given ID from the profile.
        /// </summary>
        /// <param name="accountID">ID of account to remove.</param>
        public void RemoveAccount(string accountID)
        {
            _accounts.RemoveAll(account => account.ID == accountID);
        }

        /// <summary>
        /// Gets the account with the given ID.
        /// </summary>
        /// <param name="accountID">ID of account to get.</param>
        /// <returns>Account with the given ID or null if non-existent.</returns>
        public Account GetAccount(string accountID)
        {
            return _accounts.Find(account => account.ID == accountID);
        }
    }
}
