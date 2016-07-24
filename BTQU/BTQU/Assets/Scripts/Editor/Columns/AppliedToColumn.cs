using BTQ;
using BTQLib;
using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Defines the column for displaying the applied to state
/// for income transactions.
/// </summary>
public class AppliedToColumn: TransactionColumn
{
    /// <summary>
    /// Display name of the column.
    /// </summary>
    public override string DisplayName { get { return "Applied To"; } }

    /// <summary>
    /// Draws the "applied to" of the given income transaction. If this is not
    /// an income transaction, an error will be drawn instead.
    /// </summary>
    /// <param name="transaction">Transaction to draw.</param>
    public override void Draw(Transaction transaction)
    {
        if(transaction.Amount < 0)
        {
            GUI.enabled = false;
            EditorGUILayout.TextField("!!! Non-Income !!!");
            GUI.enabled = true;
            return;
        }

        AppliedState prevVal = transaction.AppliedState;
        AppliedState newVal = (AppliedState)EditorGUILayout.EnumPopup(prevVal, GUILayout.Width(Width));

        if (newVal != prevVal)
        {
            transaction.AppliedState = newVal;
        }
    }
}
