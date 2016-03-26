using BTQLib;
using UnityEngine;

public class ProfileController
{
    private Profile _selectedProfile;

    public ProfileController(Profile selectedProfile)
    {
        _selectedProfile = selectedProfile;
    }

    public void DrawView()
    {
        GUILayout.Label(_selectedProfile.Name);

        Account accountToRemove = null;
        foreach (Account account in _selectedProfile.Accounts)
        {
            account.ID = GUILayout.TextField(account.ID);

            if(GUILayout.Button("-"))
            {
                accountToRemove = account;
            }
        }

        if(GUILayout.Button("+"))
        {
            _selectedProfile.Accounts.Add(new Account("New Account"));
        }

        if(accountToRemove != null)
        {
            _selectedProfile.Accounts.Remove(accountToRemove);
            accountToRemove = null;
        }
    }
}