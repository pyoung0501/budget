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
public class DebitRecordOld
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


/// <summary>
/// Record format for debit account transactions.
/// (Current as of September 2016.)
/// TODO: Transaction import needs to be re-thought to account
/// for format changes, don't want to have to edit each time
/// the format changes.
/// </summary>
[IgnoreFirst]
[DelimitedRecord(",")]
public class DebitRecord_20160911
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
    /// Ignoring extra Type field.
    /// </summary>
    public string ignoredType;

    /// <summary>
    /// Ignoring Balance field.
    /// </summary>
    public decimal ignoredBalance;

    /// <summary>
    /// Optional field specifying the check or slip number.
    /// </summary>
    public int? checkOrSlipNo;

    /// <summary>
    /// Ignoring unnamed trailing field which has no data.
    /// Seems that Chase added this now which kills the import.
    /// </summary>
    public string ignoredTrailing;
}
