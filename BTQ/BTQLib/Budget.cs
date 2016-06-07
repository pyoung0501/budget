using System;
using System.Collections.Generic;
using System.Linq;
using BTQLib.Util;

namespace BTQLib
{
    /// <summary>
    /// Class used to manage the budget within a profile.
    /// </summary>
    public class Budget
    {
        /// <summary>
        /// Whether or not the budget has been created.
        /// </summary>
        public bool Created { get { return _monthlyBudgets.Count > 0; } }

        /// <summary>
        /// The budgets for each month.
        /// </summary>
        public List<MonthlyBudget> MonthlyBudgets { get { return _monthlyBudgets; } }

        /// <summary>
        /// The budgets for each month.
        /// </summary>
        private List<MonthlyBudget> _monthlyBudgets = new List<MonthlyBudget>();

        /// <summary>
        /// Creates the monthly budget objects for the given profile.
        /// This will create a monthly budget starting at the oldest transaction
        /// and continue up through the newest transaction.
        /// </summary>
        /// <param name="profile">Profile.</param>
        /// <returns>
        /// True if successful; otherwise, false if failure.
        /// This can fail for two reasons:
        ///     1) Monthly budgets already exist for the profile.
        ///     2) The profile contains no accounts with transactions.
        /// </returns>
        public bool CreateMonthlyBudgets(Profile profile)
        {
            if(Created)
            {
                return false;
            }

            Tuple<int, int> minMonthAndYear = FindMinimumMonthAndYear(profile);
            Tuple<int, int> maxMonthAndYear = FindMaximumMonthAndYear(profile);

            if(minMonthAndYear == null)
            {
                // The profile contains not accounts with transactions.
                // Cannot create monthly budgets.
                return false;
            }

            int minMonth = minMonthAndYear.Item1;
            int minYear = minMonthAndYear.Item2;

            int maxMonth = maxMonthAndYear.Item1;
            int maxYear = maxMonthAndYear.Item2;

            // Create monthly budgets
            for(int iYear = minYear; iYear <= maxYear; ++iYear)
            {
                int startingMonth = (iYear == minYear) ? minMonth : 1;
                int endingMonth = (iYear == maxYear) ? maxMonth : 12;

                for (int iMonth = startingMonth; iMonth <= endingMonth; ++iMonth)
                {
                    _monthlyBudgets.Add(new MonthlyBudget(iMonth, iYear));
                }
            }

            return true;
        }

        /// <summary>
        /// Finds the minimum month and year represented by the transactions
        /// in the profile's accounts.
        /// </summary>
        /// <param name="profile">Profile.</param>
        /// <returns>Tuple containing first the minimum month and second the minimum year.</returns>
        private Tuple<int, int> FindMinimumMonthAndYear(Profile profile)
        {
            DateTime minDate = profile.Accounts.Where(account => account.Transactions.Count > 0)
                                               .Select(account => account.Transactions)
                                               .Select(transactions => transactions.Min(transaction => transaction.Date))
                                               .Min();
            
            if(minDate == null)
            {
                return null;
            }

            return new Tuple<int, int>(minDate.Month, minDate.Year);
        }

        /// <summary>
        /// Finds the maximum month and year represented by the transactions
        /// in the profile's accounts.
        /// </summary>
        /// <param name="profile">Profile.</param>
        /// <returns>Tuple containing first the maximum month and second the maximum year.</returns>
        private Tuple<int, int> FindMaximumMonthAndYear(Profile profile)
        {

            DateTime maxDate = profile.Accounts.Where(account => account.Transactions.Count > 0)
                                               .Select(account => account.Transactions)
                                               .Select(transactions => transactions.Max(transaction => transaction.Date))
                                               .Max();

            if (maxDate == null)
            {
                return null;
            }

            return new Tuple<int, int>(maxDate.Month, maxDate.Year);
        }
    }
}
