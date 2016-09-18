using System;
using System.Collections.Generic;
using System.Linq;
using BTQLib;
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
    /// Account to import transactions into.
    /// </summary>
    private Account _account;

    /// <summary>
    /// Credit account records to import.
    /// </summary>
    private CreditRecord[] _creditRecords;

    /// <summary>
    /// Debit account records to import.
    /// TODO: create a better import editor which allows the user
    /// to select the fields for import instead of rigidly defining
    /// the input format.
    /// </summary>
    private DebitRecord_20160911[] _debitRecords;

    /// <summary>
    /// Flags indicating whether a record already exists (and will be skipped over for import).
    /// </summary>
    private Dictionary<object, bool> _recordExists;

    /// <summary>
    /// Import handler to carry out the actual import.
    /// </summary>
    private Action<Transaction[]> _onImport;

    /// <summary>
    /// Records scroll position.
    /// </summary>
    private Vector2 _scrollPos;

    /// <summary>
    /// Opens the editor with the given credit account records.
    /// </summary>
    /// <param name="account">Account to import into.</param>
    /// <param name="records">Records to open with.</param>
    /// <param name="onImport">Callback to handle the imported transactions.</param>
    public static void Open(Account account, CreditRecord[] records, Action<Transaction[]> onImport)
    {
        ImportEditor editor = EditorWindow.GetWindow<ImportEditor>();
        editor.Initialize(account, records, onImport);
    }

    /// <summary>
    /// Opens the editor with the given debit account records.
    /// </summary>
    /// <param name="account">Account to import into.</param>
    /// <param name="records">Records to open with.</param>
    /// <param name="onImport">Callback to handle the imported transactions.</param>
    public static void Open(Account account, DebitRecord_20160911[] records, Action<Transaction[]> onImport)
    {
        ImportEditor editor = EditorWindow.GetWindow<ImportEditor>();
        editor.Initialize(account, records, onImport);
    }

    /// <summary>
    /// Initializes the editor with the given credit account records.
    /// </summary>
    /// <param name="account">Account to import into.</param>
    /// <param name="records">Records to initialize to.</param>
    /// <param name="onImport">Callback to handle the imported transactions.</param>
    private void Initialize(Account account, CreditRecord[] records, Action<Transaction[]> onImport)
    {
        _account = account;
        _debitRecords = null;
        _creditRecords = records;
        _onImport = onImport;

        InitializeExistingFlags(records);
    }

    /// <summary>
    /// Initializes the editor with the given debit account records.
    /// </summary>
    /// <param name="account">Account to import into.</param>
    /// <param name="records">Records to initialize to.</param>
    /// <param name="onImport">Callback to handle the imported transactions.</param>
    private void Initialize(Account account, DebitRecord_20160911[] records, Action<Transaction[]> onImport)
    {
        _account = account;
        _creditRecords = null;
        _debitRecords = records;
        _onImport = onImport;

        InitializeExistingFlags(records);
    }

    /// <summary>
    /// Initializes the existing flags for the given credit records.
    /// </summary>
    /// <param name="records">Records to initialize flags from.</param>
    private void InitializeExistingFlags(CreditRecord[] records)
    {
        _recordExists = new Dictionary<object, bool>(records.Length);

        foreach(CreditRecord record in records)
        {
            _recordExists.Add(record, _account.Transactions.Any(trans => MatchesRecord(trans, record)));
        }
    }

    /// <summary>
    /// Initializes the existing flags for the given debit records.
    /// </summary>
    /// <param name="records">Records to initialize flags from.</param>
    private void InitializeExistingFlags(DebitRecord_20160911[] records)
    {
        _recordExists = new Dictionary<object, bool>(records.Length);

        foreach (DebitRecord_20160911 record in records)
        {
            _recordExists.Add(record, _account.Transactions.Any(trans => MatchesRecord(trans, record)));
        }
    }

    /// <summary>
    /// Returns true if the given transaction and record match.
    /// </summary>
    /// <param name="transaction">Transaction.</param>
    /// <param name="record">Record.</param>
    /// <returns>True if the given transaction and record match.</returns>
    private bool MatchesRecord(Transaction transaction, CreditRecord record)
    {
        ImportData importData = transaction.ImportData;
        return record.amount == transaction.Amount
            && record.postDate == importData.PostDate
            && record.transactionDate == importData.TransactionDate
            && record.type.ToString() == importData.TransactionType
            && record.description == importData.Description;
    }

    /// <summary>
    /// Returns true if the given transaction and record match.
    /// </summary>
    /// <param name="transaction">Transaction.</param>
    /// <param name="record">Record.</param>
    /// <returns>True if the given transaction and record match.</returns>
    private bool MatchesRecord(Transaction transaction, DebitRecord_20160911 record)
    {
        ImportData importData = transaction.ImportData;
        return transaction.Amount == record.amount
            && importData.PostDate == record.postDate
            && importData.TransactionType == record.type.ToString()
            && importData.TransactionDate == null
            && importData.Description == record.description
            && importData.CheckOrSlipNo == record.checkOrSlipNo;
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

        EditorUtilities.BeginHorizontalCentering();
        EditorUtilities.ContentWidthLabel("Preview", EditorStyles.boldLabel);
        EditorUtilities.EndHorizontalCentering();

        DrawCreditRecords();
        DrawDebitRecords();

        if (EditorUtilities.Button("Import Transactions", new Color(0.5f, 1.0f, 0.5f)))
        {
            ImportTransactions();
            Close();
        }
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
            foreach(CreditRecord record in _creditRecords)
            {
                DrawCreditRecord(record, _recordExists[record]);
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
            foreach (DebitRecord_20160911 record in _debitRecords)
            {
                DrawDebitRecord(record, _recordExists[record]);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Draws the given credit record.
    /// </summary>
    /// <param name="record">Record to draw.</param>
    /// <param name="alreadyExists">Whether or not the transaction already exists in the account.</param>
    private void DrawCreditRecord(CreditRecord record, bool alreadyExists)
    {
        GUI.enabled = !alreadyExists;
        EditorGUILayout.BeginHorizontal("box");
        {
            EditorGUILayout.LabelField(record.type.ToString(), GUILayout.Width(60.0f));
            EditorGUILayout.LabelField(record.transactionDate.ToString("MM/dd/yyyy"), GUILayout.Width(100.0f));
            EditorGUILayout.LabelField(record.postDate.ToString("MM/dd/yyyy"), GUILayout.Width(100.0f));
            EditorGUILayout.LabelField(record.description, GUILayout.Width(200.0f));
            EditorGUILayout.LabelField(record.amount.ToString("C2"), GUILayout.Width(100.0f));
        }
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;
    }

    /// <summary>
    /// Draws the given debit record.
    /// </summary>
    /// <param name="record">Record to draw.</param>
    /// <param name="alreadyExists">Whether or not the transaction already exists in the account.</param>
    private void DrawDebitRecord(DebitRecord_20160911 record, bool alreadyExists)
    {
        GUI.enabled = !alreadyExists;
        EditorGUILayout.BeginHorizontal("box");
        {
            EditorGUILayout.LabelField(record.type.ToString(), GUILayout.Width(60.0f));
            EditorGUILayout.LabelField(record.postDate.ToString("MM/dd/yyyy"), GUILayout.Width(100.0f));
            EditorGUILayout.LabelField(record.description, GUILayout.Width(500.0f));
            EditorGUILayout.LabelField(record.amount.ToString("C2"), GUILayout.Width(100.0f));
            EditorGUILayout.LabelField(record.checkOrSlipNo != null ? record.checkOrSlipNo.ToString() : "", GUILayout.Width(50.0f));
        }
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;
    }

    /// <summary>
    /// Imports the credit or debit records as transactions.
    /// </summary>
    private void ImportTransactions()
    {
        if(_creditRecords != null)
        {
            Transaction[] transactions = _creditRecords.Where(record => !_recordExists[record])
                                                       .Select(record => CreateTransaction(record)).ToArray();
            _onImport(transactions);
        }

        if(_debitRecords != null)
        {
            Transaction[] transactions = _debitRecords.Where(record => !_recordExists[record])
                                                      .Select(record => CreateTransaction(record)).ToArray();
            _onImport(transactions);
        }
    }

    /// <summary>
    /// Creates a transaction from the given credit record.
    /// </summary>
    /// <param name="record">Record to create transaction from.</param>
    /// <returns>A transaction created from the given credit record.</returns>
    private Transaction CreateTransaction(CreditRecord record)
    {
        return new Transaction()
        {
            Payee = record.description,
            Description = record.description,
            Amount = record.amount,
            Category = "",
            Date = record.postDate,
            ImportData = new ImportData()
            {
                TransactionType = record.type.ToString(),
                TransactionDate = record.transactionDate,
                PostDate = record.postDate,
                Description = record.description,
                CheckOrSlipNo = null
            }
        };
    }

    /// <summary>
    /// Creates a transaction from the given credit record.
    /// </summary>
    /// <param name="record">Record to create transaction from.</param>
    /// <returns>A transaction created from the given credit record.</returns>
    private Transaction CreateTransaction(DebitRecord_20160911 record)
    {
        return new Transaction()
        {
            Payee = record.description,
            Description = record.description,
            Amount = record.amount,
            Category = "",
            Date = record.postDate,
            ImportData = new ImportData()
            {
                TransactionType = record.type.ToString(),
                TransactionDate = null,
                PostDate = record.postDate,
                Description = record.description,
                CheckOrSlipNo = record.checkOrSlipNo
            }
        };
    }
}
