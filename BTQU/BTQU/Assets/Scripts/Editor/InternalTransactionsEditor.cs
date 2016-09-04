using BTQ;
using BTQLib;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// Internal transactions editor.
/// Manages the internal transactions for a monthly budget.
/// </summary>
public class InternalTransactionsEditor : EditorWindow
{
    /// <summary>
    /// Profile containing the monthly budget.
    /// </summary>
    private Profile _profile;

    /// <summary>
    /// The monthly budget containing the internal transactions.
    /// </summary>
    private MonthlyBudget _monthlyBudget;

    /// <summary>
    /// The categories to transfer between.
    /// </summary>
    private Categories _categories;

    /// <summary>
    /// Mapping of balances for each category.
    /// </summary>
    Dictionary<string, decimal> _balancePerCategory;

    /// <summary>
    /// The internal transaction being edited for adding.
    /// </summary>
    private InternalTransaction _internalTransactionToAdd = new InternalTransaction();

    /// <summary>
    /// Array of category names used in the category popups.
    /// </summary>
    private string[] _categoryNames;


    /// <summary>
    /// Creates the editor.
    /// </summary>
    /// <param name="account">Account with starting distribution.</param>
    /// <param name="categories">Categories for the distribution.</param>
    public static void Create(Profile profile, MonthlyBudget monthlyBudget)
    {
        InternalTransactionsEditor editor = GetWindow<InternalTransactionsEditor>();
        editor._profile = profile;
        editor._monthlyBudget = monthlyBudget;
        editor._categories = profile.Categories;

        editor.Refresh();
    }

    /// <summary>
    /// Refreshes the editor.
    /// </summary>
    private void Refresh()
    {
        _balancePerCategory = GetBalancePerCategory(_monthlyBudget);
        _categoryNames = _balancePerCategory.Keys.ToArray();
        Array.Sort(_categoryNames);
    }

    /// <summary>
    /// Gets the balance per category in the given monthly budget.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    /// <returns>The balance per category.</returns>
    private Dictionary<string, decimal> GetBalancePerCategory(MonthlyBudget monthlyBudget)
    {
        Dictionary<string, decimal> balancePerCategory = new Dictionary<string, decimal>();
        foreach (string category in _categories.PrimaryCategories)
        {
            balancePerCategory.Add(category, BudgetUtilities.GetBalance(_profile, _monthlyBudget, category));
        }

        return balancePerCategory;
    }

    /// <summary>
    /// Draws the GUI.
    /// </summary>
    private void OnGUI()
    {
        if (_monthlyBudget == null)
        {
            return;
        }

        DrawCategoryBalances();
        DrawInternalTransactions();
        DrawNewInternalTransaction();
    }

    /// <summary>
    /// Draws the category balances for reference.
    /// </summary>
    private void DrawCategoryBalances()
    {
        EditorGUILayout.BeginVertical("box");
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Category", EditorStyles.boldLabel, GUILayout.Width(100.0f));
                EditorGUILayout.LabelField("Current Balance", Styles.RightAlignedBoldWrappedLabel, GUILayout.Width(80.0f));
            }
            EditorGUILayout.EndHorizontal();

            foreach (string category in _categories.PrimaryCategories)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(category, GUILayout.Width(100.0f));

                    decimal currentBalance = _balancePerCategory[category];
                    EditorGUILayout.LabelField(currentBalance.ToString("C2"), Styles.RightAlignedLabel, GUILayout.Width(80.0f));
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Draws the list of internal transactions.
    /// </summary>
    private void DrawInternalTransactions()
    {
        EditorGUILayout.BeginVertical("box");
        {
            foreach (InternalTransaction it in _monthlyBudget.InternalTransactions)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorUtilities.ContentWidthLabel("From");
                    EditorUtilities.ContentWidthLabel(it.fromCategory, EditorStyles.boldLabel);
                    EditorUtilities.ContentWidthLabel("to");
                    EditorUtilities.ContentWidthLabel(it.toCategory, EditorStyles.boldLabel);
                    EditorUtilities.ContentWidthLabel("for");
                    EditorUtilities.ContentWidthLabel(it.amount.ToString("C2"), EditorStyles.boldLabel);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Draws a new internal transaction which can be edited
    /// and added to the list of internal transactions.
    /// </summary>
    private void DrawNewInternalTransaction()
    {
        EditorGUILayout.BeginVertical("box");
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorUtilities.ContentWidthLabel("From");
                _internalTransactionToAdd.fromCategory = CategoryPopup(_internalTransactionToAdd.fromCategory);
                EditorUtilities.ContentWidthLabel("to");
                _internalTransactionToAdd.toCategory = CategoryPopup(_internalTransactionToAdd.toCategory);
                EditorUtilities.ContentWidthLabel("for");
                _internalTransactionToAdd.amount = TransactionUtilities.AmountField(_internalTransactionToAdd.amount);

                bool enabled = _internalTransactionToAdd.amount > 0
                            && _categoryNames.Contains(_internalTransactionToAdd.fromCategory)
                            && _categoryNames.Contains(_internalTransactionToAdd.toCategory);
                if (EditorUtilities.ContentWidthButton("Add", enabled))
                {
                    _monthlyBudget.InternalTransactions.Add(_internalTransactionToAdd);

                    _internalTransactionToAdd = new InternalTransaction();
                    Refresh();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Draws a category popup field.
    /// </summary>
    /// <param name="category">Current category.</param>
    /// <returns>Selected category.</returns>
    private string CategoryPopup(string category)
    {
        int index = Array.IndexOf(_categoryNames, category);
        index = EditorGUILayout.Popup(index, _categoryNames);

        return (index >= 0 && index < _categoryNames.Length)
             ? _categoryNames[index]
             : null;
    }
}
