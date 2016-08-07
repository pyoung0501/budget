using BTQ;
using BTQLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// View for a list of transactions.
/// </summary>
public class TransactionsView
{
    /// <summary>
    /// Width of the balance column.
    /// </summary>
    private const float BalanceColumnWidth = 100.0f;


    /// <summary>
    /// List of transactions being managed by this view.
    /// </summary>
    private List<Transaction> _sourceTransactions;

    /// <summary>
    /// The transactions sorted according to the sort state and column.
    /// </summary>
    private List<Transaction> _sortedTransactions;

    /// <summary>
    /// Scroll position.
    /// </summary>
    private Vector2 _scrollPos;

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
    /// Settings for the transactions view.
    /// </summary>
    private Settings _settings;

    /// <summary>
    /// Whether or not the view has any transactions in it.
    /// </summary>
    public bool HasTransactions
    {
        get { return _sourceTransactions != null && _sourceTransactions.Count > 0; }
    }


    /// <summary>
    /// The settings for the transactions view.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// The columns to display for the list of transactions.
        /// </summary>
        public TransactionColumn[] Columns;

        /// <summary>
        /// Whether or not to show a running balance for each transaction.
        /// </summary>
        public bool ShowRunningBalance;
    }


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="settings">Settings.</param>
    public TransactionsView(Settings settings)
    {
        _settings = settings;

        Initialize();
    }

    /// <summary>
    /// Initializes the account controller.
    /// </summary>
    private void Initialize()
    {
        if(_settings.Columns == null)
        {
            _settings.Columns = new TransactionColumn[0];
        }
    }

    /// <summary>
    /// Refreshes the transactions view with an optional list of new
    /// transactions to start viewing.
    /// </summary>
    /// <param name="transactions">Optional list of transactions to view.</param>
    public void Refresh(List<Transaction> transactions = null)
    {
        if(transactions != null)
        {
            _sourceTransactions = transactions;
        }

        UpdateSorting();
    }

    /// <summary>
    /// Draws the list of transactions according to the columns specified
    /// in the settings.
    /// </summary>
    public void Draw()
    {
        DrawTransactionColumnHeaders();

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        {
            decimal balance = 0;
            foreach (Transaction transaction in _sortedTransactions)
            {
                balance += transaction.Amount;

                if (_settings.ShowRunningBalance)
                {
                    DrawTransaction(transaction, balance);
                }
                else
                {
                    DrawTransaction(transaction, null);
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Draws the transaction column headers.
    /// </summary>
    private void DrawTransactionColumnHeaders()
    {
        EditorGUILayout.BeginHorizontal();
        {
            foreach (TransactionColumn column in _settings.Columns)
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
        ColumnDesignation designation = GetColumnDesignation(column);
        SortedState sortedState = GetColumnSortedState(column);

        bool pressed = DrawColumnHeading(column.DisplayName, designation, sortedState, column.Width);
        if (pressed)
        {
            if (Event.current.button == 0)
            {
                HandlePrimaryColumnClick(column);
            }
            else if (Event.current.button == 1)
            {
                HandleSecondaryColumnClick(column);
            }

            UpdateSorting();
        }
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

        if (isPrimaryColumn)
        {
            CyclePrimary();
        }
        else if (isSecondaryColumn)
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
        else if (isSecondaryColumn)
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
            foreach (TransactionColumn column in _settings.Columns)
            {
                EditorUtilities.BeginEnabled(column.Editable);
                column.Draw(transaction);
                EditorUtilities.EndEnabled();
            }

            if (balance.HasValue)
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
        if (_primarySortedColumn != null)
        {
            if (_primarySortedState == SortedState.Ascending)
            {
                if (_primarySortedColumn is DateColumn)
                {
                    orderedTransactions = _sourceTransactions.OrderBy(Date);
                }
                else if (_primarySortedColumn is PayeeColumn)
                {
                    orderedTransactions = _sourceTransactions.OrderBy(Payee);
                }
                else if (_primarySortedColumn is DescriptionColumn)
                {
                    orderedTransactions = _sourceTransactions.OrderBy(Description);
                }
                else if (_primarySortedColumn is CategoryColumn)
                {
                    orderedTransactions = _sourceTransactions.OrderBy(Category);
                }
                else if (_primarySortedColumn is AmountColumn)
                {
                    orderedTransactions = _sourceTransactions.OrderBy(Amount);
                }
                else if (_primarySortedColumn is AppliedToColumn)
                {
                    orderedTransactions = _sourceTransactions.OrderBy(AppliedState);
                }
                else if (_primarySortedColumn is CategoryAppliedToColumn)
                {
                    orderedTransactions = _sourceTransactions.OrderBy(AppliedToCategory);
                }
            }
            else if (_primarySortedState == SortedState.Descending)
            {
                if (_primarySortedColumn is DateColumn)
                {
                    orderedTransactions = _sourceTransactions.OrderByDescending(Date);
                }
                else if (_primarySortedColumn is PayeeColumn)
                {
                    orderedTransactions = _sourceTransactions.OrderByDescending(Payee);
                }
                else if (_primarySortedColumn is DescriptionColumn)
                {
                    orderedTransactions = _sourceTransactions.OrderByDescending(Description);
                }
                else if (_primarySortedColumn is CategoryColumn)
                {
                    orderedTransactions = _sourceTransactions.OrderByDescending(Category);
                }
                else if (_primarySortedColumn is AmountColumn)
                {
                    orderedTransactions = _sourceTransactions.OrderByDescending(Amount);
                }
                else if(_primarySortedColumn is AppliedToColumn)
                {
                    orderedTransactions = _sourceTransactions.OrderByDescending(AppliedState);
                }
                else if (_primarySortedColumn is CategoryAppliedToColumn)
                {
                    orderedTransactions = _sourceTransactions.OrderByDescending(AppliedToCategory);
                }
            }
            else
            {
                orderedTransactions = _sourceTransactions.OrderBy(item => _sourceTransactions.IndexOf(item));
            }
        }
        else
        {
            orderedTransactions = _sourceTransactions.OrderBy(item => _sourceTransactions.IndexOf(item));
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
                else if (_secondarySortedColumn is AppliedToColumn)
                {
                    orderedTransactions = orderedTransactions.ThenBy(AppliedState);
                }
                else if (_primarySortedColumn is CategoryAppliedToColumn)
                {
                    orderedTransactions = orderedTransactions.ThenBy(AppliedToCategory);
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
                else if(_secondarySortedColumn is AppliedToColumn)
                {
                    orderedTransactions = orderedTransactions.ThenByDescending(AppliedState);
                }
                else if (_primarySortedColumn is CategoryAppliedToColumn)
                {
                    orderedTransactions = orderedTransactions.ThenByDescending(AppliedToCategory);
                }
            }
        }

        _sortedTransactions = orderedTransactions.ToList();
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

    // Functions for sorting by fields within transaction.
    private Func<Transaction, DateTime> Date = (item) => { return item.Date.Date; };
    private Func<Transaction, string> Payee = (item) => { return item.Payee; };
    private Func<Transaction, string> Description = (item) => { return item.Description; };
    private Func<Transaction, string> Category = (item) => { return item.Category; };
    private Func<Transaction, decimal> Amount = (item) => { return item.Amount; };
    private Func<Transaction, AppliedState> AppliedState = (item) => { return item.AppliedState; };
    private Func<Transaction, string> AppliedToCategory = (item) => { return item.AppliedToCategory; };
}
