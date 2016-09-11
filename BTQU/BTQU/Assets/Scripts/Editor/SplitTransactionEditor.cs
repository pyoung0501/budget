using BTQ;
using BTQLib;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// Editor for splitting up a transaction into multiple categories.
/// </summary>
public class SplitTransactionEditor : EditorWindow
{
    /// <summary>
    /// Transaction being split.
    /// </summary>
    private Transaction _transaction;

    /// <summary>
    /// The categories to split between.
    /// </summary>
    private Categories _categories;
    
    /// <summary>
    /// Array of category names used in the category popups.
    /// </summary>
    private string[] _categoryNames;


    /// <summary>
    /// Creates the editor.
    /// </summary>
    /// <param name="transaction">Transaction to split.</param>
    /// <param name="categories">Categories.</param>
    public static void Create(Transaction transaction, Categories categories)
    {
        SplitTransactionEditor editor = GetWindow<SplitTransactionEditor>();
        editor._transaction = transaction;
        editor._categories = categories;

        editor.Refresh();
    }

    /// <summary>
    /// Refreshes the editor.
    /// </summary>
    private void Refresh()
    {
        _categoryNames = _categories.PrimaryCategories.ToArray();
        Array.Sort(_categoryNames);
    }

    /// <summary>
    /// Draws the GUI.
    /// </summary>
    private void OnGUI()
    {
        if (_transaction == null)
        {
            return;
        }

        DrawSplitEntries();
    }

    /// <summary>
    /// Draws the split transaction entries.
    /// </summary>
    private void DrawSplitEntries()
    {
        EditorGUILayout.BeginVertical("box");
        {
            // Column Headings
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Description", EditorStyles.boldLabel, GUILayout.Width(300.0f));
                EditorGUILayout.LabelField("Category", EditorStyles.boldLabel, GUILayout.Width(200.0f));
                EditorGUILayout.LabelField("Amount", Styles.RightAlignedBoldWrappedLabel, GUILayout.Width(100.0f));
            }
            EditorGUILayout.EndHorizontal();

            // Draw the split entries
            SplitEntry entryToRemove = null;
            foreach (SplitEntry entry in _transaction.SplitEntries)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    entry.Description = EditorGUILayout.TextField(entry.Description, GUILayout.Width(300.0f));
                    entry.Category = TransactionUtilities.CategoryField(entry.Category, _categories, GUILayout.Width(200.0f));
                    entry.Amount = TransactionUtilities.AmountField(entry.Amount, GUILayout.Width(100.0f));

                    if(EditorUtilities.ContentWidthButton("-"))
                    {
                        entryToRemove = entry;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            if(entryToRemove != null)
            {
                _transaction.SplitEntries.Remove(entryToRemove);
                entryToRemove = null;
            }

            EditorGUILayout.BeginHorizontal();
            {
                if (_transaction.SplitEntries.Count == 0)
                {
                    EditorUtilities.BeginEnabled(false);
                    EditorGUILayout.LabelField("(No entries yet)");
                    EditorUtilities.EndEnabled();
                }

                GUILayout.FlexibleSpace();

                if (EditorUtilities.ContentWidthButton("+"))
                {
                    _transaction.SplitEntries.Add(new SplitEntry());
                }
            }
            EditorGUILayout.EndHorizontal();

            DrawRemainingAmount();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Draws the remaining amount not assigned yet to a category.
    /// </summary>
    private void DrawRemainingAmount()
    {
        decimal totalSplit = _transaction.SplitEntries.Sum(it => it.Amount);
        decimal totalRemaining = _transaction.Amount - totalSplit;

        EditorUtilities.BeginEnabled(false);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(305.0f);
            EditorGUILayout.LabelField("Remaining Amount", Styles.RightAlignedLabel, GUILayout.Width(200.0f));
            TransactionUtilities.AmountField(totalRemaining, GUILayout.Width(100.0f));
            GUILayout.Space(16.0f);
        }
        EditorGUILayout.EndHorizontal();
        EditorUtilities.EndEnabled();
    }
}
