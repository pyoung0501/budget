using FileHelpers;
using System;

/// <summary>
/// Types of debit records.
/// </summary>
public enum DebitRecordType
{
    DEBIT,
    CREDIT,
    CHECK,
    DSLIP
}

/// <summary>
/// Record format for debit account transactions.
/// </summary>
[IgnoreFirst]
[DelimitedRecord(",")]
public class DebitRecord
{
    /// <summary>
    /// Type of record.
    /// </summary>
    public DebitRecordType type;

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

    /// <summary>
    /// Optional field specifying the check or slip number.
    /// </summary>
    public int? checkOrSlipNo;
}
