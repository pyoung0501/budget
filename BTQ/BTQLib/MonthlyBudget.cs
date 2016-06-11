using System.Collections.Generic;

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
        /// Categories mapped to percentages.
        /// </summary>
        public Dictionary<string, float> CategoryPercentages { get { return _categoryPercentages; } }

        /// <summary>
        /// Budget month.
        /// </summary>
        private int _month;

        /// <summary>
        /// Budget year.
        /// </summary>
        private int _year;

        /// <summary>
        /// Categories mapped to percentages.
        /// </summary>
        private Dictionary<string, float> _categoryPercentages = new Dictionary<string, float>();

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

        /// <summary>
        /// Gets the percentage for the given (primary) category in the range 0-1.
        /// </summary>
        /// <param name="primaryCategory">Primary category.</param>
        /// <returns>The percentage for the given primary category.</returns>
        public float GetPercentage(string primaryCategory)
        {
            return _categoryPercentages.ContainsKey(primaryCategory)
                 ? _categoryPercentages[primaryCategory]
                 : 0;
        }

        /// <summary>
        /// Sets the percentage for the specified primary category.
        /// Percentage is expected to be in the range 0-1.
        /// </summary>
        /// <param name="primaryCategory">Primary category.</param>
        /// <param name="percentage">Percentage to set to (0-1).</param>
        public void SetPercentage(string primaryCategory, float percentage)
        {
            if(!_categoryPercentages.ContainsKey(primaryCategory))
            {
                _categoryPercentages.Add(primaryCategory, percentage);
            }
            else
            {
                _categoryPercentages[primaryCategory] = percentage;
            }
        }
    }
}
