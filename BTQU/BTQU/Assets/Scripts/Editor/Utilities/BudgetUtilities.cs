using BTQLib;
using System.Linq;
using System;

public static class BudgetUtilities
{
    /// <summary>
    /// Gets the income transactions for the given month and year.
    /// </summary>
    /// <param name="profile">Profile.</param>
    /// <param name="month">Month.</param>
    /// <param name="year">Year.</param>
    /// <returns>The income transactions for the given month and year.</returns>
    public static Transaction[] GetIncomeTransactions(Profile profile, int month, int year)
    {
        return profile.Accounts.SelectMany(a => a.Transactions)
                               .Where(t => t.Date.Year == year && t.Date.Month == month)
                               .Where(t => t.Amount > 0)
                               .ToArray();
    }

    /// <summary>
    /// Gets the expense transactions for the given month and year.
    /// </summary>
    /// <param name="profile">Profile.</param>
    /// <param name="month">Month.</param>
    /// <param name="year">Year.</param>
    /// <returns>The expense transactions for the given month and year.</returns>
    public static Transaction[] GetExpenseTransactions(Profile profile, int month, int year)
    {
        return profile.Accounts.SelectMany(a => a.Transactions)
                               .Where(t => t.Date.Year == year && t.Date.Month == month)
                               .Where(t => t.Amount < 0)
                               .ToArray();
    }

    /// <summary>
    /// Gets the income transactions for the given monthly budget.
    /// </summary>
    /// <param name="profile">Profile.</param>
    /// <param name="monthlyBudget">Month budget.</param>
    /// <returns>The income transactions for the given monthly budget.</returns>
    public static Transaction[] GetIncomeTransactions(Profile profile, MonthlyBudget monthlyBudget)
    {
        return GetIncomeTransactions(profile, monthlyBudget.Month, monthlyBudget.Year);
    }

    /// <summary>
    /// Gets the expense transactions for the given monthly budget.
    /// </summary>
    /// <param name="profile">Profile.</param>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The expense transactions for the given monthly budget.</returns>
    public static Transaction[] GetExpenseTransactions(Profile profile, MonthlyBudget monthlyBudget)
    {
        return GetExpenseTransactions(profile, monthlyBudget.Month, monthlyBudget.Year);
    }

    /// <summary>
    /// Gets the balance up through the given monthly budget.
    /// </summary>
    /// <param name="profile">Profile.</param>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The balance up through the given monthly budget.</returns>
    public static decimal GetBalance(Profile profile, MonthlyBudget monthlyBudget)
    {
        decimal previousBalance;
        {
            MonthlyBudget prevMonthlyBudget = GetPreviousMonthlyBudget(profile, monthlyBudget);
            previousBalance = prevMonthlyBudget != null
                            ? GetBalance(profile, prevMonthlyBudget)
                            : GetStartingBalance(profile);
        }

        decimal totalExpenses = GetTotalExpenses(profile, monthlyBudget);
        decimal totalIncome = GetTotalIncome(profile, monthlyBudget);

        return previousBalance + totalExpenses + totalIncome;
    }

    /// <summary>
    /// Gets the overall starting balance of the profile.
    /// </summary>
    /// <param name="profile">Profile.</param>
    /// <returns>The overall starting balance of the profile.</returns>
    private static decimal GetStartingBalance(Profile profile)
    {
        return profile.Accounts.Select(a => a.StartingBalance).Sum();
    }

    /// <summary>
    /// Gets the monthly budget previous to the given one or null if there is
    /// not a previous one.
    /// </summary>
    /// <param name="profile">Profile.</param>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The monthly budget previous to the given one.</returns>
    public static MonthlyBudget GetPreviousMonthlyBudget(Profile profile, MonthlyBudget monthlyBudget)
    {
        int prevIndex = profile.Budget.MonthlyBudgets.IndexOf(monthlyBudget) - 1;
        return prevIndex >= 0 ? profile.Budget.MonthlyBudgets[prevIndex] : null;
    }

    /// <summary>
    /// Gets the total monthly expenses for the given monthly budget.
    /// </summary>
    /// <param name="profile">Profile.</param>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The total monthly expenses for the given monthly budget.</returns>
    public static decimal GetTotalExpenses(Profile profile, MonthlyBudget monthlyBudget)
    {
        Transaction[] transactions = GetExpenseTransactions(profile, monthlyBudget);
        return transactions.Sum(t => t.Amount);
    }

    /// <summary>
    /// Gets the total monthly income for the given monthly budget.
    /// </summary>
    /// <param name="profile">Profile.</param>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The total monthly income for the given monthly budget.</returns>
    public static decimal GetTotalIncome(Profile profile, MonthlyBudget monthlyBudget)
    {
        Transaction[] transactions = GetIncomeTransactions(profile, monthlyBudget);
        return transactions.Sum(t => t.Amount);
    }
}
