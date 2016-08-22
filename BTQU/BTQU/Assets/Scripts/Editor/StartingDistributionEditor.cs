using BTQ;
using BTQLib;
using System;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// Starting distribution editor.
/// </summary>
public class StartingDistributionEditor : EditorWindow
{
    /// <summary>
    /// Account to edit starting distribution.
    /// </summary>
    private Account _account;

    /// <summary>
    /// The categories to distribute to.
    /// </summary>
    private Categories _categories;


    /// <summary>
    /// Creates the editor.
    /// </summary>
    /// <param name="account">Account with starting distribution.</param>
    /// <param name="categories">Categories for the distribution.</param>
    public static void Create(Account account, Categories categories)
    {
        StartingDistributionEditor editor = GetWindow<StartingDistributionEditor>();
        editor._account = account;
        editor._categories = categories;
    }

    /// <summary>
    /// Draws the GUI.
    /// </summary>
    private void OnGUI()
    {
        if(_account == null)
        {
            return;
        }

        EditorUtilities.BeginHorizontalCentering();
        {
            EditorGUILayout.BeginVertical("box");
            {
                // Starting Balance
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Starting Balance", EditorStyles.boldLabel);

                    EditorGUILayout.LabelField(_account.StartingBalance.ToString("C2"), Styles.RightAlignedLabel);
                }
                EditorGUILayout.EndHorizontal();

                // Category Distributions
                decimal amountDistributed = 0;
                List<string> primaryCategories = _categories.GetPrimaryCategories();
                foreach (string category in primaryCategories)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (!_account.StartingDistribution.ContainsKey(category))
                        {
                            _account.StartingDistribution.Add(category, 0);
                        }
                        
                        EditorGUILayout.LabelField(category);

                        _account.StartingDistribution[category] = TransactionUtilities.AmountField(_account.StartingDistribution[category]);
                        
                        amountDistributed += _account.StartingDistribution[category];
                    }
                    EditorGUILayout.EndHorizontal();
                }

                // Undistributed Amount
                decimal undistributedAmount = _account.StartingBalance - amountDistributed;
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Undistributed");

                    EditorGUILayout.LabelField(undistributedAmount.ToString("C2"), Styles.RightAlignedLabel);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        EditorUtilities.EndHorizontalCentering();
    }
}
