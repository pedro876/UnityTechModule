//https://vintay.medium.com/creating-custom-unity-attributes-readonly-d279e1e545c9
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

/// <summary>
/// Will enable gui interaction if the specified bool value is true.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class EnableIfAttribute : PropertyAttribute
{
    public bool isEnum;
    public string variableName;
    public int enumValue;

    public EnableIfAttribute(string boolVariableName)
    {
        this.isEnum = false;
        this.variableName = boolVariableName;
    }

    public EnableIfAttribute(string enumVariableName, int enumValue)
    {
        this.isEnum = true;
        this.variableName = enumVariableName;
        this.enumValue = enumValue;
    }
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EnableIfAttribute))]
public class EnableIfPropertyDrawer : PropertyDrawer
{
    public bool ShouldBeInteractable(SerializedProperty property)
    {
        EnableIfAttribute attrib = (EnableIfAttribute)attribute;
        string variableName = PropertyDrawerUtil.GetRelativePropertyPath(property, attrib.variableName);
        SerializedProperty variableProperty = property.serializedObject.FindProperty(variableName);
        bool shouldBeInteractable;
        if (variableProperty == null)
        {
            Debug.LogError($"Could not find property {variableName} for property {property.name} at object {property.serializedObject.targetObject.name}");
            shouldBeInteractable = true;
        }
        else if (attrib.isEnum)
        {
            shouldBeInteractable = (int)(object)attrib.enumValue == variableProperty.enumValueIndex;
        }
        else
        {
            shouldBeInteractable = variableProperty.boolValue;
        }
        return shouldBeInteractable;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = ShouldBeInteractable(property);
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif