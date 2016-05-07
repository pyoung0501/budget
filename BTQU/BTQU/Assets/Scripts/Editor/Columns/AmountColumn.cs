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
        string prevVal = transaction.Amount.ToString("C2");
        string newVal = EditorGUILayout.TextField(prevVal, Styles.RightAlignedTextField, GUILayout.Width(Width));
        
        if (newVal != prevVal)
        {
            newVal = newVal.Replace("$", "");

            decimal amount;
            if (decimal.TryParse(newVal, out amount))
            {
                amount = Math.Round(amount, 2, MidpointRounding.ToEven);
                transaction.Amount = amount;
            }
        }
    }
}
