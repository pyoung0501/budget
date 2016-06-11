using BTQLib;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Controller for a Monthly Budget object.
/// </summary>
public class MonthlyBudgetController
{
    /// <summary>
    /// Monthly budget.
    /// </summary>
    private MonthlyBudget _monthlyBudget;

    /// <summary>
    /// Parent profile.
    /// </summary>
    private Profile _profile;

    /// <summary>
    /// List of month names in chronological order.
    /// </summary>
    private static readonly string[] _monthNames = new string[]
    {
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December",
    };

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="monthlyBudget">Monthly budget.</param>
    public MonthlyBudgetController(MonthlyBudget monthlyBudget, Profile profile)
    {
        _monthlyBudget = monthlyBudget;
        _profile = profile;
    }

    /// <summary>
    /// Draws the monthly budget.
    /// </summary>
    public void DrawView()
    {
        EditorUtilities.BeginHorizontalCentering();
        {
            EditorGUILayout.BeginHorizontal("box");
            {
                string text = string.Format("{0} {1}", _monthNames[_monthlyBudget.Month - 1], _monthlyBudget.Year);

                GUILayout.Space(16.0f);
                EditorUtilities.ContentWidthLabel(text, EditorStyles.boldLabel);
                GUILayout.Space(16.0f);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorUtilities.EndHorizontalCentering();

        EditorGUILayout.BeginHorizontal();
        {
            DrawCategories();
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the budget categories.
    /// </summary>
    private void DrawCategories()
    {
        EditorGUILayout.BeginVertical("box");
        {
            foreach(string category in _profile.Categories.PrimaryCategories)
            {
                EditorGUILayout.LabelField(category);
            }
        }
        EditorGUILayout.EndVertical();
    }
}
