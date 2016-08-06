using BTQ;
using BTQLib;
using BTQLib.Util;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Controller for a Monthly Budget object.
/// </summary>
public class MonthlyBudgetController
{
    /// <summary>
    /// Monthly budget.
    /// </summary>
    private MonthlyBudget _monthlyBudget;

    /// <summary>
    /// Parent profile.
    /// </summary>
    private Profile _profile;

    /// <summary>
    /// List of month names in chronological order.
    /// </summary>
    private static readonly string[] _monthNames = new string[]
    {
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December",
    };

    /// <summary>
    /// The income transactions.
    /// </summary>
    private Transaction[] _incomeTransactions;

    /// <summary>
    /// The expense transactions.
    /// </summary>
    private Transaction[] _expenseTransactions;

    /// <summary>
    /// The uncategorized expense transactions.
    /// </summary>
    private Transaction[] _uncategorizedExpenses;

    /// <summary>
    /// The uncategorized income transactions.
    /// </summary>
    private Transaction[] _uncategorizedIncome;

    /// <summary>
    /// The total amount of income which is to be applied to
    /// the whole of the budget categories.
    /// </summary>
    private decimal _totalIncomeAppliedToWhole;

    /// <summary>
    /// The balance of uncategorized transactions prior to the
    /// current monthly budget.
    /// </summary>
    private decimal _previousUncategorizedBalance;

    /// <summary>
    /// The total amount of uncategorized expenses.
    /// </summary>
    private decimal _totalUncategorizedExpenses;

    /// <summary>
    /// The total amount of uncategorized income.
    /// </summary>
    private decimal _totalUncategorizedIncome;

    /// <summary>
    /// Mapping of total expenses for each category.
    /// </summary>
    private Dictionary<string, decimal> _expensesPerCategory;

    /// <summary>
    /// Mapping of total income for each category.
    /// </summary>
    private Dictionary<string, decimal> _incomePerCategory;

    /// <summary>
    /// Mapping of previous balances for each category.
    /// </summary>
    Dictionary<string, decimal> _previousBalancePerCategory;

    /// <summary>
    /// View for expense transactions.
    /// </summary>
    private TransactionsView _expensesView;

    /// <summary>
    /// View for income transactions.
    /// </summary>
    private TransactionsView _incomeView;
    
    /// <summary>
    /// Whether or not to show all the transactions, or only the 
    /// uncategorized/unassigned transactions.
    /// </summary>
    private bool _showAllTransactions;

    /// <summary>
    /// The active tab to display.
    /// </summary>
    private Tab _activeTab = Tab.Expenses;


    /// <summary>
    /// Tabs in the monthly budget controller.
    /// </summary>
    protected enum Tab
    {
        Expenses,
        Income
    }


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    public MonthlyBudgetController(MonthlyBudget monthlyBudget, Profile profile)
    {
        _monthlyBudget = monthlyBudget;
        _profile = profile;

        Initialize();
    }

    /// <summary>
    /// Initializes the controller.
    /// </summary>
    private void Initialize()
    {
        _expensesView = new TransactionsView(
            new TransactionsView.Settings()
            {
                Columns = new TransactionColumn[]
                {
                    new DateColumn() { Width = 80.0f, Editable = false },
                    new PayeeColumn() { Width = 300.0f, Editable = false },
                    new DescriptionColumn() { Width = 300.0f, Editable = false },
                    new CategoryColumn(_profile.Categories) { Width = 200.0f },
                    new AmountColumn() { Width = 100.0f, Editable = false }
                }
            });

        _incomeView = new TransactionsView(
            new TransactionsView.Settings()
            {
                Columns = new TransactionColumn[]
                {
                    new DateColumn() { Width = 80.0f, Editable = false },
                    new PayeeColumn() { Width = 300.0f, Editable = false },
                    new DescriptionColumn() { Width = 300.0f, Editable = false },
                    new CategoryColumn(_profile.Categories) { Width = 200.0f },
                    new AmountColumn() { Width = 100.0f, Editable = false },
                    new AppliedToColumn() { Width = 120.0f, Editable = true },
                    new CategoryAppliedToColumn(_profile.Categories) { Width = 200.0f, Editable = true }
                }
            });

        Refresh();
    }

