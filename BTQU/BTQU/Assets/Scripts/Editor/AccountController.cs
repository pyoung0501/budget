using BTQ;
using BTQLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class AccountController
{
    private Profile _profile;
    private Account _account;
    private Transaction _transactionToAdd;
    
    /// <summary>
    /// The columns to display for the transaction data.
    /// </summary>
    private TransactionColumn[] _transactionColumns;

    /// <summary>
    /// View for displaying the list of transactions in the account.
    /// </summary>
    private TransactionsView _transactionsView;

    public AccountController(Profile profile, Account account)
    {
        _profile = profile;
        _account = account;

        Initialize();
    }

    /// <summary>
    /// Initializes the account controller.
    /// </summary>
    private void Initialize()
    {
        _transactionColumns = new TransactionColumn[]
            {
                new DateColumn() { Width = 80.0f },
                new PayeeColumn() { Width = 300.0f },
                new DescriptionColumn() { Width = 300.0f },
                new CategoryColumn(_profile.Categories) { Width = 200.0f },
                new AmountColumn() { Width = 100.0f }
            };

        _transactionsView = new TransactionsView(
            new TransactionsView.Settings()
            {
                Columns = _transactionColumns,
                ShowRunningBalance = true
            }
        );

        _transactionsView.Refresh(_account.Transactions.ToList());
    }
    

    public void DrawView()
    {
        EditorGUILayout.BeginVertical("box");
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical();
                {
                    _account.Name = EditorGUILayout.TextField("Name", _account.Name);
                    _account.Institution = EditorGUILayout.TextField("Institution", _account.Institution);
                    _account.Number = EditorGUILayout.TextField("Account No.", _account.Number);
                }
                EditorGUILayout.EndVertical();

                GUILayout.Space(16.0f);

                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Starting Balance", GUILayout.Width(EditorGUIUtility.labelWidth));

                        _account.StartingBalance = TransactionUtilities.AmountField(_account.StartingBalance);

                        if(EditorUtilities.ContentWidthButton("Edit Distribution"))
                        {
                            StartingDistributionEditor.Create(_account, _profile.Categories);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical();
        {
            if(_transactionToAdd != null)
            {
                DrawTransaction(_transactionToAdd);

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Add"))
                    {
                        _account.AddTransaction(_transactionToAdd);
                        _transactionToAdd = null;
                        _transactionsView.Refresh(_account.Transactions.ToList());
                    }

                    if(GUILayout.Button("Cancel"))
                    {
                        _transactionToAdd = null;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                _transactionsView.Draw(_account.StartingBalance);

                if (EditorUtilities.ContentWidthButton("+ Transaction"))
                {
                    _transactionToAdd = new Transaction() { Date = DateTime.Now };
                }
            }
        }
        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal("box");
        {
            if(EditorUtilities.ContentWidthButton("Import"))
            {
                string importDir = EditorPrefs.GetString("BTQU_ImportDir");
                string importFile = EditorUtility.OpenFilePanel("Import", importDir, "csv");
                if(!string.IsNullOrEmpty(importFile))
                {
                    importDir = Path.GetDirectoryName(importFile);
                    EditorPrefs.SetString("BTQU_ImportDir", importDir);

                    TransactionImporter importer = new TransactionImporter();
                    importer.Import(importFile, _account, OnImportTransactions);
                }
            }

            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Handles importing the given transactions.
    /// </summary>
    /// <param name="transactions">Transactions to import.</param>
    private void OnImportTransactions(Transaction[] transactions)
    {
        foreach(Transaction transaction in transactions)
        {
            _account.AddTransaction(transaction);
        }

        _transactionsView.Refresh(_account.Transactions.ToList());
    }
    
    /// <summary>
    /// Draws the given transaction.
    /// </summary>
    /// <param name="transaction">Transaction to draw.</param>
    private void DrawTransaction(Transaction transaction)
    {
        EditorGUILayout.BeginHorizontal();
        {
            foreach (TransactionColumn column in _transactionColumns)
            {
                column.Draw(transaction);
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
