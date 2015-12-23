using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTQ
{
    /// <summary>
    /// A financial account containing transactions.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Account ID (tied to institution).
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// User assigned name of the account.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name of institution account is associated with.
        /// </summary>
        public string Institution { get; set; }

        /// <summary>
        /// Transactions within the account.
        /// </summary>
        public IReadOnlyList<Transaction> Transactions
        {
            get { return _transactions; }
        }


        /// <summary>
        /// Transactions within the account.
        /// </summary>
        private List<Transaction> _transactions = new List<Transaction>();
        

        /// <summary>
        /// Constructs an Account without an ID.
        /// </summary>
        public Account()
        {
            ID = "";
        }

        /// <summary>
        /// Constructs an Account with the given ID.
        /// </summary>
        /// <param name="id">ID of account.</param>
        public Account(string id)
        {
            ID = id;
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
