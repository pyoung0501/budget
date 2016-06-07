namespace BTQLib
{
    /// <summary>
    /// Represents the budget for a single month.
    /// </summary>
    public class MonthlyBudget
    {
        /// <summary>
        /// Budget month.
        /// </summary>
        public int Month { get { return _month; } }

        /// <summary>
        /// Budget year.
        /// </summary>
        public int Year { get { return _year; } }

        /// <summary>
        /// Budget month.
        /// </summary>
        private int _month;

        /// <summary>
        /// Budget year.
        /// </summary>
        private int _year;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="month">Budget month.</param>
        /// <param name="year">Budget year.</param>
        public MonthlyBudget(int month, int year)
        {
            _month = month;
            _year = year;
        }
    }
}
