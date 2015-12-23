using money;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTQ
{
    /// <summary>
    /// Transaction within an account.
    /// TODO: Check terminology of all these types.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Entity the amount was paid to.
        /// </summary>
        public string Payee { get; set; }

        /// <summary>
        /// Description of the transaction.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Amount of the transaction.
        /// </summary>
        public Money Amount { get; set; }

        /// <summary>
        /// Category type (for budgeting).
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Date of the transaction.
        /// </summary>
        public DateTime Date { get; set; }
    }
}
