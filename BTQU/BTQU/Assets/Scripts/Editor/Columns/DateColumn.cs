using BTQLib;
using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Defines the date column used in drawing transaction dates.
/// </summary>
public class DateColumn : TransactionColumn
{
    /// <summary>
    /// The display name of the column.
    /// </summary>
    public override string DisplayName { get { return "Date"; } }

    /// <summary>
    /// Draws the date field of the given transaction.
    /// </summary>
    /// <param name="transaction">Transaction to draw.</param>
    public override void Draw(Transaction transaction)
    {
        string prevDate = transaction.Date.ToString("MM/dd/yyyy");
        string currDate = EditorGUILayout.TextField(prevDate, GUILayout.Width(Width));

        if (currDate != prevDate)
        {
            DateTime newDate;
            if (DateTime.TryParse(currDate, out newDate))
            {
                transaction.Date = newDate;
            }
        }
    }
}
