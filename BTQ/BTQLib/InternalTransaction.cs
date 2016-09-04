namespace BTQLib
{
    /// <summary>
    /// Defines an internal transaction between categories.
    /// </summary>
    public class InternalTransaction
    {
        /// <summary>
        /// Category transfering amount from.
        /// </summary>
        public string fromCategory;

        /// <summary>
        /// Category transfering amount to.
        /// </summary>
        public string toCategory;

        /// <summary>
        /// Amount to transfer between the categories.
        /// </summary>
        public decimal amount;
    }
}
