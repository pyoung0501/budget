using FileHelpers;
using System;

/// <summary>
/// Types of credit records.
/// </summary>
public enum CreditRecordType
{
    SALE,
    PAYMENT,
    FEE
}

/// <summary>
/// Record format for credit account transactions.
/// </summary>
[IgnoreFirst]
[DelimitedRecord(",")]
public class CreditRecord
{
    /// <summary>
    /// Record type.
    /// </summary>
    public CreditRecordType type;

    /// <summary>
    /// Date of the actual transaction.
    /// </summary>
    [FieldConverter(ConverterKind.Date, "MM/dd/yyyy")]
    public DateTime transactionDate;

    /// <summary>
    /// Date transaction was posted to the account.
    /// </summary>
    [FieldConverter(ConverterKind.Date, "MM/dd/yyyy")]
    public DateTime postDate;

    /// <summary>
    /// Transaction description.
    /// </summary>
    public string description;

    /// <summary>
    /// Amount of the transaction.
    /// </summary>
    [FieldConverter(ConverterKind.Decimal, ".")]
    public decimal amount;
}
