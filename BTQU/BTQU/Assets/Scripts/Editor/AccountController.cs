using BTQLib;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


internal class AccountController
{
    private Profile _profile;
    private Account _account;
    private Transaction _transactionToAdd;

    /// <summary>
    /// List of the transaction columns used for displaying transactions.
    /// </summary>
    private List<TransactionColumn> _transactionColumns = new List<TransactionColumn>();

    /// <summary>
    /// The column the transactions are sorted on.
    /// </summary>
    private TransactionColumn _sortedColumn;

    /// <summary>
    /// The sorted state of the sorted column.
    /// </summary>
    private SortedState _sortedState;

    /// <summary>
    /// The transactions sorted according to the sort state and column.
    /// </summary>
    private List<Transaction> _sortedTransactions;

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
        _transactionColumns.Add(new CategoryColumn(_profile.BudgetCategories) { Width = 200.0f });
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
                DrawTransaction(_transactionToAdd);

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
                
                foreach(Transaction transaction in _sortedTransactions)
                {
                    DrawTransaction(transaction);
                }

                if (EditorUtilities.ContentWidthButton("+ Transaction"))
                {
                    _transactionToAdd = new Transaction() { Date = DateTime.Now };
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawTransactionColumnHeaders()
    {
        EditorGUILayout.BeginHorizontal();
        {
            foreach (TransactionColumn column in _transactionColumns)
            {
                DrawColumnHeading(column);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the heading for the given column.
    /// </summary>
    /// <param name="column">Column to draw heading for.</param>
    private void DrawColumnHeading(TransactionColumn column)
    {
        SortedState sortedState = (_sortedColumn == column) ? _sortedState : SortedState.None;
        bool pressed = DrawColumnHeading(column.DisplayName, sortedState, column.Width);
        if(pressed)
        {
            if(column != _sortedColumn)
            {
                _sortedColumn = column;
                _sortedState = SortedState.Ascending;
            }
            else
            {
                if(_sortedState == SortedState.None)
                {
                    _sortedState = SortedState.Ascending;
                }
                else if(_sortedState == SortedState.Ascending)
                {
                    _sortedState = SortedState.Descending;
                }
                else
                {
                    _sortedState = SortedState.None;
                }
            }

            UpdateSorting();
        }
    }

    /// <summary>
    /// Draws a column heading as a button with the given label, sorted state and width.
    /// Return value is true if the button was pressed.
    /// </summary>
    /// <param name="label">Column label.</param>
    /// <param name="sortedState">Column's sorted state.</param>
    /// <param name="width">Width of the column.</param>
    /// <returns>True if the heading button was pressed.</returns>
    private bool DrawColumnHeading(string label, SortedState sortedState, float width)
    {
        string stateIndicator = GetStateIndicator(sortedState);
        string fullLabel = string.Concat(label, " ", stateIndicator);

        return GUILayout.Button(fullLabel, GUILayout.Width(width));
    }

    /// <summary>
    /// Gets the indicator to use for the given sorted state.
    /// </summary>
    /// <param name="sortedState">Sorted state.</param>
    /// <returns>The indicator to use for the given sorted state.</returns>
    private string GetStateIndicator(SortedState sortedState)
    {
        if (sortedState == SortedState.Ascending) return "▲";
        if (sortedState == SortedState.Descending) return "▼";

        return "";
    }

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

    /// <summary>
    /// Updates the sorting of the transactions.  If there is no column
    /// the data is sorted on, the transactions will remain in the order
    /// in which they were entered into the account.
    /// </summary>
    private void UpdateSorting()
    {
        _sortedTransactions = new List<Transaction>(_account.Transactions);

        if (_sortedColumn != null && _sortedState != SortedState.None)
        {
            if (_sortedColumn is DateColumn)
            {
                _sortedTransactions.Sort((lhs, rhs) => lhs.Date.CompareTo(rhs.Date));
            }
            else if (_sortedColumn is PayeeColumn)
            {
                _sortedTransactions.Sort((lhs, rhs) => string.Compare(lhs.Payee, rhs.Payee));
            }
            else if (_sortedColumn is DescriptionColumn)
            {
                _sortedTransactions.Sort((lhs, rhs) => string.Compare(lhs.Description, rhs.Description));
            }
            else if (_sortedColumn is CategoryColumn)
            {
                _sortedTransactions.Sort((lhs, rhs) => string.Compare(lhs.Category, rhs.Category));
            }
            else if (_sortedColumn is AmountColumn)
            {
                _sortedTransactions.Sort((lhs, rhs) => lhs.Amount.CompareTo(rhs.Amount));
            }

            if (_sortedState == SortedState.Descending)
            {
                _sortedTransactions.Reverse();
            }
        }
    }
}
