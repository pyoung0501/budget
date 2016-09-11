using BTQLib;
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

        /// <summary>
        /// Draws the category field which allows specifying primary and secondary categories
        /// as well as adding those categories to the list of available categories.
        /// </summary>
        /// <param name="category">Category name (primary and secondary).</param>
        /// <param name="categories">The budget categories.</param>
        /// <param name="layoutOptions">Additional layout options.</param>
        /// <returns>The current category.</returns>
        public static string CategoryField(string category, Categories categories, params GUILayoutOption[] layoutOptions)
        {
            string prevCategory = category;
            string currCategory = EditorGUILayout.TextField(prevCategory, layoutOptions);
            if (currCategory != null)
            {
                // Split category into primary and secondary
                string primaryCategory;
                string secondaryCategory;
                GetPrimaryAndSecondaryCategories(currCategory, out primaryCategory, out secondaryCategory);

                // If a secondary category is not specified, set to the primary.
                if (string.IsNullOrEmpty(secondaryCategory))
                {
                    currCategory = primaryCategory;
                }
                else // Otherwise, set to both.
                {
                    currCategory = string.Format("{0}:{1}", primaryCategory, secondaryCategory);
                }

                // Show the "Add Category" button if
                //  primaryCategory is non-empty
                //  and primaryCategory does not yet exist
                //  or secondaryCategory is non-empty and secondaryCategory does not yet exist
                bool showButton = !string.IsNullOrEmpty(primaryCategory)
                                && (!categories.PrimaryCategoryExists(primaryCategory)
                                || (secondaryCategory != "" && !categories.SecondaryCategoryExists(primaryCategory, secondaryCategory)));
                if (showButton)
                {
                    if (EditorUtilities.ContentWidthButton("+"))
                    {
                        // Add primary if it doesn't exist
                        if (!categories.PrimaryCategoryExists(primaryCategory))
                        {
                            categories.AddPrimaryCategory(primaryCategory);
                        }

                        // Add secondary if it doesn't exist and it is valid
                        if (secondaryCategory != "" && !categories.SecondaryCategoryExists(primaryCategory, secondaryCategory))
                        {
                            categories.AddSecondaryCategory(primaryCategory, secondaryCategory);
                        }
                    }
                }
            }

            return currCategory;
        }

        /// <summary>
        /// Outputs the primary and secondary categories from the given combined category.
        /// </summary>
        /// <param name="combinedCategory">Combined category.</param>
        /// <param name="primaryCategory">Output primary category.</param>
        /// <param name="secondaryCategory">Output secondary category.</param>
        private static void GetPrimaryAndSecondaryCategories(string combinedCategory,
                                                             out string primaryCategory,
                                                             out string secondaryCategory)
        {
            string[] categories = combinedCategory.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (categories.Length == 0)
            {
                primaryCategory = "";
                secondaryCategory = "";
            }
            else if (categories.Length == 1)
            {
                primaryCategory = categories[0];
                secondaryCategory = "";
            }
            else
            {
                primaryCategory = categories[0];
                secondaryCategory = categories[1];
            }
        }
    }
}
