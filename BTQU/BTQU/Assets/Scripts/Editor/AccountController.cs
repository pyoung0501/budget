using System;
using BTQLib;
using UnityEditor;
using UnityEngine;

internal class AccountController
{
    private Profile _profile;

    private Account _account;

    private Transaction _transactionToAdd;

    public AccountController(Profile profile, Account account)
    {
        _profile = profile;
        _account = account;
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

                // Transactions
                foreach(Transaction transaction in _account.Transactions)
                {
                    DrawTransaction(transaction);
                }

                if (EditorUtilities.ContentWidthButton("+ Transaction"))
                {
                    _account.AddTransaction(new Transaction() { Date = DateTime.Now });
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawTransactionColumnHeaders()
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Date", GUILayout.Width(80.0f));
            EditorGUILayout.LabelField("Payee", GUILayout.Width(300.0f));
            EditorGUILayout.LabelField("Description", GUILayout.Width(300.0f));
            EditorGUILayout.LabelField("Category", GUILayout.Width(200.0f));
            EditorGUILayout.LabelField("Amount", GUILayout.Width(100.0f));
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawTransaction(Transaction transaction)
    {
        EditorGUILayout.BeginHorizontal();
        {
            DrawDate(transaction);
            transaction.Payee = EditorGUILayout.TextField(transaction.Payee, GUILayout.Width(300.0f));
            transaction.Description = EditorGUILayout.TextField(transaction.Description, GUILayout.Width(300.0f));
            DrawCategory(transaction);
            DrawAmount(transaction);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawDate(Transaction transaction)
    {
        string prevDate = transaction.Date.ToString("MM/dd/yyyy");
        string currDate = EditorGUILayout.TextField(prevDate, GUILayout.Width(80.0f));

        if (currDate != prevDate)
        {
            DateTime newDate;
            if (DateTime.TryParse(currDate, out newDate))
            {
                transaction.Date = newDate;
            }
        }
    }

    private void DrawCategory(Transaction transaction)
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(200.0f));
        {
            string prevCategory = transaction.Category;
            string currCategory = EditorGUILayout.TextField(prevCategory);
            if (currCategory != null)
            {
                string primaryCategory;
                string secondaryCategory;
                GetPrimaryAndSecondaryCatetories(currCategory, out primaryCategory, out secondaryCategory);

                if (secondaryCategory == "")
                {
                    transaction.Category = primaryCategory;
                }
                else
                {
                    transaction.Category = string.Format("{0}:{1}", primaryCategory, secondaryCategory);
                }

                // Show Button if
                //  primaryCategory is non-empty
                //  and primaryCategory does not yet exist
                //  or secondaryCategory is non-empty and secondaryCategory does not yet exist

                bool showButton = primaryCategory != ""
                              && (!_profile.BudgetCategories.PrimaryCategoryExists(primaryCategory)
                               || (secondaryCategory != "" && !_profile.BudgetCategories.SecondaryCategoryExists(primaryCategory, secondaryCategory)));
                if (showButton)
                {
                    if (EditorUtilities.ContentWidthButton("+"))
                    {
                        if (!_profile.BudgetCategories.PrimaryCategoryExists(primaryCategory))
                        {
                            _profile.BudgetCategories.AddPrimaryCategory(primaryCategory);
                        }

                        if (secondaryCategory != "" && !_profile.BudgetCategories.SecondaryCategoryExists(primaryCategory, secondaryCategory))
                        {
                            _profile.BudgetCategories.AddSecondaryCategory(primaryCategory, secondaryCategory);
                        }
                    }
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void GetPrimaryAndSecondaryCatetories(string currCategory, out string primaryCategory, out string secondaryCategory)
    {
        string[] categories = currCategory.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
        if(categories.Length == 0)
        {
            primaryCategory = "";
            secondaryCategory = "";
        }
        else if(categories.Length == 1)
        {
            primaryCategory = categories[0];
            secondaryCategory = "";
        }
        else
        {
            primaryCategory = categories[0];
            secondaryCategory = categories[1];
        }
    }

    GUIStyle rightAlignTextField;
    private void DrawAmount(Transaction transaction)
    {
        //EditorGUILayout.LabelField(transaction.Amount.ToString());

        if (rightAlignTextField == null)
        {
            rightAlignTextField = new GUIStyle(GUI.skin.textField);
            rightAlignTextField.alignment = TextAnchor.MiddleRight;
        }

        string prevVal = transaction.Amount.ToString("C2");

        string newVal = EditorGUILayout.TextField(prevVal, rightAlignTextField, GUILayout.Width(100.0f));
        
        if(newVal != prevVal)
        {
            newVal = newVal.Replace("$", "");

            //TextEditor te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

            decimal amount;
            if(decimal.TryParse(newVal, out amount))
            {
                amount = Math.Round(amount, 2, MidpointRounding.ToEven);
                transaction.Amount = amount;
            }
        }

    }
}
