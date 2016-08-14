using BTQ;
using BTQLib;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Controller for the budget.
/// </summary>
public class BudgetController
{
    // Column widths for the summary data.
    private const float MonthNameWidth = 80.0f;
    private const float ExpensesWidth = 100.0f;
    private const float IncomeWidth = 100.0f;
    private const float NetWidth = 100.0f;
    private const float BalanceWidth = 100.0f;

    /// <summary>
    /// Profile with budget.
    /// </summary>
    private Profile _profile;

    /// <summary>
    /// The current year being viewed.
    /// </summary>
    private int _currYear;

    /// <summary>
    /// The minimum year represented by the budget.
    /// </summary>
    private int _minYear;

    /// <summary>
    /// The maximum year represented by the budget.
    /// </summary>
    private int _maxYear;

    /// <summary>
    /// Scroll view position.
    /// </summary>
    private Vector3 _scrollPos;

    /// <summary>
    /// Controller for the selected monthly budget.
    /// </summary>
    private MonthlyBudgetController _monthlyBudgetController;

    /// <summary>
    /// The summary items for each month of the current year.
    /// </summary>
    private MonthlySummaryItem[] _monthlySummaryItems;

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
    /// Constructor.
    /// </summary>
    /// <param name="profile">Profile.</param>
    public BudgetController(Profile profile)
    {
        _profile = profile;

        if(_profile.Budget.Created)
        {
            _minYear = _profile.Budget.MonthlyBudgets[0].Year;
            _maxYear = _profile.Budget.MonthlyBudgets[_profile.Budget.MonthlyBudgets.Count - 1].Year;
            _currYear = _maxYear;

            UpdateMonthlySummaryData();
        }
    }

