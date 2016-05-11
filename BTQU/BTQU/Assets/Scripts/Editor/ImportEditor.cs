using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor dispaying transactions to import from file.
/// Serves as a preview of the transactions before import
/// is carried out.
/// </summary>
public class ImportEditor : EditorWindow
{
    /// <summary>
    /// Credit account records to import.
    /// </summary>
    private CreditRecord[] _creditRecords;

    /// <summary>
    /// Debit account records to import.
    /// </summary>
    private DebitRecord[] _debitRecords;

    /// <summary>
    /// Records scroll position.
    /// </summary>
    private Vector2 _scrollPos;

    /// <summary>
    /// Opens the editor with the given credit account records.
    /// </summary>
    /// <param name="records">Records to open with.</param>
    public static void Open(CreditRecord[] records)
    {
        ImportEditor editor = EditorWindow.GetWindow<ImportEditor>();
        editor.Initialize(records);
    }

    /// <summary>
    /// Opens the editor with the given debit account records.
    /// </summary>
    /// <param name="records">Records to open with.</param>
    public static void Open(DebitRecord[] records)
    {
        ImportEditor editor = EditorWindow.GetWindow<ImportEditor>();
        editor.Initialize(records);
    }

    /// <summary>
    /// Initializes the editor with the given credit account records.
    /// </summary>
    /// <param name="records">Records to initialize to.</param>
    private void Initialize(CreditRecord[] records)
    {
        _debitRecords = null;
        _creditRecords = records;
    }

    /// <summary>
    /// Initializes the editor with the given debit account records.
    /// </summary>
    /// <param name="records">Records to initialize to.</param>
    private void Initialize(DebitRecord[] records)
    {
        _creditRecords = null;
        _debitRecords = records;
    }

    /// <summary>
    /// Draws the editor GUI.
    /// </summary>
    private void OnGUI()
    {
        if(_creditRecords == null && _debitRecords == null)
        {
            Close();
            return;
        }

        DrawCreditRecords();
        DrawDebitRecords();
    }

    /// <summary>
    /// Draws the credit records if they exist.
    /// </summary>
    private void DrawCreditRecords()
    {
        if(_creditRecords == null)
        {
            return;
        }

        _scrollPos =
        EditorGUILayout.BeginScrollView(_scrollPos);
        {
            foreach (CreditRecord record in _creditRecords)
            {
                DrawCreditRecord(record);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Draws the debit records if they exist.
    /// </summary>
    private void DrawDebitRecords()
    {
        if(_debitRecords == null)
        {
            return;
        }

        _scrollPos =
        EditorGUILayout.BeginScrollView(_scrollPos);
        {
            foreach (DebitRecord record in _debitRecords)
            {
                DrawDebitRecord(record);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Draws the given credit record.
    /// </summary>
    /// <param name="record">Record to draw.</param>
    private void DrawCreditRecord(CreditRecord record)
    {
        EditorGUILayout.BeginHorizontal("box");
        {
            EditorGUILayout.LabelField(record.type.ToString(), GUILayout.Width(60.0f));
            EditorGUILayout.LabelField(record.transactionDate.ToString("MM/dd/yyyy"), GUILayout.Width(100.0f));
            EditorGUILayout.LabelField(record.postDate.ToString("MM/dd/yyyy"), GUILayout.Width(100.0f));
            EditorGUILayout.LabelField(record.description, GUILayout.Width(200.0f));
            EditorGUILayout.LabelField(record.amount.ToString("C2"), GUILayout.Width(100.0f));
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the given debit record.
    /// </summary>
    /// <param name="record">Record to draw.</param>
    private void DrawDebitRecord(DebitRecord record)
    {
        EditorGUILayout.BeginHorizontal("box");
        {
            EditorGUILayout.LabelField(record.type.ToString(), GUILayout.Width(60.0f));
            EditorGUILayout.LabelField(record.postDate.ToString("MM/dd/yyyy"), GUILayout.Width(100.0f));
            EditorGUILayout.LabelField(record.description, GUILayout.Width(500.0f));
            EditorGUILayout.LabelField(record.amount.ToString("C2"), GUILayout.Width(100.0f));
            EditorGUILayout.LabelField(record.checkOrSlipNo != null ? record.checkOrSlipNo.ToString() : "", GUILayout.Width(50.0f));
        }
        EditorGUILayout.EndHorizontal();
    }
}
