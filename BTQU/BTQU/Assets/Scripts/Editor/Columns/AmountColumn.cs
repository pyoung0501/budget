using BTQ;
using BTQLib;
using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Defines the amount column for drawing the amount field of transactions.
/// </summary>
public class AmountColumn : TransactionColumn
{
    /// <summary>
    /// Display name of the column.
    /// </summary>
    public override string DisplayName { get { return "Amount"; } }
    
    /// <summary>
    /// Draws the amount of the given transaction.
    /// </summary>
    /// <param name="transaction">Transaction to draw.</param>
    public override void Draw(Transaction transaction)
    {
        transaction.Amount = TransactionUtilities.AmountField(transaction.Amount, GUILayout.Width(Width));
    }
}
