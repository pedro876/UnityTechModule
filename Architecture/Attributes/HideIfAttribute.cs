//https://vintay.medium.com/creating-custom-unity-attributes-readonly-d279e1e545c9
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

/// <summary>
/// Hides the field if the specified bool value is true.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class HideIfAttribute : PropertyAttribute
{
    public bool isEnum;
    public string variableName;
    public int enumValue;

    public HideIfAttribute(string boolVariableName)
    {
        this.isEnum = false;
        this.variableName = boolVariableName;
    }

    public HideIfAttribute(string enumVariableName, int enumValue)
    {
        this.isEnum = true;
        this.variableName = enumVariableName;
        this.enumValue = enumValue;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(HideIfAttribute))]
public class HideIfPropertyDrawer : PropertyDrawer
{
    public bool CanBeShown(SerializedProperty property)
    {
        HideIfAttribute attrib = (HideIfAttribute)attribute;
        string variableName = PropertyDrawerUtil.GetRelativePropertyPath(property, attrib.variableName);
        SerializedProperty variableProperty = property.serializedObject.FindProperty(variableName);
        bool canBeShown;
        if (variableProperty == null)
        {
            Debug.LogError($"Could not find property {variableName} for property {property.name} at object {property.serializedObject.targetObject.name}");
            canBeShown = true;
        }
        else if (attrib.isEnum)
        {
            canBeShown = (int)(object)attrib.enumValue != variableProperty.enumValueIndex;
        }
        else
        {
            canBeShown = !variableProperty.boolValue;
        }
        return canBeShown;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (CanBeShown(property))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (CanBeShown(property))
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        else return -EditorGUIUtility.standardVerticalSpacing;

    }
}
#endif
