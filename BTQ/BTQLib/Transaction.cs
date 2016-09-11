using System;
using System.Collections.Generic;

namespace BTQLib
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
        public decimal Amount { get; set; }

        /// <summary>
        /// Category type (for budgeting).
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Date of the transaction.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// For income transactions this indicates how the transaction is
        /// applied to the budget categories.
        /// </summary>
        public AppliedState AppliedState { get; set; }

        /// <summary>
        /// For income transactions applied to a specific category this
        /// indicates the category the income transaction applies to.
        /// </summary>
        public string AppliedToCategory { get; set; }

        /// <summary>
        /// List of split transaction entries.  If split entries exist,
        /// the Category field will be ignored.  The individial category entries
        /// will be used instead.
        /// </summary>
        public List<SplitEntry> SplitEntries { get; set; }

        /// <summary>
        /// Additional data for the transaction if it was imported.
        /// Manually input transactions should not have any import data.
        /// </summary>
        public ImportData ImportData { get; set; }
        
        /// <summary>
        /// True if the transaction is split across categories; otherwise, false.
        /// </summary>
        public bool IsSplit { get { return SplitEntries != null && SplitEntries.Count > 0; } }


        /// <summary>
        /// Constructor.
        /// </summary>
        public Transaction()
        {
            Payee = "";
            Description = "";
            Category = "";
            AppliedToCategory = "";
        }
    }

    /// <summary>
    /// Additional data for an imported transaction.
    /// </summary>
    public class ImportData
    {
        /// <summary>
        /// The type of the transaction (as defined by the institution
        /// the transaction was imported from).
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Date of the transaction.  This will possibly be unset if the
        /// institution the transaction was imported from does not specify this.
        /// </summary>
        public DateTime? TransactionDate { get; set; }

        /// <summary>
        /// Date the transaction was posted to the account.  This should always
        /// be set.
        /// </summary>
        public DateTime PostDate { get; set; }

        /// <summary>
        /// The original description of the imported transaction.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The check number (for checks) or the slip number (for deposits)
        /// of the transaction.  This may or may not be set.
        /// </summary>
        public int? CheckOrSlipNo { get; set; }
    }

    /// <summary>
    /// A single split entry within a transaction.
    /// </summary>
    public class SplitEntry
    {
        /// <summary>
        /// Description of the split entry.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Category to assign entry to.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Amount of the entry.
        /// </summary>
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// The applied states of an income transaction.
    /// </summary>
    public enum AppliedState
    {
        NotApplied,
        ApplyToWhole,
        ApplyToCategory
    }
}
