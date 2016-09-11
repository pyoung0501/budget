using System.Linq;

namespace BTQLib.Util
{
    /// <summary>
    /// Utilities for transactions.
    /// </summary>
    public static class Transactions
    {
        /// <summary>
        /// Gets the primary category of the given transaction.
        /// </summary>
        /// <param name="transaction">Transaction.</param>
        /// <returns>The primary category of the given transaction.</returns>
        public static string GetPrimaryCategory(Transaction transaction)
        {
            return GetPrimaryCategory(transaction.Category);
        }

        /// <summary>
        /// Gets the primary category from the given category string.
        /// Expected format is [primary category]:[secondary category].
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static string GetPrimaryCategory(string category)
        {
            string[] categories = category.Split(':');
            return categories[0];
        }

        /// <summary>
        /// Gets the secondary category of the given transaction if it exists.
        /// Null is returned if it does not exist.
        /// </summary>
        /// <param name="transaction">Transaction.</param>
        /// <returns>The secondary category of the transaction, or null if it doesn't exist.</returns>
        public static string GetSecondaryCategory(Transaction transaction)
        {
            string[] categories = transaction.Category.Split(':');
            return categories.Length > 1 ? categories[1] : null;
        }

        /// <summary>
        /// Returns true if the transaction has a primary category, false if not.
        /// </summary>
        /// <param name="transaction">Transaction.</param>
        /// <returns>True if the transaction has a primary category, false if not.</returns>
        public static bool HasPrimaryCategory(Transaction transaction)
        {
            return string.IsNullOrEmpty(GetPrimaryCategory(transaction));
        }

        /// <summary>
        /// Returns true if the transaction has a secondary category, false if not.
        /// </summary>
        /// <param name="transaction">Transaction.</param>
        /// <returns>True if the transaction has a secondary category, false if not.</returns>
        public static bool HasSecondaryCategory(Transaction transaction)
        {
            return string.IsNullOrEmpty(GetSecondaryCategory(transaction));
        }

        /// <summary>
        /// Returns true if the transaction is categorized with a known category, false if not.
        /// If the transaction is split, this will return true if the total amount is categorized
        /// to known categories.
        /// </summary>
        /// <param name="transaction">Transaction.</param>
        /// <param name="categories">Categories to check against.</param>
        /// <returns>True if the transaction is categorized with a known category, false if not.</returns>
        public static bool IsCategorized(Transaction transaction, Categories categories)
        {
            if(transaction.IsSplit)
            {
                bool allEntriesCategorized = transaction.SplitEntries.All(it => categories.PrimaryCategoryExists(GetPrimaryCategory(it.Category)));
                
                decimal totalSplit = transaction.SplitEntries.Sum(it => it.Amount);
                bool totalAmountAssigned = transaction.Amount == totalSplit;

                return allEntriesCategorized && totalAmountAssigned;
            }

            return categories.PrimaryCategoryExists(GetPrimaryCategory(transaction));
        }

        /// <summary>
        /// Returns true if the given transaction applies to the given primary category.
        /// If the transaction is split, this will return true if any of the split entries
        /// apply to the category.
        /// </summary>
        /// <param name="transaction">Transaction.</param>
        /// <param name="primaryCategory">Primary category.</param>
        /// <returns>True if the transaction applies to the primary category.</returns>
        public static bool AppliesToPrimaryCategory(Transaction transaction, string primaryCategory)
        {
            return transaction.IsSplit
                ? transaction.SplitEntries.Any(it => GetPrimaryCategory(it.Category) == primaryCategory)
                : !string.IsNullOrEmpty(transaction.Category) && GetPrimaryCategory(transaction) == primaryCategory;
        }

        /// <summary>
        /// Returns the amount of the transaction that applies to the given primary category.
        /// </summary>
        /// <param name="transaction">Transaction.</param>
        /// <param name="primaryCategory">Primary category.</param>
        /// <returns>Amount of the transaction which applies to the primary category.</returns>
        public static decimal AmountForPrimaryCategory(Transaction transaction, string primaryCategory)
        {
            return transaction.IsSplit
                 ? transaction.SplitEntries.Where(it => GetPrimaryCategory(it.Category) == primaryCategory).Sum(it => it.Amount)
                 : AppliesToPrimaryCategory(transaction, primaryCategory) ? transaction.Amount : 0;
        }

        /// <summary>
        /// Returns the amount of the transaction which is uncategorized.
        /// </summary>
        /// <param name="transaction">Transaction.</param>
        /// <param name="categories">The budget categories.</param>
        /// <returns>The amount of the transaction which is uncategorized.</returns>
        public static decimal UncategorizedAmount(Transaction transaction, Categories categories)
        {
            if(transaction.IsSplit)
            {
                decimal categorizedAmount = transaction.SplitEntries.Where(it => categories.PrimaryCategoryExists(GetPrimaryCategory(it.Category))).Sum(it => it.Amount);

                return transaction.Amount - categorizedAmount;
            }

            return IsCategorized(transaction, categories) ? 0 : transaction.Amount;
        }
    }
}
