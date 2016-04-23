using BTQLib;

/// <summary>
/// Base class for a transaction column (associated with one field of a transaction).
/// </summary>
public abstract class TransactionColumn
{
    /// <summary>
    /// Width of the column.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// The display name of the column.
    /// </summary>
    public abstract string DisplayName { get; }

    /// <summary>
    /// Draws the column value for the given transaction.
    /// </summary>
    /// <param name="transaction">Transaction to draw.</param>
    public abstract void Draw(Transaction transaction);
}