    /// <summary>
    /// Refreshes the displayed data.
    /// </summary>
    private void Refresh()
    {
        _incomeTransactions = GetIncomeTransactions(_monthlyBudget);
        _expenseTransactions = GetExpenseTransactions(_monthlyBudget);
        _uncategorizedExpenses = GetUncategorizedExpenses(_monthlyBudget);
        _uncategorizedIncome = GetUncategorizedIncome(_monthlyBudget);

        _totalIncomeAppliedToWhole = GetIncomeAppliedToWhole(_monthlyBudget);

        _previousBalancePerCategory = GetPreviousBalancePerCategory(_monthlyBudget);
        _expensesPerCategory = GetExpensesPerCategory(_monthlyBudget);
        _incomePerCategory = GetIncomePerCategory(_monthlyBudget);

        _previousUncategorizedBalance = GetPreviousUncategorizedBalance(_monthlyBudget);
        _totalUncategorizedExpenses = GetTotalUncategorizedExpenses(_monthlyBudget);
        _totalUncategorizedIncome = GetTotalUncategorizedIncome(_monthlyBudget);

        _expensesView.Refresh(_showAllTransactions ? _expenseTransactions.ToList() : _uncategorizedExpenses.ToList());
        _incomeView.Refresh(_showAllTransactions ? _incomeTransactions.ToList() : _uncategorizedIncome.ToList());
    }

    #region GUI

    /// <summary>
    /// Draws the monthly budget.
    /// </summary>
    public void DrawView()
    {
        DrawMonthTitle();
        DrawCategories();
        DrawTabs();
    }