    /// <summary>
    /// Draws the budget view.
    /// </summary>
    public void DrawView()
    {
        if(_currYear == 0)
        {
            return;
        }

        // Draw Selected Monthly Budget
        if(_monthlyBudgetController != null)
        {
            EditorGUILayout.BeginHorizontal("box");
            {
                if(EditorUtilities.ContentWidthButton("< Budget"))
                {
                    _monthlyBudgetController = null;
                }

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            if (_monthlyBudgetController != null)
            {
                _monthlyBudgetController.DrawView();
            }

            return;
        }

        EditorGUILayout.BeginHorizontal();
        {
            DrawSummaryData();

            EditorGUILayout.BeginVertical("box");
            {
                _scrollPos =
                EditorGUILayout.BeginScrollView(_scrollPos);
                {
                    EditorUtilities.BeginHorizontalCentering();
                    EditorGUILayout.BeginVertical();
                    {
                        DrawMonths();
                    }
                    EditorGUILayout.EndVertical();
                    EditorUtilities.EndHorizontalCentering();
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal("box");
        {
            // Go to Previous Year
            if(EditorUtilities.ContentWidthButton("<<", _minYear < _currYear))
            {
                --_currYear;
                UpdateMonthlySummaryData();
            }

            GUILayout.FlexibleSpace();

            // Current Year
            EditorUtilities.ContentWidthLabel(_currYear.ToString());

            GUILayout.FlexibleSpace();

            // Go to Next Year
            if(EditorUtilities.ContentWidthButton(">>", _maxYear > _currYear))
            {
                ++_currYear;
                UpdateMonthlySummaryData();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the summary data.
    /// </summary>
    private void DrawSummaryData()
    {
        EditorGUILayout.BeginVertical("box");
        {
            // Column Headings
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("", GUILayout.Width(MonthNameWidth));
                EditorGUILayout.LabelField("Expenses", Styles.RightAlignedBoldWrappedLabel, GUILayout.Width(ExpensesWidth));
                EditorGUILayout.LabelField("Income", Styles.RightAlignedBoldWrappedLabel, GUILayout.Width(IncomeWidth));
                EditorGUILayout.LabelField("Net", Styles.RightAlignedBoldWrappedLabel, GUILayout.Width(NetWidth));
                EditorGUILayout.LabelField("Balance", Styles.RightAlignedBoldWrappedLabel, GUILayout.Width(BalanceWidth));
            }
            EditorGUILayout.EndHorizontal();
            
            // Draw Non-Existent Summary Data for beginning months
            int monthNumber = 1;
            int firstSummaryMonth = _monthlySummaryItems.Length > 0 ? _monthlySummaryItems[0].monthNumber : 13;
            EditorUtilities.BeginEnabled(false);
            while (monthNumber < firstSummaryMonth)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    string monthName = _monthNames[monthNumber - 1];

                    EditorGUILayout.LabelField(monthName, EditorStyles.boldLabel, GUILayout.Width(MonthNameWidth));
                    EditorGUILayout.LabelField("---", Styles.RightAlignedLabel, GUILayout.Width(ExpensesWidth));
                    EditorGUILayout.LabelField("---", Styles.RightAlignedLabel, GUILayout.Width(IncomeWidth));
                    EditorGUILayout.LabelField("---", Styles.RightAlignedLabel, GUILayout.Width(NetWidth));
                    EditorGUILayout.LabelField("---", Styles.RightAlignedLabel, GUILayout.Width(BalanceWidth));
                }
                EditorGUILayout.EndHorizontal();

                ++monthNumber;
            }
            EditorUtilities.EndEnabled();

            // Draw summary data for existing months
            for (int iMonth = 0; iMonth < _monthlySummaryItems.Length; iMonth++)
            {
                MonthlySummaryItem summaryItem = _monthlySummaryItems[iMonth];

                EditorGUILayout.BeginHorizontal();
                {
                    string monthName = _monthNames[summaryItem.monthNumber - 1];
                    Color netColor = Color.black;
                    if (summaryItem.netAmount < 0)
                    {
                        netColor = Color.red;
                    }
                    else if (summaryItem.netAmount > 0)
                    {
                        netColor = Color.green;
                    }

                    EditorGUILayout.LabelField(monthName, EditorStyles.boldLabel, GUILayout.Width(MonthNameWidth));
                    EditorGUILayout.LabelField(summaryItem.totalExpenses.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(ExpensesWidth));
                    EditorGUILayout.LabelField(summaryItem.totalIncome.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(IncomeWidth));
                    EditorUtilities.BeginForegroundColor(netColor);
                    EditorGUILayout.LabelField(summaryItem.netAmount.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(NetWidth));
                    EditorUtilities.EndForegroundColor();
                    EditorGUILayout.LabelField(summaryItem.balance.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(BalanceWidth));
                }
                EditorGUILayout.EndHorizontal();

                ++monthNumber;
            }

            // Draw Non-Existent Summary Data for ending months
            EditorUtilities.BeginEnabled(false);
            while (monthNumber < 13)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    string monthName = _monthNames[monthNumber - 1];

                    EditorGUILayout.LabelField(monthName, GUILayout.Width(MonthNameWidth));
                    EditorGUILayout.LabelField("---", Styles.RightAlignedLabel, GUILayout.Width(ExpensesWidth));
                    EditorGUILayout.LabelField("---", Styles.RightAlignedLabel, GUILayout.Width(IncomeWidth));
                    EditorGUILayout.LabelField("---", Styles.RightAlignedLabel, GUILayout.Width(NetWidth));
                    EditorGUILayout.LabelField("---", Styles.RightAlignedLabel, GUILayout.Width(BalanceWidth));
                }
                EditorGUILayout.EndHorizontal();

                ++monthNumber;
            }
            EditorUtilities.EndEnabled();

            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Draws the months for the current year.
    /// </summary>
    private void DrawMonths()
    {
        MonthlyBudget firstMB = _profile.Budget.MonthlyBudgets.First(mb => mb.Year == _currYear);
        MonthlyBudget lastMB = _profile.Budget.MonthlyBudgets.Last(mb => mb.Year == _currYear);

        // Draw the months in a 4x3 grid
        for (int iRow = 0; iRow < 4; ++iRow)
        {
            EditorGUILayout.BeginHorizontal();
            {
                for (int iCol = 0; iCol < 3; ++iCol)
                {
                    int month = iRow * 3 + iCol + 1;
                    bool isNextBudget = month == lastMB.Month + 1;
                    bool enabled = (month >= firstMB.Month && month <= lastMB.Month) || isNextBudget;

                    if (MonthButton(month, enabled, isNextBudget, 100f))
                    {
                        if (isNextBudget)
                        {
                            AddNextMonthlyBudget();
                            UpdateMonthlySummaryData();
                        }
                        else
                        {
                            MonthlyBudget monthlyBudget = _profile.Budget.MonthlyBudgets.Find(mb => mb.Year == _currYear && mb.Month == month);
                            _monthlyBudgetController = new MonthlyBudgetController(monthlyBudget, _profile);
                        }
                    }

                    if (iCol < 2)
                    {
                        GUILayout.Space(16f);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (iRow < 3)
            {
                GUILayout.Space(16f);
            }
        }

        // If the current year is filled up, show a button
        // to add another monthly budget for the next year.
        if (_maxYear == _currYear && lastMB.Month == 12)
        {
            GUILayout.Space(16f);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(150f);

                if (MonthButton(1, true, true, 50f))
                {
                    AddNextMonthlyBudget();
                    UpdateMonthlySummaryData();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// Adds the next monthly budget to the profile.
    /// </summary>
    private void AddNextMonthlyBudget()
    {
        _profile.Budget.AddNextMonthlyBudget(_profile);
        _maxYear = _profile.Budget.MonthlyBudgets[_profile.Budget.MonthlyBudgets.Count - 1].Year;
        _currYear = _maxYear;
    }

    /// <summary>
    /// Draws a button for the specified month.
    /// </summary>
    /// <param name="month">Month number (1-12).</param>
    /// <param name="enabled">Whether or not the button is enabled.</param>
    /// <param name="next">Whether or not this represents the next monthly budget to create.</param>
    /// <param name="size">Size of the button (width and height).</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    private bool MonthButton(int month, bool enabled, bool next, float size)
    {
        string text = next ? "+" : enabled ? _monthNames[month - 1] : "";
        Color color = next ? new Color(0.75f, 1.0f, 0.75f) : Color.white;
        return EditorUtilities.Button(text, color, enabled, GUILayout.Width(size), GUILayout.Height(size));
    }


    /// <summary>
    /// Summary data for an individual month.
    /// </summary>
    private class MonthlySummaryItem
    {
        /// <summary>
        /// The number of the month this corresponds to (1 to 12).
        /// </summary>
        public int monthNumber;

        /// <summary>
        /// Total expenses for the month.
        /// </summary>
        public decimal totalExpenses;

        /// <summary>
        /// Total income for the month.
        /// </summary>
        public decimal totalIncome;

        /// <summary>
        /// Net amount of expenses and income.
        /// </summary>
        public decimal netAmount;

        /// <summary>
        /// Balance up through this month.
        /// </summary>
        public decimal balance;
    }

    /// <summary>
    /// Updates the monthly summary data.
    /// </summary>
    private void UpdateMonthlySummaryData()
    {
        MonthlyBudget[] currentYearMonthlyBudgets = _profile.Budget.MonthlyBudgets.Where(mb => mb.Year == _currYear).ToArray();
        _monthlySummaryItems = currentYearMonthlyBudgets.Select<MonthlyBudget, MonthlySummaryItem>(GenerateSummaryItem).ToArray();
    }

    /// <summary>
    /// Generates a monthly summary item for the given monthly budget.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>A monthly summary item created from the given monthly budget.</returns>
    private MonthlySummaryItem GenerateSummaryItem(MonthlyBudget monthlyBudget)
    {
        decimal expenses = BudgetUtilities.GetTotalExpenses(_profile, monthlyBudget);
        decimal income = BudgetUtilities.GetTotalIncome(_profile, monthlyBudget);

        return new MonthlySummaryItem()
        {
            monthNumber = monthlyBudget.Month,
            totalExpenses = expenses,
            totalIncome = income,
            netAmount = expenses + income,
            balance = BudgetUtilities.GetBalance(_profile, monthlyBudget)
        };
    }
}
