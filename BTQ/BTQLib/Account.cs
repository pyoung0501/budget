using System.Collections.Generic;


namespace BTQLib
{
    /// <summary>
    /// A financial account containing transactions.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// User assigned name of the account.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Account Number (tied to institution).
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Name of institution account is associated with.
        /// </summary>
        public string Institution { get; set; }

        /// <summary>
        /// The balance of the account prior to any of the
        /// transactions.
        /// </summary>
        public decimal StartingBalance { get; set; }

        /// <summary>
        /// The distribution of the starting balance to budget categories.
        /// </summary>
        public Dictionary<string, decimal> StartingDistribution { get { return _startingDistribution; } }

        /// <summary>
        /// Transactions within the account.
        /// </summary>
        public IList<Transaction> Transactions
        {
            get { return _transactions; }
        }


        /// <summary>
        /// The distribution of the starting balance to budget categories.
        /// </summary>
        private Dictionary<string, decimal> _startingDistribution = new Dictionary<string, decimal>();

        /// <summary>
        /// Transactions within the account.
        /// </summary>
        private List<Transaction> _transactions = new List<Transaction>();


        /// <summary>
        /// Constructs a blank Account.
        /// </summary>
        public Account()
        {
            Name = "";
            Number = "";
            Institution = "";
        }

        /// <summary>
        /// Constructs an Account with the given name.
        /// </summary>
        /// <param name="name">Name of account.</param>
        public Account(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Adds the given transaction to the account.
        /// </summary>
        /// <param name="transaction">Transaction.</param>
        public void AddTransaction(Transaction transaction)
        {
            _transactions.Add(transaction);
        }
    }
}
