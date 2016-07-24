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
            string[] categories = transaction.Category.Split(':');
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
        /// </summary>
        /// <param name="transaction">Transaction.</param>
        /// <param name="categories">Categories to check against.</param>
        /// <returns>True if the transaction is categorized with a known category, false if not.</returns>
        public static bool IsCategorized(Transaction transaction, Categories categories)
        {
            return categories.PrimaryCategoryExists(GetPrimaryCategory(transaction));
        }
    }
}
