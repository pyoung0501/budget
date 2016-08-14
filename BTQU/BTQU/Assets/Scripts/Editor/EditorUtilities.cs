using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


internal class EditorUtilities
{
    /// <summary>
    /// The stack tracking enabled states. 
    /// </summary>
    private static Stack<bool> _enabledStatck = new Stack<bool>();

    /// <summary>
    /// Whether or not the GUI is currently enabled.
    /// </summary>
    internal static bool Enabled { get { return _enabledStatck.Peek(); } }

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

    /// <summary>
    /// Draws a content width button.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool ContentWidthButton(string text)
    {
        return ContentWidthButton(text, Color.white, GUI.skin.button, true);
    }

    /// <summary>
    /// Draws a content width button with the given text and color.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="color">Color.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool ContentWidthButton(string text, Color color)
    {
        return ContentWidthButton(text, color, GUI.skin.button, true);
    }

    /// <summary>
    /// Draws a content width button with the given text and style.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="style">Style.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool ContentWidthButton(string text, GUIStyle style)
    {
        return ContentWidthButton(text, Color.white, style, true);
    }

    /// <summary>
    /// Draws a content width button with the given text and enabled state.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="enabled">Whether or not the button is enabled.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool ContentWidthButton(string text, bool enabled)
    {
        return ContentWidthButton(text, Color.white, GUI.skin.button, enabled);
    }

    /// <summary>
    /// Draws a content width button with the given text, color and style.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="color">Color.</param>
    /// <param name="style">Style.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool ContentWidthButton(string text, Color color, GUIStyle style)
    {
        return ContentWidthButton(text, color, style, true);
    }

    /// <summary>
    /// Draws a content width button with the given text, color and enabled state.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="color">Color.</param>
    /// <param name="enabled">Whether or not the button is enabled.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool ContentWidthButton(string text, Color color, bool enabled)
    {
        return ContentWidthButton(text, color, GUI.skin.button, enabled);
    }

    /// <summary>
    /// Draws a content width button with the given text, style and enabled state.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="style">Style.</param>
    /// <param name="enabled">Whether or not the button is enabled.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool ContentWidthButton(string text, GUIStyle style, bool enabled)
    {
        return ContentWidthButton(text, Color.white, style, enabled);
    }
    
    /// <summary>
    /// Draws a content width button with the given text, color, style and enabled state.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="color">Color.</param>
    /// <param name="style">Style.</param>
    /// <param name="enabled">Whether or not the button is enabled.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool ContentWidthButton(string text, Color color, GUIStyle style, bool enabled)
    {
        GUIContent buttonContent = new GUIContent(text);
        Vector2 buttonSize = style.CalcSize(buttonContent);
        return Button(buttonContent, color, style, enabled, GUILayout.Width(buttonSize.x));
    }

    /// <summary>
    /// Draws a button.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="layoutOptions">Optional layout options.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool Button(string text, params GUILayoutOption[] layoutOptions)
    {
        return Button(new GUIContent(text), Color.white, GUI.skin.button, true, layoutOptions);
    }

    /// <summary>
    /// Draws a button.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="color">Color.</param>
    /// <param name="layoutOptions">Optional layout options.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool Button(string text, Color color, params GUILayoutOption[] layoutOptions)
    {
        return Button(new GUIContent(text), color, GUI.skin.button, true, layoutOptions);
    }

    /// <summary>
    /// Draws a button.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="style">Style.</param>
    /// <param name="layoutOptions">Optional layout options.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool Button(string text, GUIStyle style, params GUILayoutOption[] layoutOptions)
    {
        return Button(new GUIContent(text), Color.white, style, true, layoutOptions);
    }

    /// <summary>
    /// Draws a button.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="enabled">Whether or not the button is enabled.</param>
    /// <param name="layoutOptions">Optional layout options.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool Button(string text, bool enabled, params GUILayoutOption[] layoutOptions)
    {
        return Button(new GUIContent(text), Color.white, GUI.skin.button, enabled, layoutOptions);
    }

    /// <summary>
    /// Draws a button.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="color">Color.</param>
    /// <param name="style">Style.</param>
    /// <param name="layoutOptions">Optional layout options.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool Button(string text, Color color, GUIStyle style, params GUILayoutOption[] layoutOptions)
    {
        return Button(new GUIContent(text), color, style, true, layoutOptions);
    }

