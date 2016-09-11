using BTQ;
using BTQLib;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Defines the category column for drawing the category field of transactions.
/// </summary>
public class CategoryColumn : TransactionColumn
{
    /// <summary>
    /// Reference to the transaction categories for an account.
    /// </summary>
    private Categories _categories;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="categories">An account's transaction categories.</param>
    public CategoryColumn(Categories categories)
    {
        _categories = categories;
    }

    /// <summary>
    /// The display name of the column.
    /// </summary>
    public override string DisplayName { get { return "Category"; } }

    /// <summary>
    /// Draws the category field of the given transaction with button to create
    /// or edit a split in the transaction amount.
    /// </summary>
    /// <param name="transaction">Transaction to draw.</param>
    public override void Draw(Transaction transaction)
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(Width));
        {
            if (transaction.IsSplit)
            {
                EditorUtilities.BeginEnabled(false);
                EditorGUILayout.TextField("--- Split ---");
                EditorUtilities.EndEnabled();
            }
            else
            {
                transaction.Category = TransactionUtilities.CategoryField(transaction.Category, _categories);
            }

            if(EditorUtilities.ContentWidthButton("Split"))
            {
                if(transaction.SplitEntries == null)
                {
                    transaction.SplitEntries = new List<SplitEntry>();
                }

                SplitTransactionEditor.Create(transaction, _categories);
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
