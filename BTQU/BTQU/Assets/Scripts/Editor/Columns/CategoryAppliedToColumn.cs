using BTQLib;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Defines the column for displaying the category an income transaction
/// is applied to if it is applied to a specific category.
/// </summary>
public class CategoryAppliedToColumn : TransactionColumn
{
    /// <summary>
    /// Reference to the transaction categories for an account.
    /// </summary>
    private Categories _categories;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="categories">An account's transaction categories.</param>
    public CategoryAppliedToColumn(Categories categories)
    {
        _categories = categories;
    }

    /// <summary>
    /// The display name of the column.
    /// </summary>
    public override string DisplayName { get { return "Applied Category"; } }

    /// <summary>
    /// Draws the category the given income transaction applies to.
    /// </summary>
    /// <param name="transaction">Transaction to draw.</param>
    public override void Draw(Transaction transaction)
    {
        if (transaction.Amount < 0)
        {
            GUI.enabled = false;
            EditorGUILayout.TextField("!!! Non-Income !!!");
            GUI.enabled = true;
            return;
        }

        EditorGUILayout.BeginHorizontal(GUILayout.Width(Width));
        {
            if (transaction.AppliedState != AppliedState.ApplyToCategory)
            {
                GUI.enabled = false;
                EditorGUILayout.TextField("");
                GUI.enabled = true;
            }
            else
            {
                string prevCategory = transaction.AppliedToCategory;
                string currCategory = EditorGUILayout.TextField(prevCategory);
                if (currCategory != null)
                {
                    string primaryCategory = currCategory;
                    transaction.AppliedToCategory = primaryCategory;
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