    /// <summary>
    /// Draws a button.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="color">Color.</param>
    /// <param name="enabled">Whether or not the button is enabled.</param>
    /// <param name="layoutOptions">Optional layout options.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool Button(string text, Color color, bool enabled, params GUILayoutOption[] layoutOptions)
    {
        return Button(new GUIContent(text), color, GUI.skin.button, enabled, layoutOptions);
    }

    /// <summary>
    /// Draws a button.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="style">Style.</param>
    /// <param name="enabled">Whether or not the button is enabled.</param>
    /// <param name="layoutOptions">Optional layout options.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool Button(string text, GUIStyle style, bool enabled, params GUILayoutOption[] layoutOptions)
    {
        return Button(new GUIContent(text), Color.white, style, enabled, layoutOptions);
    }

    /// <summary>
    /// Draws a button.
    /// </summary>
    /// <param name="content">Content.</param>
    /// <param name="color">Color.</param>
    /// <param name="layoutOptions">Optional layout options.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool Button(GUIContent content, Color color, params GUILayoutOption[] layoutOptions)
    {
        return Button(content, color, GUI.skin.button, true, layoutOptions);
    }

    /// <summary>
    /// Draws a button.
    /// </summary>
    /// <param name="content">Content.</param>
    /// <param name="style">Style.</param>
    /// <param name="layoutOptions">Optional layout options.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool Button(GUIContent content, GUIStyle style, params GUILayoutOption[] layoutOptions)
    {
        return Button(content, Color.white, style, true, layoutOptions);
    }

    /// <summary>
    /// Draws a button.
    /// </summary>
    /// <param name="content">Content.</param>
    /// <param name="color">Color.</param>
    /// <param name="style">Style.</param>
    /// <param name="layoutOptions">Optional layout options.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool Button(GUIContent content, Color color, GUIStyle style, params GUILayoutOption[] layoutOptions)
    {
        return Button(content, color, style, true, layoutOptions);
    }

    /// <summary>
    /// Draws a button.
    /// </summary>
    /// <param name="content">Content.</param>
    /// <param name="color">Color.</param>
    /// <param name="style">Style.</param>
    /// <param name="enabled">Whether or not the button is enabled.</param>
    /// <param name="layoutOptions">Optional layout options.</param>
    /// <returns>True if pressed; otherwise, false.</returns>
    internal static bool Button(GUIContent content, Color color, GUIStyle style, bool enabled, params GUILayoutOption[] layoutOptions)
    {
        BeginEnabled(enabled);
        BeginBackgroundColor(color);
        bool pressed = GUILayout.Button(content, style, layoutOptions);
        EndBackgroundColor();
        EndEnabled();

        return pressed;
    }
    
    private static Stack<Color> _backgroundColorStack = new Stack<Color>();
    private static Stack<Color> _foregroundColorStack = new Stack<Color>();
    internal static Color BackgroundColor { get { return _backgroundColorStack.Peek(); } }
    internal static Color ForegroundColor { get { return _foregroundColorStack.Peek(); } }

    /// <summary>
    /// Begins a background color section.
    /// </summary>
    /// <param name="color">Color to set background to.</param>
    internal static void BeginBackgroundColor(Color color)
    {
        _backgroundColorStack.Push(GUI.backgroundColor);
        GUI.backgroundColor = color;
    }

    /// <summary>
    /// Ends a background color section.
    /// </summary>
    internal static void EndBackgroundColor()
    {
        GUI.backgroundColor = _backgroundColorStack.Pop();
    }

    /// <summary>
    /// Begins a foreground color section.
    /// </summary>
    /// <param name="color">Color to set foreground to.</param>
    internal static void BeginForegroundColor(Color color)
    {
        _foregroundColorStack.Push(GUI.contentColor);
        GUI.contentColor = color;
    }

    /// <summary>
    /// Ends a foreground color section.
    /// </summary>
    internal static void EndForegroundColor()
    {
        GUI.contentColor = _foregroundColorStack.Pop();
    }

    /// <summary>
    /// Begins an enabled state.
    /// </summary>
    /// <param name="enable">Whether or not to enable the GUI.</param>
    internal static void BeginEnabled(bool enable)
    {
        _enabledStatck.Push(GUI.enabled);
        GUI.enabled = enable;
    }

    /// <summary>
    /// Ends the last enabled state.
    /// </summary>
    internal static void EndEnabled()
    {
        GUI.enabled = _enabledStatck.Pop();
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