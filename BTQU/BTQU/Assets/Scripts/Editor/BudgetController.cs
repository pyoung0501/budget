using BTQLib;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;

/// <summary>
/// Controller for the budget.
/// </summary>
public class BudgetController
{
    /// <summary>
    /// Profile with budget.
    /// </summary>
    private Profile _profile;

    /// <summary>
    /// The current year being viewed.
    /// </summary>
    private int _currYear;

    /// <summary>
    /// The minimum year represented by the budget.
    /// </summary>
    private int _minYear;

    /// <summary>
    /// The maximum year represented by the budget.
    /// </summary>
    private int _maxYear;

    /// <summary>
    /// Scroll view position.
    /// </summary>
    private Vector3 _scrollPos;

    /// <summary>
    /// Controller for the selected monthly budget.
    /// </summary>
    private MonthlyBudgetController _monthlyBudgetController;

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
    /// <param name="profile">Profile.</param>
    public BudgetController(Profile profile)
    {
        _profile = profile;

        if(_profile.Budget.Created)
        {
            _minYear = _profile.Budget.MonthlyBudgets[0].Year;
            _maxYear = _profile.Budget.MonthlyBudgets[_profile.Budget.MonthlyBudgets.Count - 1].Year;
            _currYear = _maxYear;
        }
    }

    /// <summary>
    /// Draws the budget view.
    /// </summary>
    public void DrawView()
    {
        if(_currYear == 0)
        {
            return;
        }

        // Draw Selected Monthly Budget
        if(_monthlyBudgetController != null)
        {
            EditorGUILayout.BeginHorizontal("box");
            {
                if(EditorUtilities.ContentWidthButton("< Budget"))
                {
                    _monthlyBudgetController = null;
                }

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            if (_monthlyBudgetController != null)
            {
                _monthlyBudgetController.DrawView();
            }

            return;
        }

        _scrollPos =
        EditorGUILayout.BeginScrollView(_scrollPos);
        {
            EditorUtilities.BeginHorizontalCentering();
            EditorGUILayout.BeginVertical();
            {
                DrawMonths();
            }
            EditorGUILayout.EndVertical();
            EditorUtilities.EndHorizontalCentering();
        }
        EditorGUILayout.EndScrollView();

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal("box");
        {
            // Go to Previous Year
            if(EditorUtilities.ContentWidthButton("<<", _minYear < _currYear))
            {
                --_currYear;
            }

            GUILayout.FlexibleSpace();

            // Current Year
            EditorUtilities.ContentWidthLabel(_currYear.ToString());

            GUILayout.FlexibleSpace();

            // Go to Next Year
            if(EditorUtilities.ContentWidthButton(">>", _maxYear > _currYear))
            {
                ++_currYear;
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the months for the current year.
    /// </summary>
    private void DrawMonths()
    {
        MonthlyBudget firstMB = _profile.Budget.MonthlyBudgets.First(mb => mb.Year == _currYear);
        MonthlyBudget lastMB = _profile.Budget.MonthlyBudgets.Last(mb => mb.Year == _currYear);

        // Draw the months in a 4x3 grid
        for (int iRow = 0; iRow < 4; ++iRow)
        {
            EditorGUILayout.BeginHorizontal();
            {
                for (int iCol = 0; iCol < 3; ++iCol)
                {
                    int month = iRow * 3 + iCol + 1;
                    bool isNextBudget = month == lastMB.Month + 1;
                    bool enabled = (month >= firstMB.Month && month <= lastMB.Month) || isNextBudget;

                    if (MonthButton(month, enabled, isNextBudget, 100f))
                    {
                        if (isNextBudget)
                        {
                            AddNextMonthlyBudget();
                        }
                        else
                        {
                            MonthlyBudget monthlyBudget = _profile.Budget.MonthlyBudgets.Find(mb => mb.Year == _currYear && mb.Month == month);
                            _monthlyBudgetController = new MonthlyBudgetController(monthlyBudget);
                        }
                    }

                    if (iCol < 2)
                    {
                        GUILayout.Space(16f);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (iRow < 3)
            {
                GUILayout.Space(16f);
            }
        }

        // If the current year is filled up, show a button
        // to add another monthly budget for the next year.
        if (_maxYear == _currYear && lastMB.Month == 12)
        {
            GUILayout.Space(16f);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(150f);

                if (MonthButton(1, true, true, 50f))
                {
                    AddNextMonthlyBudget();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// Adds the next monthly budget to the profile.
    /// </summary>
    private void AddNextMonthlyBudget()
    {
        _profile.Budget.AddNextMonthlyBudget(_profile);
        _maxYear = _profile.Budget.MonthlyBudgets[_profile.Budget.MonthlyBudgets.Count - 1].Year;
        _currYear = _maxYear;
    }

    /// <summary>
    /// Draws a button for the specified month.
    /// </summary>
    /// <param name="month">Month number (1-12).</param>
    /// <param name="enabled">Whether or not the button is enabled.</param>
    /// <param name="next">Whether or not this represents the next monthly budget to create.</param>
    /// <param name="size">Size of the button (width and height).</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    private bool MonthButton(int month, bool enabled, bool next, float size)
    {
        string text = next ? "+" : enabled ? _monthNames[month - 1] : "";
        Color color = next ? new Color(0.75f, 1.0f, 0.75f) : Color.white;
        return EditorUtilities.Button(text, color, enabled, GUILayout.Width(size), GUILayout.Height(size));
    }
}
