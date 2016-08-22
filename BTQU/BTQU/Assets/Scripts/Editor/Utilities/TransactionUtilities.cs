using System;
using UnityEditor;
using UnityEngine;

namespace BTQ
{
    public static class TransactionUtilities
    {
        /// <summary>
        /// Draws and amount field.
        /// </summary>
        /// <param name="amount">Amount.</param>
        /// <param name="options">Layout options.</param>
        /// <returns>The edited amount.</returns>
        public static decimal AmountField(decimal amount, params GUILayoutOption[] options)
        {
            string prevVal = amount.ToString("C2");
            string newVal = EditorGUILayout.TextField(prevVal, Styles.RightAlignedTextField, options);

            if (newVal != prevVal)
            {
                newVal = newVal.Replace("$", "");

                decimal parsedAmount;
                if (decimal.TryParse(newVal, out parsedAmount))
                {
                    parsedAmount = Math.Round(parsedAmount, 2, MidpointRounding.ToEven);
                    amount = parsedAmount;
                }
            }

            return amount;
        }
    }
}
