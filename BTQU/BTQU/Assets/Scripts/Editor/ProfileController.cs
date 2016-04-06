﻿using BTQLib;
using BTQ;
using UnityEngine;
using UnityEditor;
using System;

public class ProfileController
{
    private Profile _selectedProfile;

    private Account _selectedAccount;
    private AccountController _accountController;

    public ProfileController(Profile selectedProfile)
    {
        _selectedProfile = selectedProfile;
    }

    public void DrawView()
    {
        if (_selectedAccount != null)
        {
            if (_accountController == null)
            {
                _accountController = new AccountController(_selectedProfile, _selectedAccount);
            }

            DrawSelectedAccountHeader();

            if(_accountController != null)
            {
                _accountController.DrawView();
            }

            return;
        }

        DrawAccounts();
    }

    private void DrawSelectedAccountHeader()
    {
        EditorGUILayout.BeginHorizontal("box");
        {
            if (EditorUtilities.ContentWidthButton("< Accounts"))
            {
                _selectedAccount = null;
                _accountController = null;
            }
            else
            {
                EditorUtilities.BeginHorizontalCentering();
                EditorUtilities.ContentWidthLabel(string.Format("{0}", _selectedAccount.Name), EditorStyles.boldLabel);
                EditorUtilities.EndHorizontalCentering();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawAccounts()
    {
        EditorUtilities.BeginVerticalCentering();
        {
            if (_selectedProfile.Accounts.Count > 0)
            {
                EditorUtilities.BeginHorizontalCentering();
                EditorUtilities.ContentWidthLabel("Select an Account", EditorStyles.boldLabel);
                EditorUtilities.EndHorizontalCentering();

                GUILayout.Space(16.0f);

                EditorUtilities.BeginHorizontalCentering();
                EditorGUILayout.BeginVertical("box");
                {
                    Account accountToRemove = null;
                    foreach (Account account in _selectedProfile.Accounts)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if(EditorUtilities.ContentWidthButton(">"))
                            {
                                _selectedAccount = account;
                            }

                            account.Name = GUILayout.TextField(account.Name != null ? account.Name : "", GUILayout.Width(300.0f));
                            
                            if(EditorUtilities.ContentWidthButton("-"))
                            {
                                accountToRemove = account;
                            }
                        }
                        GUILayout.EndHorizontal();
                    }

                    if (accountToRemove != null)
                    {
                        _selectedProfile.Accounts.Remove(accountToRemove);
                        accountToRemove = null;
                    }
                }
                EditorGUILayout.EndVertical();
                EditorUtilities.EndHorizontalCentering();
            }

            GUILayout.Space(16.0f);

            EditorUtilities.BeginHorizontalCentering();
            if (EditorUtilities.ContentWidthButton("+ Account"))
            {
                _selectedProfile.Accounts.Add(new Account("New Account"));
            }
            EditorUtilities.EndHorizontalCentering();
        }
        EditorUtilities.EndVerticalCentering();
    }
}