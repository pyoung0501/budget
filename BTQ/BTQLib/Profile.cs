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
        public IList<Account> Accounts
        {
            get
            {
                return _accounts;
            }
        }

        private BudgetCategories _budgetCategories = new BudgetCategories();

        public BudgetCategories BudgetCategories { get { return _budgetCategories; } }
        

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

    public class BudgetCategories
    {
        private List<string> _fullyQualifiedCategories;

        private List<string> _primaryCategories = new List<string>();

        public List<string> PrimaryCategories { get { return _primaryCategories; } }

        private List<List<string>> _secondaryCategories = new List<List<string>>();

        public List<List<string>> SecondaryCategories { get { return _secondaryCategories; } }

        public List<string> GetPrimaryCategories()
        {
            return _primaryCategories;
        }

        public List<string> GetSecondaryCategories(string primaryCategory)
        {
            int index = _primaryCategories.BinarySearch(primaryCategory);
            if(index >= 0)
            {
                return _secondaryCategories[index];
            }

            return null;
        }

        public void AddPrimaryCategory(string category)
        {
            int index = _primaryCategories.BinarySearch(category);
            if(index >= 0)
            {
                return;
            }

            _primaryCategories.Insert(~index, category);
            _secondaryCategories.Insert(~index, new List<string>());
        }

        public bool PrimaryCategoryExists(string category)
        {
            return _primaryCategories.BinarySearch(category) >= 0;
        }

        public void AddSecondaryCategory(string primaryCategory, string secondaryCategory)
        {
            if(!PrimaryCategoryExists(primaryCategory))
            {
                AddPrimaryCategory(primaryCategory);
            }

            int primaryIndex = _primaryCategories.BinarySearch(primaryCategory);
            int secondaryIndex = _secondaryCategories[primaryIndex].BinarySearch(secondaryCategory);
            if(secondaryIndex >= 0)
            {
                return;
            }

            _secondaryCategories[primaryIndex].Insert(~secondaryIndex, secondaryCategory);
        }

        public bool SecondaryCategoryExists(string primaryCategory, string secondaryCategory)
        {
            int primaryIndex = _primaryCategories.BinarySearch(primaryCategory);
            if (primaryIndex >= 0)
            {
                int secondaryIndex = _secondaryCategories[primaryIndex].BinarySearch(secondaryCategory);
                return secondaryIndex >= 0;
            }

            return false;
        }
    }
}
