using BTQ;
using BTQLib;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

public class BTQEditor : EditorWindow
{
    [MenuItem("BTQ/BTQ Editor")]
    private static void OpenEditor()
    {
        EditorWindow.GetWindow<BTQEditor>();
    }
    
    private Profile _profileToAdd = null;

    private Profile _selectedProfile;

    private ProfileController _profileController;

    private string[] _profileFiles;

    private bool _initialized;

    private Vector2 _scrollPos;

    private void OnGUI()
    {
        if (!_initialized)
        {
            Initialize();
        }

        _scrollPos =
        EditorGUILayout.BeginScrollView(_scrollPos);
        {
            if (_selectedProfile != null)
            {
                DrawSelectedProfile();
            }
            else if (_profileToAdd != null)
            {
                EditorUtilities.BeginHorizontalCentering();
                EditorUtilities.BeginVerticalCentering();
                DrawAddProfile();
                EditorUtilities.EndVerticalCentering();
                EditorUtilities.EndHorizontalCentering();
            }
            else
            {
                EditorUtilities.BeginVerticalCentering();
                DrawProfiles();
                EditorUtilities.EndVerticalCentering();
            }
        }
        EditorGUILayout.EndScrollView();

        if (_actionQueue.Count > 0)
        {
            while (_actionQueue.Count > 0)
            {
                Action action = _actionQueue.Dequeue();
                action();
            }
        }
    }

    Queue<Action> _actionQueue = new Queue<Action>();

    private void DrawProfiles()
    {
        if (_profileFiles.Length > 0)
        {
            EditorUtilities.BeginHorizontalCentering();
            EditorUtilities.ContentWidthLabel("Select a Profile", EditorStyles.boldLabel);
            EditorUtilities.EndHorizontalCentering();

            GUILayout.Space(16.0f);

            EditorUtilities.BeginHorizontalCentering();
            GUILayout.BeginVertical("box", GUILayout.MinWidth(100.0f), GUILayout.MaxWidth(200.0f));
            {
                foreach (string profileFile in _profileFiles)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        string profileName = Path.GetFileNameWithoutExtension(profileFile);

                        if (GUILayout.Button(profileName))
                        {
                            SelectProfile(profileFile);
                        }

                        if (EditorUtilities.ContentWidthButton("-"))
                        {
                            bool remove = EditorUtility.DisplayDialog("Remove Profile",
                                                                      string.Format("Are you sure you want to remove the {0} profile?", profileName),
                                                                      "Yes",
                                                                      "No");
                            if (remove)
                            {
                                string filepath = new string(profileFile.ToCharArray());
                                _actionQueue.Enqueue(() => { File.Delete(filepath); UpdateProfileList(); Debug.Log(filepath); });
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
            EditorUtilities.EndHorizontalCentering();
        }

        GUILayout.Space(16.0f);

        EditorUtilities.BeginHorizontalCentering();
        if (EditorUtilities.ContentWidthButton("+ Profile"))
        {
            _profileToAdd = new Profile();
        }
        EditorUtilities.EndHorizontalCentering();
    }

    private void UpdateProfileList()
    {
        if (Directory.Exists(Common.ProfilesDirectory))
        {
            string[] profileFiles = Directory.GetFiles(Common.ProfilesDirectory);
            _profileFiles = profileFiles.Where((fileName) => fileName.EndsWith(".profile")).ToArray();
        }
        else
        {
            _profileFiles = new string[0];
        }
    }

    private void DrawSelectedProfile()
    {
        if (_profileController == null)
        {
            _profileController = new ProfileController(_selectedProfile);
        }

        DrawSelectedProfileHeader();

        if (_profileController != null)
        {
            _profileController.DrawView();
        }
    }

    private void DrawSelectedProfileHeader()
    {
        EditorGUILayout.BeginHorizontal("box");
        {
            if (EditorUtilities.ContentWidthButton("< Profiles"))
            {
                _selectedProfile = null;
                _profileController = null;
            }
            else
            {
                EditorUtilities.ContentWidthLabel(string.Format("{0} (Active Profile)", _selectedProfile.Name));

                EditorUtilities.BeginRightJustify();
                {
                    if (GUILayout.Button("Save"))
                    {
                        Common.SaveProfile(_selectedProfile);
                    }
                }
                EditorUtilities.EndRightJustify();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private Profile LoadProfile(string profileFile)
    {
        using (StreamReader stream = new StreamReader(profileFile))
        {
            return JsonConvert.DeserializeObject(stream.ReadToEnd(), typeof(Profile)) as Profile;
        }
    }

    private void Initialize()
    {
        UpdateProfileList();
        SelectSingleProfileByDefault();

        _initialized = true;
    }

    /// <summary>
    /// This will select a profile by default if there is
    /// exactly one profile.
    /// </summary>
    private void SelectSingleProfileByDefault()
    {
        if(_profileFiles.Length == 1)
        {
            SelectProfile(_profileFiles[0]);
        }
    }

    /// <summary>
    /// Selects the profile specified by its filepath.
    /// </summary>
    /// <param name="profileFile">Filepath of a profile.</param>
    private void SelectProfile(string profileFile)
    {
        Profile profile = LoadProfile(profileFile);
        if (profile != null)
        {
            _selectedProfile = profile;
        }
    }

    private void DrawAddProfile()
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name");
            _profileToAdd.Name = GUILayout.TextField(_profileToAdd.Name, GUILayout.Width(200.0f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add"))
                {
                    Common.SaveProfile(_profileToAdd);
                    Initialize();
                    _profileToAdd = null;
                }

                if (GUILayout.Button("Cancel"))
                {
                    _profileToAdd = null;
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }
}
