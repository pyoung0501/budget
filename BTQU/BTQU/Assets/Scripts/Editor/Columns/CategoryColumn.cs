using BTQLib;
using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Defines the category column for drawing the category field of transactions.
/// </summary>
public class CategoryColumn : TransactionColumn
{
    /// <summary>
    /// Reference to the budget categories for an account.
    /// </summary>
    private BudgetCategories _budgetCategories;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="budgetCategories">An account's budget categories.</param>
    public CategoryColumn(BudgetCategories budgetCategories)
    {
        _budgetCategories = budgetCategories;
    }

    /// <summary>
    /// The display name of the column.
    /// </summary>
    public override string DisplayName { get { return "Category"; } }

    /// <summary>
    /// Draws the category field of the given transaction.
    /// </summary>
    /// <param name="transaction">Transaction to draw.</param>
    public override void Draw(Transaction transaction)
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(Width));
        {
            string prevCategory = transaction.Category;
            string currCategory = EditorGUILayout.TextField(prevCategory);
            if (currCategory != null)
            {
                // Split category into primary and secondary
                string primaryCategory;
                string secondaryCategory;
                GetPrimaryAndSecondaryCategories(currCategory, out primaryCategory, out secondaryCategory);

                // If a secondary category is not specified, set to the primary.
                if (string.IsNullOrEmpty(secondaryCategory))
                {
                    transaction.Category = primaryCategory;
                }
                else // Otherwise, set to both.
                {
                    transaction.Category = string.Format("{0}:{1}", primaryCategory, secondaryCategory);
                }

                // Show the "Add Category" button if
                //  primaryCategory is non-empty
                //  and primaryCategory does not yet exist
                //  or secondaryCategory is non-empty and secondaryCategory does not yet exist
                bool showButton = !string.IsNullOrEmpty(primaryCategory)
                              && (!_budgetCategories.PrimaryCategoryExists(primaryCategory)
                               || (secondaryCategory != "" && !_budgetCategories.SecondaryCategoryExists(primaryCategory, secondaryCategory)));
                if (showButton)
                {
                    if (EditorUtilities.ContentWidthButton("+"))
                    {
                        // Add primary if it doesn't exist
                        if (!_budgetCategories.PrimaryCategoryExists(primaryCategory))
                        {
                            _budgetCategories.AddPrimaryCategory(primaryCategory);
                        }

                        // Add secondary if it doesn't exist and it is valid
                        if (secondaryCategory != "" && !_budgetCategories.SecondaryCategoryExists(primaryCategory, secondaryCategory))
                        {
                            _budgetCategories.AddSecondaryCategory(primaryCategory, secondaryCategory);
                        }
                    }
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Outputs the primary and secondary categories from the given combined category.
    /// </summary>
    /// <param name="combinedCategory">Combined category.</param>
    /// <param name="primaryCategory">Output primary category.</param>
    /// <param name="secondaryCategory">Output secondary category.</param>
    private void GetPrimaryAndSecondaryCategories(string combinedCategory, out string primaryCategory, out string secondaryCategory)
    {
        string[] categories = combinedCategory.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
        if (categories.Length == 0)
        {
            primaryCategory = "";
            secondaryCategory = "";
        }
        else if (categories.Length == 1)
        {
            primaryCategory = categories[0];
            secondaryCategory = "";
        }
        else
        {
            primaryCategory = categories[0];
            secondaryCategory = categories[1];
        }
    }
}
