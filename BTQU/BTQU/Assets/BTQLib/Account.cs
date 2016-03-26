namespace BTQ
{
    /// <summary>
    /// A financial account containing transactions.
    /// </summary>
    public class Account
    {
        public string ID { get; set; }

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
    }
}