    /// <summary>
    /// Draws the title of the monthly budget.
    /// </summary>
    private void DrawMonthTitle()
    {
        EditorUtilities.BeginHorizontalCentering();
        {
            EditorGUILayout.BeginHorizontal("box");
            {
                string text = string.Format("{0} {1}", _monthNames[_monthlyBudget.Month - 1], _monthlyBudget.Year);

                GUILayout.Space(16.0f);
                EditorUtilities.ContentWidthLabel(text, EditorStyles.boldLabel);
                GUILayout.Space(16.0f);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorUtilities.EndHorizontalCentering();
    }

    /// <summary>
    /// Draws the budget categories.
    /// </summary>
    private void DrawCategories()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical("box");
        {
            EditorUtilities.BeginHorizontalCentering();
            {
                EditorUtilities.ContentWidthLabel("Percentages", EditorStyles.boldLabel);
            }
            EditorUtilities.EndHorizontalCentering();

            float remainingPercentage = 100.0f;
            foreach(string category in _profile.Categories.PrimaryCategories)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(category, GUILayout.Width(100.0f));

                    float percentage = _monthlyBudget.GetPercentage(category) * 100.0f;
                    float newPercentage = EditorGUILayout.FloatField(percentage, GUILayout.Width(50.0f));
                    remainingPercentage -= newPercentage;

                    if (Mathf.Abs(newPercentage - percentage) >= 0.01f)
                    {
                        _monthlyBudget.SetPercentage(category, Mathf.Floor(newPercentage * 100.0f) / 10000.0f);
                        _incomePerCategory[category] = GetIncome(_monthlyBudget, category);
                    }

                    EditorUtilities.ContentWidthLabel("%");
                }
                EditorGUILayout.EndHorizontal();
            }

            // Total percentage
            EditorGUILayout.BeginHorizontal();
            {
                Color color = remainingPercentage > 0 ? Color.white : remainingPercentage == 0 ? Color.gray : Color.red;
                EditorUtilities.BeginBackgroundColor(color);
                EditorUtilities.BeginEnabled(false);
                {
                    EditorGUILayout.LabelField("(Total)", GUILayout.Width(100.0f));
                    EditorGUILayout.FloatField(100.0f - remainingPercentage, GUILayout.Width(50.0f));
                    EditorUtilities.ContentWidthLabel("%");
                }
                EditorUtilities.EndEnabled();
                EditorUtilities.EndBackgroundColor();
            }
            EditorGUILayout.EndHorizontal();

            // Unassigned percentage
            EditorGUILayout.BeginHorizontal();
            {
                Color color = remainingPercentage > 0 ? Color.white : remainingPercentage == 0 ? Color.gray : Color.red;
                EditorUtilities.BeginBackgroundColor(color);
                EditorUtilities.BeginEnabled(false);
                {
                    EditorGUILayout.LabelField("(Unassigned)", GUILayout.Width(100.0f));
                    EditorGUILayout.FloatField(remainingPercentage, GUILayout.Width(50.0f));
                    EditorUtilities.ContentWidthLabel("%");
                }
                EditorUtilities.EndEnabled();
                EditorUtilities.EndBackgroundColor();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        decimal totalPrevious = 0;
        decimal totalExpenses = 0;
        decimal totalIncome = 0;
        decimal totalBalance = 0;

        EditorGUILayout.BeginVertical("box");
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Category", EditorStyles.boldLabel, GUILayout.Width(100.0f));
                EditorGUILayout.LabelField("Previous Balance", Styles.RightAlignedBoldWrappedLabel, GUILayout.Width(80.0f));
                EditorGUILayout.LabelField("Expenses", Styles.RightAlignedBoldWrappedLabel, GUILayout.Width(80.0f));
                EditorGUILayout.LabelField("Income", Styles.RightAlignedBoldWrappedLabel, GUILayout.Width(80.0f));
                EditorGUILayout.LabelField("Current Balance", Styles.RightAlignedBoldWrappedLabel, GUILayout.Width(80.0f));
            }
            EditorGUILayout.EndHorizontal();

            foreach (string category in _profile.Categories.PrimaryCategories)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(category, GUILayout.Width(100.0f));

                    decimal previousBalance = _previousBalancePerCategory[category];
                    decimal expenses = _expensesPerCategory[category];
                    decimal income = _incomePerCategory[category];
                    decimal currentBalance = previousBalance + expenses + income;

                    totalPrevious += previousBalance;
                    totalExpenses += expenses;
                    totalIncome += income;
                    totalBalance += currentBalance;

                    EditorGUILayout.LabelField(previousBalance.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
                    EditorGUILayout.LabelField(expenses.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
                    EditorGUILayout.LabelField(income.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
                    EditorGUILayout.LabelField(currentBalance.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
                }
                EditorGUILayout.EndHorizontal();
            }

            // Unassigned amount
            EditorUtilities.BeginEnabled(false);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Unassigned", GUILayout.Width(100.0f));

                decimal previousBalance = _previousUncategorizedBalance;
                decimal expenses = _totalUncategorizedExpenses;
                decimal income = _totalUncategorizedIncome;
                decimal currentBalance = previousBalance + expenses + income;

                totalPrevious += previousBalance;
                totalExpenses += expenses;
                totalIncome += income;
                totalBalance += currentBalance;

                EditorGUILayout.LabelField(previousBalance.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
                EditorGUILayout.LabelField(expenses.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
                EditorGUILayout.LabelField(income.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
                EditorGUILayout.LabelField(currentBalance.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
            }
            EditorGUILayout.EndHorizontal();
            EditorUtilities.EndEnabled();

            // Total percentage
            EditorUtilities.BeginEnabled(false);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Total", GUILayout.Width(100.0f));

                EditorGUILayout.LabelField(totalPrevious.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
                EditorGUILayout.LabelField(totalExpenses.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
                EditorGUILayout.LabelField(totalIncome.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
                EditorGUILayout.LabelField(totalBalance.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
            }
            EditorGUILayout.EndHorizontal();
            EditorUtilities.EndEnabled();
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the tabs section.
    /// </summary>
    private void DrawTabs()
    {
        EditorGUILayout.BeginVertical("box");
        {
            EditorGUILayout.BeginHorizontal();
            {
                string prefix = _showAllTransactions ? "" : "Uncategorized ";

                Color selectedColor = new Color(0, 1, 1);
                Color unselectedColor = new Color(1, 1, 1);

                bool ueSelected = _activeTab == Tab.Expenses;
                if (EditorUtilities.Button(prefix + "Expenses", ueSelected ? selectedColor : unselectedColor))
                {
                    _activeTab = Tab.Expenses;
                }

                bool uiSelected = _activeTab == Tab.Income;
                if (EditorUtilities.Button(prefix + "Income", uiSelected ? selectedColor : unselectedColor))
                {
                    _activeTab = Tab.Income;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (_activeTab == Tab.Expenses)
            {
                DrawExpenses();
            }
            else if (_activeTab == Tab.Income)
            {
                DrawIncome();
            }
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Draws the expenses for the month.
    /// </summary>
    private void DrawExpenses()
    {
        // Header
        {
            string prefix = _showAllTransactions ? "" : "Uncategorized ";

            EditorUtilities.BeginHorizontalCentering();
            EditorUtilities.ContentWidthLabel(prefix + "Expenses", EditorStyles.boldLabel);
            EditorUtilities.EndHorizontalCentering();
        }

        // Expense transactions.
        _expensesView.Draw();

        // Footer
        EditorGUILayout.BeginHorizontal();
        {
            EditorUtilities.ContentWidthLabel("Balance");
            EditorGUILayout.LabelField(_totalUncategorizedExpenses.ToString("C2"));

            bool prevShow = _showAllTransactions;
            bool newShow = EditorGUILayout.ToggleLeft("Show All", _showAllTransactions);
            if (prevShow != newShow)
            {
                _showAllTransactions = newShow;
                Refresh();
            }

            GUILayout.FlexibleSpace();

            if (EditorUtilities.ContentWidthButton("Refresh"))
            {
                Refresh();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the income transactions for the month.
    /// </summary>
    private void DrawIncome()
    {
        // Header
        {
            string prefix = _showAllTransactions ? "" : "Uncategorized ";

            EditorUtilities.BeginHorizontalCentering();
            EditorUtilities.ContentWidthLabel(prefix + "Income", EditorStyles.boldLabel);
            EditorUtilities.EndHorizontalCentering();
        }

        // Transactions
        _incomeView.Draw();

        // Footer
        EditorGUILayout.BeginHorizontal();
        {
            EditorUtilities.ContentWidthLabel("Balance");
            EditorGUILayout.LabelField(_totalUncategorizedIncome.ToString("C2"));

            bool prevShow = _showAllTransactions;
            bool newShow = EditorGUILayout.ToggleLeft("Show All", _showAllTransactions);
            if (prevShow != newShow)
            {
                _showAllTransactions = newShow;
                Refresh();
            }

            GUILayout.FlexibleSpace();

            if (EditorUtilities.ContentWidthButton("Refresh"))
            {
                Refresh();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    #endregion GUI

    /// <summary>
    /// Gets the income transactions for the given monthly budget.
    /// </summary>
    /// <param name="monthlyBudget">Month budget.</param>
    /// <returns>The income transactions for the given monthly budget.</returns>
    private Transaction[] GetIncomeTransactions(MonthlyBudget monthlyBudget)
    {
        return GetIncomeTransactions(monthlyBudget.Month, monthlyBudget.Year);
    }

    /// <summary>
    /// Gets the income transactions for the given month and year.
    /// </summary>
    /// <param name="month">Month.</param>
    /// <param name="year">Year.</param>
    /// <returns>The income transactions for the given month and year.</returns>
    private Transaction[] GetIncomeTransactions(int month, int year)
    {
        return _profile.Accounts.SelectMany(a => a.Transactions)
                                .Where(t => t.Date.Year == year && t.Date.Month == month)
                                .Where(t => t.Amount > 0)
                                .ToArray();
    }

    /// <summary>
    /// Gets the expense transactions for the given monthly budget.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The expense transactions for the given monthly budget.</returns>
    private Transaction[] GetExpenseTransactions(MonthlyBudget monthlyBudget)
    {
        return GetExpenseTransactions(monthlyBudget.Month, monthlyBudget.Year);
    }

    /// <summary>
    /// Gets the expense transactions for the given month and year.
    /// </summary>
    /// <param name="month">Month.</param>
    /// <param name="year">Year.</param>
    /// <returns>The expense transactions for the given month and year.</returns>
    private Transaction[] GetExpenseTransactions(int month, int year)
    {
        return _profile.Accounts.SelectMany(a => a.Transactions)
                                .Where(t => t.Date.Year == year && t.Date.Month == month)
                                .Where(t => t.Amount < 0)
                                .ToArray();
    }

    /// <summary>
    /// Gets the uncategorized expenses for the given monthly budget.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The uncategorized expenses for the given monthly budget.</returns>
    private Transaction[] GetUncategorizedExpenses(MonthlyBudget monthlyBudget)
    {
        Transaction[] expenseTransactions = GetExpenseTransactions(monthlyBudget);
        return expenseTransactions.Where(t => !Transactions.IsCategorized(t, _profile.Categories))
                                  .ToArray();
    }

    /// <summary>
    /// Gets the uncategorized income transactions for the given monthly budget.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The uncategorized income transactions for the given monthly budget.</returns>
    private Transaction[] GetUncategorizedIncome(MonthlyBudget monthlyBudget)
    {
        Transaction[] incomeTransactions = GetIncomeTransactions(monthlyBudget);
        return incomeTransactions.Where(t => !Transactions.IsCategorized(t, _profile.Categories)
                                             || t.AppliedState == AppliedState.NotApplied
                                             || (t.AppliedState == AppliedState.ApplyToCategory && !_profile.Categories.PrimaryCategoryExists(t.AppliedToCategory)))
                                 .ToArray();
    }

    /// <summary>
    /// Gets the expenses per category for the given monthly budget.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The expenses per category for the given monthly budget.</returns>
    private Dictionary<string, decimal> GetExpensesPerCategory(MonthlyBudget monthlyBudget)
    {
        Dictionary<string, decimal> expensesPerCategory = new Dictionary<string, decimal>();
        foreach (string category in _profile.Categories.PrimaryCategories)
        {
            expensesPerCategory.Add(category, GetExpenses(monthlyBudget, category));
        }

        return expensesPerCategory;
    }

    /// <summary>
    /// Gets the total monthly expenses for the category in the given monthly budget.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <param name="category">Category.</param>
    /// <returns>The total monthly expenses for the category in the given monthly budget.</returns>
    private decimal GetExpenses(MonthlyBudget monthlyBudget, string category)
    {
        Transaction[] transactions = GetTransactions(category, monthlyBudget.Month, monthlyBudget.Year);
        transactions = transactions.Where(t => t.Amount < 0).ToArray();
        return transactions.Sum(t => t.Amount);
    }

    /// <summary>
    /// Gets the transactions for the specified category, month and year.
    /// </summary>
    /// <param name="category">Category.</param>
    /// <param name="month">Month.</param>
    /// <param name="year">Year.</param>
    /// <returns>The transactions for the specified category, month and year.</returns>
    private Transaction[] GetTransactions(string category, int month, int year)
    {
        return _profile.Accounts.SelectMany(a => a.Transactions)
                                .Where(t => !string.IsNullOrEmpty(t.Category) && t.Category.StartsWith(category))
                                .Where(t => t.Date.Year == year && t.Date.Month == month)
                                .ToArray();
    }

    /// <summary>
    /// Gets the income per category for the given monthly budget.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The income per category for the given monthly budget.</returns>
    private Dictionary<string, decimal> GetIncomePerCategory(MonthlyBudget monthlyBudget)
    {
        Dictionary<string, decimal> incomePerCategory = new Dictionary<string, decimal>();
        foreach (string category in _profile.Categories.PrimaryCategories)
        {
            incomePerCategory.Add(category, GetIncome(monthlyBudget, category));
        }

        return incomePerCategory;
    }

    /// <summary>
    /// Gets the total monthly income for the given category.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <param name="category">Category.</param>
    /// <returns>The total monthly income for the given category.</returns>
    private decimal GetIncome(MonthlyBudget monthlyBudget, string category)
    {
        // TODO: change percentages to be stored as decimal
        float percentage = monthlyBudget.GetPercentage(category);
        decimal incomeFromWhole = GetIncomeAppliedToWhole(monthlyBudget) * (decimal)percentage;

        Transaction[] incomeTransactions = GetIncomeTransactions(monthlyBudget);
        decimal incomeFromSpecific = incomeTransactions.Where(t => t.AppliedState == AppliedState.ApplyToCategory)
                                                       .Where(t => t.AppliedToCategory == category)
                                                       .Sum(t => t.Amount);

        return incomeFromWhole + incomeFromSpecific;
    }

    /// <summary>
    /// Gets the amount in income applied to all categories in the given monthly budget.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The amount in income applied to all categories in the given monthly budget.</returns>
    private decimal GetIncomeAppliedToWhole(MonthlyBudget monthlyBudget)
    {
        Transaction[] incomeTransactions = GetIncomeTransactions(monthlyBudget);
        return incomeTransactions.Where(t => t.AppliedState == AppliedState.ApplyToWhole)
                                 .Sum(t => t.Amount);
    }

    /// <summary>
    /// Gets the total uncategorized expenses for the given monthly budget.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The total uncategorized expenses for the given monthly budget.</returns>
    private decimal GetTotalUncategorizedExpenses(MonthlyBudget monthlyBudget)
    {
        Transaction[] uncategorizedExpenses = GetUncategorizedExpenses(monthlyBudget);
        return uncategorizedExpenses.Sum(t => t.Amount);
    }

    /// <summary>
    /// Gets the total uncategorized income in the given monthly budget.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The total uncategorized income in the given monthly budget.</returns>
    private decimal GetTotalUncategorizedIncome(MonthlyBudget monthlyBudget)
    {
        Transaction[] uncategorizedIncome = GetUncategorizedIncome(monthlyBudget);
        return uncategorizedIncome.Sum(t => t.Amount);
    }
    
    /// <summary>
    /// Gets the previous balances per category for the given monthly budget.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The previous balances per category for the given monthly budget.</returns>
    private Dictionary<string, decimal> GetPreviousBalancePerCategory(MonthlyBudget monthlyBudget)
    {
        Dictionary<string, decimal> previousBalancePerCategory = new Dictionary<string, decimal>();
        foreach (string category in _profile.Categories.PrimaryCategories)
        {
            MonthlyBudget prevMonthlyBudget = GetPreviousMonthlyBudget(monthlyBudget);
            if (prevMonthlyBudget != null)
            {
                previousBalancePerCategory.Add(category, GetBalance(prevMonthlyBudget, category));
            }
            else
            {
                previousBalancePerCategory.Add(category, 0);
            }
        }

        return previousBalancePerCategory;
    }

    /// <summary>
    /// Gets the monthly budget previous to the given one or null if there is
    /// not a previous one.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The monthly budget previous to the given one.</returns>
    private MonthlyBudget GetPreviousMonthlyBudget(MonthlyBudget monthlyBudget)
    {
        int prevIndex = _profile.Budget.MonthlyBudgets.IndexOf(monthlyBudget) - 1;
        return prevIndex >= 0 ? _profile.Budget.MonthlyBudgets[prevIndex] : null;
    }

    /// <summary>
    /// Gets the balance for the category in the given monthly budget.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <param name="category">Category.</param>
    /// <returns>The balance for the category in the given monthly budget.</returns>
    public decimal GetBalance(MonthlyBudget monthlyBudget, string category)
    {
        decimal previousBalance;
        {
            MonthlyBudget prevMonthlyBudget = GetPreviousMonthlyBudget(monthlyBudget);
            previousBalance = prevMonthlyBudget != null
                            ? GetBalance(prevMonthlyBudget, category)
                            : 0;
        }
                
        decimal totalExpenses = GetExpenses(monthlyBudget, category);
        decimal totalIncome = GetIncome(monthlyBudget, category);

        return previousBalance + totalExpenses + totalIncome;
    }

    /// <summary>
    /// Gets the balance of the uncategorized transactions for the monthly budget
    /// previous to the given one.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The balance of the uncategorized transactions for the previous monthly budget.</returns>
    private decimal GetPreviousUncategorizedBalance(MonthlyBudget monthlyBudget)
    {
        MonthlyBudget prevMonthlyBudget = GetPreviousMonthlyBudget(monthlyBudget);
        if (prevMonthlyBudget != null)
        {
            return GetUncategorizedBalance(prevMonthlyBudget);
        }

        return 0;
    }

    /// <summary>
    /// Gets the balance of the uncategorized transactions for the given monthly budget.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The balance of the uncategorized transactions for the given monthly budget.</returns>
    private decimal GetUncategorizedBalance(MonthlyBudget monthlyBudget)
    {
        decimal previousUncategorizedBalance;
        {
            MonthlyBudget prevMonthlyBudget = GetPreviousMonthlyBudget(monthlyBudget);
            previousUncategorizedBalance = prevMonthlyBudget != null
                                         ? GetUncategorizedBalance(prevMonthlyBudget)
                                         : 0;
        }

        decimal totalUncategorizedExpenses = GetTotalUncategorizedExpenses(monthlyBudget);
        decimal totalUncategorizedIncome = GetTotalUncategorizedIncome(monthlyBudget);

        return previousUncategorizedBalance + totalUncategorizedExpenses + totalUncategorizedIncome;
    }
}
