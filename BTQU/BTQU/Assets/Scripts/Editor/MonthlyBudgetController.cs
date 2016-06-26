using System;
using System.Linq;
using BTQ;
using BTQLib;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

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

    private Transaction[] _incomeTransactions;

    private Transaction[] _expenseTransactions;

    private decimal _totalIncome;

    private Dictionary<string, decimal> _expensesPerCategory;

    private Dictionary<string, decimal> _incomePerCategory;

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

    private void Initialize()
    {
        _incomeTransactions = GetIncomeTransactions(_monthlyBudget.Month, _monthlyBudget.Year);
        _expenseTransactions = GetExpenseTransactions(_monthlyBudget.Month, _monthlyBudget.Year);

        _expensesPerCategory = new Dictionary<string, decimal>();
        _incomePerCategory = new Dictionary<string, decimal>();
        foreach (string category in _profile.Categories.PrimaryCategories)
        {
            _expensesPerCategory.Add(category, GetExpenses(category));
            _incomePerCategory.Add(category, GetIncome(category));
        }

        _totalIncome = _incomeTransactions.Sum(t => t.Amount);
    }

    /// <summary>
    /// Draws the monthly budget.
    /// </summary>
    public void DrawView()
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

        EditorGUILayout.BeginHorizontal();
        {
            DrawCategories();
        }
        EditorGUILayout.EndHorizontal();
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
                        _incomePerCategory[category] = GetIncome(category);
                    }

                    EditorUtilities.ContentWidthLabel("%");
                }
                EditorGUILayout.EndHorizontal();
            }

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
        }
        EditorGUILayout.EndVertical();

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

                    decimal previousBalance = 0;//GetPrevousBalance(category);
                    decimal expenses = _expensesPerCategory[category];
                    decimal income = _incomePerCategory[category];
                    decimal currentBalance = previousBalance + expenses + income;

                    EditorGUILayout.LabelField(previousBalance.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
                    EditorGUILayout.LabelField(expenses.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
                    EditorGUILayout.LabelField(income.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
                    EditorGUILayout.LabelField(currentBalance.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
                }
                EditorGUILayout.EndHorizontal();
            }
            /*
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
            */
        }
        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();
    }

    private decimal GetExpenses(string category)
    {
        Transaction[] transactions = GetTransactions(category, _monthlyBudget.Month, _monthlyBudget.Year);
        transactions = transactions.Where(t => t.Amount < 0).ToArray();
        return transactions.Sum(t => t.Amount);
    }

    private decimal GetIncome(string category)
    {
        float percentage = _monthlyBudget.GetPercentage(category);
        return _totalIncome * (decimal)percentage;
    }

    private Transaction[] GetTransactions(string category, int month, int year)
    {
        return _profile.Accounts.SelectMany(a => a.Transactions)
                                .Where(t => t.Category.StartsWith(category))
                                .Where(t => t.Date.Year == year && t.Date.Month == month)
                                .ToArray();
    }

    private Transaction[] GetIncomeTransactions(int month, int year)
    {
        return _profile.Accounts.SelectMany(a => a.Transactions)
                                .Where(t => t.Date.Year == year && t.Date.Month == month)
                                .Where(t => t.Amount > 0)
                                .ToArray();
    }

    private Transaction[] GetExpenseTransactions(int month, int year)
    {
        return _profile.Accounts.SelectMany(a => a.Transactions)
                                .Where(t => t.Date.Year == year && t.Date.Month == month)
                                .Where(t => t.Amount < 0)
                                .ToArray();
    }
}
