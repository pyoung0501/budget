using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


internal class EditorUtilities
{
    internal static void ContentWidthLabel(string text, GUIStyle style = null)
    {
        if(style == null)
        {
            style = GUI.skin.label;
        }

        GUIContent labelContent = new GUIContent(text);
        Vector2 labelSize = style.CalcSize(labelContent);
        EditorGUILayout.LabelField(labelContent, style, GUILayout.Width(labelSize.x));
    }

    internal static bool ContentWidthButton(string text, GUIStyle style = null)
    {
        if (style == null)
        {
            style = GUI.skin.button;
        }

        GUIContent buttonContent = new GUIContent(text);
        Vector2 buttonSize = style.CalcSize(buttonContent);
        return Button(buttonContent, style, GUILayout.Width(buttonSize.x));
    }

    internal static bool ContentWidthButton(string text, Color color, GUIStyle style = null)
    {
        if (style == null)
        {
            style = GUI.skin.button;
        }

        GUIContent buttonContent = new GUIContent(text);
        Vector2 buttonSize = style.CalcSize(buttonContent);
        return Button(buttonContent, color, style, GUILayout.Width(buttonSize.x));
    }

    internal static bool Button(string buttonText, params GUILayoutOption[] guiLayoutOptions)
    {
        return Button(new GUIContent(buttonText), Color.white, GUI.skin.button, guiLayoutOptions);
    }

    internal static bool Button(string buttonText, Color color, params GUILayoutOption[] guiLayoutOptions)
    {
        return Button(new GUIContent(buttonText), color, GUI.skin.button, guiLayoutOptions);
    }

    internal static bool Button(string buttonText, GUIStyle style, params GUILayoutOption[] guiLayoutOptions)
    {
        return Button(new GUIContent(buttonText), Color.white, style, guiLayoutOptions);
    }

    internal static bool Button(GUIContent buttonContent, GUIStyle style, params GUILayoutOption[] guiLayoutOptions)
    {
        return Button(buttonContent, Color.white, style, guiLayoutOptions);
    }

    internal static bool Button(GUIContent buttonContent, Color color, GUIStyle style, params GUILayoutOption[] guiLayoutOptions)
    {
        BeginBackgroundColor(color);
        bool pressed = GUILayout.Button(buttonContent, style, guiLayoutOptions);
        EndBackgroundColor();

        return pressed;
    }
    
    private static Stack<Color> _backgroundColorStack = new Stack<Color>();
    internal static Color BackgroundColor { get { return _backgroundColorStack.Peek(); } }

    internal static void BeginBackgroundColor(Color color)
    {
        _backgroundColorStack.Push(GUI.backgroundColor);
        GUI.backgroundColor = color;
    }

    internal static void EndBackgroundColor()
    {
        GUI.backgroundColor = _backgroundColorStack.Pop();
    }

    internal static void BeginRightJustify()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
    }

    internal static void EndRightJustify()
    {
        EditorGUILayout.EndHorizontal();
    }

    internal static void BeginHorizontalCentering()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
    }

    internal static void BeginVerticalCentering()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
    }

    internal static void EndVerticalCentering()
    {
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();
    }

    internal static void EndHorizontalCentering()
    {
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }
}