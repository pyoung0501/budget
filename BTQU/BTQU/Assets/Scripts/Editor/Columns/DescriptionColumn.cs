using BTQLib;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Defines the description column used for drawing the description field of transactions.
/// </summary>
public class DescriptionColumn : TransactionColumn
{
    /// <summary>
    /// Display name of the column.
    /// </summary>
    public override string DisplayName { get { return "Description"; } }

    /// <summary>
    /// Draws the description field of the given transaction.
    /// </summary>
    /// <param name="transaction">Transaction to draw.</param>
    public override void Draw(Transaction transaction)
    {
        transaction.Description = EditorGUILayout.TextField(transaction.Description, GUILayout.Width(Width));
    }
}
