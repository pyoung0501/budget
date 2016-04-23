using BTQLib;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Defines the payee column for drawing the Payee field of transactions.
/// </summary>
public class PayeeColumn : TransactionColumn
{
    /// <summary>
    /// The display name of the column.
    /// </summary>
    public override string DisplayName { get { return "Payee"; } }

    /// <summary>
    /// Draws the payee field of the given transaction.
    /// </summary>
    /// <param name="transaction">Transaction to draw.</param>
    public override void Draw(Transaction transaction)
    {
        transaction.Payee = EditorGUILayout.TextField(transaction.Payee, GUILayout.Width(Width));
    }
}
