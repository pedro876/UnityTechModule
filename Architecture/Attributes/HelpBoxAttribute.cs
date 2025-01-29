//https://vintay.medium.com/creating-custom-unity-attributes-readonly-d279e1e545c9
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

/// <summary>
/// Shows a help box with the provided text over the field
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class HelpBoxAttribute : PropertyAttribute
{
    public string helpText;
    public bool isWarning;

    public HelpBoxAttribute(string helpText, bool isWarning = false)
    {
        this.helpText = helpText;
        this.isWarning = isWarning;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
public class HelpBoxPropertyDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        HelpBoxAttribute attrib = (HelpBoxAttribute)attribute;
        EditorGUILayout.HelpBox(attrib.helpText, attrib.isWarning ? MessageType.Warning : MessageType.Info);
        EditorGUILayout.PropertyField(property, label);
        //EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0f;
        //return EditorGUI.GetPropertyHeight(property, label, true);
        //return -EditorGUIUtility.standardVerticalSpacing;

    }
}
#endif
