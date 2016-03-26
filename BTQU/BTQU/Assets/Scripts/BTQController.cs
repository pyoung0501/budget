using UnityEngine;
using BTQLib;
using System.Collections.Generic;

public class BTQController : MonoBehaviour
{
    private List<Profile> _profiles = new List<Profile>();

    private Profile _profileToAdd = null;

    private Profile _selectedProfile;

    private ProfileController _profileController;

    void OnGUI()
    {
        if(_selectedProfile != null)
        {
            if(_profileController == null)
            {
                _profileController = new ProfileController(_selectedProfile);
            }

            GUILayout.BeginVertical("box");
            {
                _profileController.DrawView();

                if (GUILayout.Button("Back"))
                {
                    _selectedProfile = null;
                }
            }
            GUILayout.EndVertical();

            return;
        }

        if(_profileToAdd != null)
        {
            DrawAddProfile();
            return;
        }

        GUILayout.BeginVertical("box");
        {
            GUILayout.Label("Profiles");

            foreach (Profile profile in _profiles)
            {
                if (GUILayout.Button(profile.Name))
                {
                    _selectedProfile = profile;
                }
            }

            if(GUILayout.Button("+ Profile"))
            {
                _profileToAdd = new Profile();
            }
        }
        GUILayout.EndVertical();
    }

    private void DrawAddProfile()
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name");
            _profileToAdd.Name = GUILayout.TextField(_profileToAdd.Name);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                if(GUILayout.Button("Add"))
                {
                    _profiles.Add(_profileToAdd);
                    _profileToAdd = null;
                }

                if(GUILayout.Button("Cancel"))
                {
                    _profileToAdd = null;
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }
}
