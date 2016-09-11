using BTQ;
using BTQLib;
using BTQLib.Util;
using UnityEngine;

/// <summary>
/// Defines the amount column for drawing the amount of transactions
/// applying to a particular category.
/// </summary>
public class CategoryAmountColumn : TransactionColumn
{
    /// <summary>
    /// Category of the column.
    /// </summary>
    public string PrimaryCategory { get; set; }

    /// <summary>
    /// Display name of the column.
    /// </summary>
    public override string DisplayName { get { return "Amount"; } }

    /// <summary>
    /// Draws the amount of the given transaction applicable to the primary category.
    /// This is drawn non-editable.
    /// </summary>
    /// <param name="transaction">Transaction to draw.</param>
    public override void Draw(Transaction transaction)
    {
        decimal amount = Transactions.AmountForPrimaryCategory(transaction, PrimaryCategory);
        TransactionUtilities.AmountField(amount, GUILayout.Width(Width));
    }
}
