#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Shows wheter the specified file exists or not and allows to use the file explorer to set a new path.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class FilePathAttribute : PropertyAttribute
{
    public string extension;

    public FilePathAttribute(string extension = "")
    {
        this.extension = extension;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(FilePathAttribute))]
public class FilePathDrawer : PropertyDrawer
{
    private static GUIStyle errorStyle = null;
    private static Dictionary<string, bool> propertyFileExists = new Dictionary<string, bool>();
    private static bool subscribedToUndo = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!subscribedToUndo)
        {
            Undo.undoRedoPerformed += () => propertyFileExists.Clear();
            subscribedToUndo = true;
        }

        if(property.propertyType != SerializedPropertyType.String)
        {
            GUILayout.Label($"Property {property.displayName} with attribute {nameof(FilePathAttribute)} must be a string");
            return;
        }

        if(errorStyle == null)
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
            FilePathAttribute attrib = (FilePathAttribute)attribute;

            if (!propertyFileExists.TryGetValue(property.propertyPath, out bool fileExists))
            {
                SetFileExists();
            }

            string path = EditorGUILayout.TextField(property.displayName, property.stringValue, fileExists ? EditorStyles.textField : errorStyle);
            
            if(path != property.stringValue)
            {
                property.stringValue = path;
                SetFileExists();
            }

            if (GUILayout.Button(folderIcon, GUILayout.Width(32), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                if (!fileExists) path = "";
                path = EditorUtility.OpenFilePanel("Select a file", path, attrib.extension);
                property.stringValue = path;
                SetFileExists();
            }

            void SetFileExists()
            {
                fileExists = File.Exists(property.stringValue);
                propertyFileExists[property.propertyPath] = fileExists;
                //Debug.Log("Setting if file exists for " + property.propertyPath);
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0f;
    }
}
#endif
