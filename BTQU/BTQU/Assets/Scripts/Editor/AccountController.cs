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
    /// <summary>
    /// Width of the balance column.
    /// </summary>
    private const float BalanceColumnWidth = 100.0f;

    private Profile _profile;
    private Account _account;
    private Transaction _transactionToAdd;

    /// <summary>
    /// List of the transaction columns used for displaying transactions.
    /// </summary>
    private List<TransactionColumn> _transactionColumns = new List<TransactionColumn>();

    /// <summary>
    /// The primary column to sort transactions by.
    /// </summary>
    private TransactionColumn _primarySortedColumn;

    /// <summary>
    /// The secondary column to sort transactions by.
    /// </summary>
    private TransactionColumn _secondarySortedColumn;

    /// <summary>
    /// The sorted state of the primary column.
    /// </summary>
    private SortedState _primarySortedState;

    /// <summary>
    /// The sorted state of the secondary column.
    /// </summary>
    private SortedState _secondarySortedState;

    /// <summary>
    /// The transactions sorted according to the sort state and column.
    /// </summary>
    private List<Transaction> _sortedTransactions;

    /// <summary>
    /// Scroll position.
    /// </summary>
    private Vector2 _scrollPos;

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
        _transactionColumns.Add(new DateColumn() { Width = 80.0f });
        _transactionColumns.Add(new PayeeColumn() { Width = 300.0f });
        _transactionColumns.Add(new DescriptionColumn() { Width = 300.0f });
        _transactionColumns.Add(new CategoryColumn(_profile.Categories) { Width = 200.0f });
        _transactionColumns.Add(new AmountColumn() { Width = 100.0f });

        _sortedTransactions = new List<Transaction>(_account.Transactions);
    }
    

    public void DrawView()
    {
        EditorGUILayout.BeginVertical("box");
        {
            _account.Name = EditorGUILayout.TextField("Name", _account.Name);
            _account.Institution = EditorGUILayout.TextField("Institution", _account.Institution);
            _account.Number = EditorGUILayout.TextField("Account No.", _account.Number);
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical();
        {
            if(_transactionToAdd != null)
            {
                DrawTransaction(_transactionToAdd, null);

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Add"))
                    {
                        _account.AddTransaction(_transactionToAdd);
                        _transactionToAdd = null;

                        UpdateSorting();
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
                DrawTransactionColumnHeaders();

                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
                {
                    decimal balance = 0;
                    foreach (Transaction transaction in _sortedTransactions)
                    {
                        balance += transaction.Amount;

                        DrawTransaction(transaction, balance);
                    }

                    if (EditorUtilities.ContentWidthButton("+ Transaction"))
                    {
                        _transactionToAdd = new Transaction() { Date = DateTime.Now };
                    }
                }
                EditorGUILayout.EndScrollView();
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

        UpdateSorting();
    }

    private void DrawTransactionColumnHeaders()
    {
        EditorGUILayout.BeginHorizontal();
        {
            foreach (TransactionColumn column in _transactionColumns)
            {
                DrawColumnHeading(column);
            }

            EditorGUILayout.LabelField("Balance", Styles.CenterJustifiedLabel, GUILayout.Width(BalanceColumnWidth));
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the heading for the given column.
    /// </summary>
    /// <param name="column">Column to draw heading for.</param>
    private void DrawColumnHeading(TransactionColumn column)
    {
        ColumnDesignation designation = GetColumnDesignation(column);
        SortedState sortedState = GetColumnSortedState(column);

        bool pressed = DrawColumnHeading(column.DisplayName, designation, sortedState, column.Width);
        if(pressed)
        {
            if(Event.current.button == 0)
            {
                HandlePrimaryColumnClick(column);
            }
            else if(Event.current.button == 1)
            {
                HandleSecondaryColumnClick(column);
            }

            UpdateSorting();
        }
    }

    /// <summary>
    /// Gets the designation of the given column.
    /// </summary>
    /// <param name="column">Column.</param>
    /// <returns>The designation of the given column.</returns>
    private ColumnDesignation GetColumnDesignation(TransactionColumn column)
    {
        if (column == _primarySortedColumn)
        {
            return ColumnDesignation.Primary;
        }
        else if (column == _secondarySortedColumn)
        {
            return ColumnDesignation.Secondary;
        }

        return ColumnDesignation.Other;
    }

    /// <summary>
    /// Gets the sorted state of the given column.
    /// </summary>
    /// <param name="column">Column.</param>
    /// <returns>The sorted state of the given column.</returns>
    private SortedState GetColumnSortedState(TransactionColumn column)
    {
        if (column == _primarySortedColumn)
        {
            return _primarySortedState;
        }
        else if (column == _secondarySortedColumn)
        {
            return _secondarySortedState;
        }

        return SortedState.None;
    }

    /// <summary>
    /// Draws a column heading as a button with the given label, sorted state and width.
    /// Return value is true if the button was pressed.
    /// </summary>
    /// <param name="label">Column label.</param>
    /// <param name="columnDesignation">Column's designation.</param>
    /// <param name="sortedState">Column's sorted state.</param>
    /// <param name="width">Width of the column.</param>
    /// <returns>True if the heading button was pressed.</returns>
    private bool DrawColumnHeading(string label, ColumnDesignation columnDesignation, SortedState sortedState, float width)
    {
        string stateIndicator = GetStateIndicator(columnDesignation, sortedState);
        string fullLabel = string.Concat(label, " ", stateIndicator);

        return GUILayout.Button(fullLabel, GUILayout.Width(width));
    }

    /// <summary>
    /// Gets the indicator to use for the specified designate and state.
    /// </summary>
    /// <param name="columnDesignation">Column's designation.</param>
    /// <param name="sortedState">Sorted state.</param>
    /// <returns>The indicator to use for the specified designate and state.</returns>
    private string GetStateIndicator(ColumnDesignation columnDesignation, SortedState sortedState)
    {
        if (sortedState == SortedState.Ascending)
        {
            if (columnDesignation == ColumnDesignation.Primary)
            {
                return "▲";
            }
            else if (columnDesignation == ColumnDesignation.Secondary)
            {
                return "↑";
            }
        }

        if (sortedState == SortedState.Descending)
        {
            if (columnDesignation == ColumnDesignation.Primary)
            {
                return "▼";
            }
            else if (columnDesignation == ColumnDesignation.Secondary)
            {
                return "↓";
            }
        }

        return "";
    }
    
    /// <summary>
    /// Handles a column click by the primary button.
    /// If the column clicked is already a primary, the sort state is cycled.
    /// If the column clicked is secondary, the column is made primary.
    /// If the column clicked is neither, the column is made primary.
    /// </summary>
    /// <param name="column">Column clicked on.</param>
    private void HandlePrimaryColumnClick(TransactionColumn column)
    {
        bool isPrimaryColumn = column == _primarySortedColumn;
        bool isSecondaryColumn = column == _secondarySortedColumn;
        
        if(isPrimaryColumn)
        {
            CyclePrimary();
        }
        else if(isSecondaryColumn)
        {
            _secondarySortedColumn = null;
            _primarySortedColumn = column;
        }
        else
        {
            _primarySortedColumn = column;
            _primarySortedState = SortedState.Ascending;
        }
    }

    /// <summary>
    /// Cycles the primary sort state.
    /// </summary>
    private void CyclePrimary()
    {
        if (_primarySortedState == SortedState.None)
        {
            _primarySortedState = SortedState.Ascending;
        }
        else if (_primarySortedState == SortedState.Ascending)
        {
            _primarySortedState = SortedState.Descending;
        }
        else
        {
            _primarySortedState = SortedState.None;
            _primarySortedColumn = null;
        }
    }

    /// <summary>
    /// Handles a column clicked by the secondary button.
    /// If the column clicked is primary, cancel the secondary column.
    /// If the column clicked is secondary, cycle the secondary state.
    /// If the column clicked is neither, the column is made secondary.
    /// </summary>
    /// <param name="column">Column clicked.</param>
    private void HandleSecondaryColumnClick(TransactionColumn column)
    {
        bool isPrimaryColumn = column == _primarySortedColumn;
        bool isSecondaryColumn = column == _secondarySortedColumn;
        
        if (isPrimaryColumn)
        {
            _secondarySortedColumn = null;
            _secondarySortedState = SortedState.None;
        }
        else if(isSecondaryColumn)
        {
            CycleSecondary();
        }
        else
        {
            _secondarySortedColumn = column;
            _secondarySortedState = SortedState.Ascending;
        }
    }

    /// <summary>
    /// Cycles the secondary column state.
    /// </summary>
    private void CycleSecondary()
    {
        if (_secondarySortedState == SortedState.None)
        {
            _secondarySortedState = SortedState.Ascending;
        }
        else if (_secondarySortedState == SortedState.Ascending)
        {
            _secondarySortedState = SortedState.Descending;
        }
        else
        {
            _secondarySortedState = SortedState.None;
            _secondarySortedColumn = null;
        }
    }

    /// <summary>
    /// Draws the given transaction with optional balance.
    /// </summary>
    /// <param name="transaction">Transaction to draw.</param>
    /// <param name="balance">Optional balance.</param>
    private void DrawTransaction(Transaction transaction, decimal? balance)
    {
        EditorGUILayout.BeginHorizontal();
        {
            foreach (TransactionColumn column in _transactionColumns)
            {
                column.Draw(transaction);
            }

            if(balance.HasValue)
            {
                GUI.enabled = false;
                EditorGUILayout.TextField(balance.Value.ToString("C2"),
                                          Styles.RightAlignedTextField,
                                          GUILayout.Width(BalanceColumnWidth));
                GUI.enabled = true;
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Updates the sorting of the transactions.  If there is no column
    /// the data is sorted on, the transactions will remain in the order
    /// in which they were entered into the account.
    /// </summary>
    private void UpdateSorting()
    {
        IOrderedEnumerable<Transaction> orderedTransactions = null;
        if(_primarySortedColumn != null)
        {
            if(_primarySortedState == SortedState.Ascending)
            {
                if (_primarySortedColumn is DateColumn)
                {
                    orderedTransactions = _account.Transactions.OrderBy(Date);
                }
                else if (_primarySortedColumn is PayeeColumn)
                {
                    orderedTransactions = _account.Transactions.OrderBy(item => item.Payee);
                }
                else if (_primarySortedColumn is DescriptionColumn)
                {
                    orderedTransactions = _account.Transactions.OrderBy(item => item.Description);
                }
                else if (_primarySortedColumn is CategoryColumn)
                {
                    orderedTransactions = _account.Transactions.OrderBy(item => item.Category);
                }
                else if (_primarySortedColumn is AmountColumn)
                {
                    orderedTransactions = _account.Transactions.OrderBy(item => item.Amount);
                }
            }
            else if(_primarySortedState == SortedState.Descending)
            {
                if (_primarySortedColumn is DateColumn)
                {
                    orderedTransactions = _account.Transactions.OrderByDescending(Date);
                }
                else if (_primarySortedColumn is PayeeColumn)
                {
                    orderedTransactions = _account.Transactions.OrderByDescending(item => item.Payee);
                }
                else if (_primarySortedColumn is DescriptionColumn)
                {
                    orderedTransactions = _account.Transactions.OrderByDescending(item => item.Description);
                }
                else if (_primarySortedColumn is CategoryColumn)
                {
                    orderedTransactions = _account.Transactions.OrderByDescending(item => item.Category);
                }
                else if (_primarySortedColumn is AmountColumn)
                {
                    orderedTransactions = _account.Transactions.OrderByDescending(item => item.Amount);
                }
            }
            else
            {
                orderedTransactions = _account.Transactions.OrderBy(item => _account.Transactions.IndexOf(item));
            }
        }
        else
        {
            orderedTransactions = _account.Transactions.OrderBy(item => _account.Transactions.IndexOf(item));
        }

        if (_secondarySortedColumn != null)
        {
            if (_secondarySortedState == SortedState.Ascending)
            {
                if (_secondarySortedColumn is DateColumn)
                {
                    orderedTransactions = orderedTransactions.ThenBy(Date);
                }
                else if (_secondarySortedColumn is PayeeColumn)
                {
                    orderedTransactions = orderedTransactions.ThenBy(Payee);
                }
                else if (_secondarySortedColumn is DescriptionColumn)
                {
                    orderedTransactions = orderedTransactions.ThenBy(Description);
                }
                else if (_secondarySortedColumn is CategoryColumn)
                {
                    orderedTransactions = orderedTransactions.ThenBy(Category);
                }
                else if (_secondarySortedColumn is AmountColumn)
                {
                    orderedTransactions = orderedTransactions.ThenBy(Amount);
                }
            }
            else if (_secondarySortedState == SortedState.Descending)
            {
                if (_secondarySortedColumn is DateColumn)
                {
                    orderedTransactions = orderedTransactions.ThenByDescending(Date);
                }
                else if (_secondarySortedColumn is PayeeColumn)
                {
                    orderedTransactions = orderedTransactions.ThenByDescending(Payee);
                }
                else if (_secondarySortedColumn is DescriptionColumn)
                {
                    orderedTransactions = orderedTransactions.ThenByDescending(Description);
                }
                else if (_secondarySortedColumn is CategoryColumn)
                {
                    orderedTransactions = orderedTransactions.ThenByDescending(Category);
                }
                else if (_secondarySortedColumn is AmountColumn)
                {
                    orderedTransactions = orderedTransactions.ThenByDescending(Amount);
                }
            }
        }

        _sortedTransactions = orderedTransactions.ToList();
    }

    // Functions for sorting by fields within transaction.
    private Func<Transaction, DateTime> Date =          (item) => { return item.Date.Date; };
    private Func<Transaction, string>   Payee =         (item) => { return item.Payee; };
    private Func<Transaction, string>   Description =   (item) => { return item.Description; };
    private Func<Transaction, string>   Category =      (item) => { return item.Category; };
    private Func<Transaction, decimal>  Amount =        (item) => { return item.Amount; };
}
