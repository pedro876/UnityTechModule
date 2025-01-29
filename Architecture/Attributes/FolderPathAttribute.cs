#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Shows wheter the specified folder exists or not and allows to use the file explorer to set a new path.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class FolderPathAttribute : PropertyAttribute
{
    public FolderPathAttribute()
    {
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(FolderPathAttribute))]
public class FolderPathDrawer : PropertyDrawer
{
    private static GUIStyle errorStyle = null;
    private static Dictionary<string, bool> propertyFolderExists = new Dictionary<string, bool>();
    private static bool subscribedToUndo = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!subscribedToUndo)
        {
            Undo.undoRedoPerformed += () => propertyFolderExists.Clear();
            subscribedToUndo = true;
        }

        if (property.propertyType != SerializedPropertyType.String)
        {
            GUILayout.Label($"Property {property.displayName} with attribute {nameof(FilePathAttribute)} must be a string");
            return;
        }

        if (errorStyle == null)
        {
            errorStyle = new GUIStyle(GUI.skin.textField);
            Color nonExistentPathColor = new Color(1f, 0.2f, 0f, 1f);
            errorStyle.normal.textColor = nonExistentPathColor;
            errorStyle.active.textColor = nonExistentPathColor;
            errorStyle.focused.textColor = nonExistentPathColor;
            errorStyle.hover.textColor = nonExistentPathColor;
        }


        GUIContent folderIcon = EditorGUIUtility.IconContent("Folder Icon", "Select a path using the file explorer");

        using (new GUILayout.HorizontalScope())
        {
            FolderPathAttribute attrib = (FolderPathAttribute)attribute;

            if (!propertyFolderExists.TryGetValue(property.propertyPath, out bool directory))
            {
                SetDirectoryExists();
            }

            string path = EditorGUILayout.TextField(property.displayName, property.stringValue, directory ? EditorStyles.textField : errorStyle);

            if (path != property.stringValue)
            {
                property.stringValue = path;
                SetDirectoryExists();
            }

            if (GUILayout.Button(folderIcon, GUILayout.Width(32), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                if (!directory) path = "";
                path = EditorUtility.OpenFolderPanel("Select a folder", path, "");
                property.stringValue = path;
                SetDirectoryExists();
            }

            void SetDirectoryExists()
            {
                directory = Directory.Exists(property.stringValue);
                propertyFolderExists[property.propertyPath] = directory;
                //Debug.Log("Setting if folder exists for " + property.propertyPath);
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0f;
    }
}
#endif
