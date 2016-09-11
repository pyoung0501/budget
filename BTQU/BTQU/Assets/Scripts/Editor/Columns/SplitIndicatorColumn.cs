using BTQ;
using BTQLib;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Defines a column displaying the IsSplit field of transactions.
/// </summary>
public class SplitIndicatorColumn : TransactionColumn
{
    /// <summary>
    /// Display name of the column.
    /// </summary>
    public override string DisplayName { get { return "Split"; } }
    
    /// <summary>
    /// Draws a non-editable checkbox indicating the IsSplit value
    /// of the given transaction.
    /// </summary>
    /// <param name="transaction">Transaction.</param>
    public override void Draw(Transaction transaction)
    {
        EditorGUILayout.Toggle(transaction.IsSplit, GUILayout.Width(Width));
    }
}
