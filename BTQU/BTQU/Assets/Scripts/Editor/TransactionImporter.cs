﻿using System.IO;
using UnityEngine;

/// <summary>
/// Importer for importing transactions.
/// </summary>
public class TransactionImporter
{
    /// <summary>
    /// The fields header in a credit account CSV file used to identify
    /// the file contents.
    /// </summary>
    private const string CreditAccountFieldsHeader = "Type,Trans Date,Post Date,Description,Amount";

    /// <summary>
    /// The fields header in a debit account CSV file used to identify
    /// the file contents.
    /// </summary>
    private const string DebitAccountFieldsHeader = "Type,Post Date,Description,Amount,Check or Slip #";

    /// <summary>
    /// Imports the transactions contained in the given file.
    /// </summary>
    /// <param name="filePath">File to import.</param>
    public void Import(string filePath)
    {
        string fileFields = null;
        using (StreamReader reader = new StreamReader(filePath))
        {
            fileFields = reader.ReadLine();
        }

        if (fileFields == CreditAccountFieldsHeader)
        {
            Debug.Log("Reading Credit Records");

            var engine = new FileHelpers.FileHelperEngine<CreditRecord>();
            engine.ErrorManager.ErrorMode = FileHelpers.ErrorMode.SaveAndContinue;
            var records = engine.ReadFile(filePath);

            if (engine.ErrorManager.HasErrors)
            {
                foreach (FileHelpers.ErrorInfo error in engine.ErrorManager.Errors)
                {
                    Debug.LogError(string.Format("Error on Line number: {0}\nRecord causing the problem: {1}\nComplete exception information: {2}",
                        error.LineNumber, error.RecordString, error.ExceptionInfo.ToString()));
                }
            }
            else
            {
                foreach (CreditRecord record in records)
                {
                    record.description = record.description.Trim(' ', '\"');
                }

                ImportEditor.Open(records);
            }
        }
        else if (fileFields == DebitAccountFieldsHeader)
        {
            Debug.Log("Reading Debit Records");

            var engine = new FileHelpers.FileHelperEngine<DebitRecord>();
            engine.ErrorManager.ErrorMode = FileHelpers.ErrorMode.SaveAndContinue;
            var records = engine.ReadFile(filePath);

            if (engine.ErrorManager.HasErrors)
            {
                foreach (FileHelpers.ErrorInfo error in engine.ErrorManager.Errors)
                {
                    Debug.LogError(string.Format("Error on Line number: {0}\nRecord causing the problem: {1}\nComplete exception information: {2}",
                        error.LineNumber, error.RecordString, error.ExceptionInfo.ToString()));
                }
            }
            else
            {
                foreach (DebitRecord record in records)
                {
                    record.description = record.description.Trim(' ', '\"');
                }

                ImportEditor.Open(records);
            }
        }
        else
        {
            Debug.LogError("File contents do not match any known format.");
        }
    }
}
