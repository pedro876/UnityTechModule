//https://vintay.medium.com/creating-custom-unity-attributes-readonly-d279e1e545c9
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;


/// <summary>
/// Will disable gui interaction if the specified bool value is true.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class DisableIfAttribute : PropertyAttribute
{
    public bool isEnum;
    public string variableName;
    public int enumValue;

    public DisableIfAttribute(string boolVariableName)
    {
        this.isEnum = false;
        this.variableName = boolVariableName;
    }

    public DisableIfAttribute(string enumVariableName, int enumValue)
    {
        this.isEnum = true;
        this.variableName = enumVariableName;
        this.enumValue = enumValue;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DisableIfAttribute))]
public class DisableIfPropertyDrawer : PropertyDrawer
{
    public bool ShouldBeInteractable(SerializedProperty property)
    {
        DisableIfAttribute attrib = (DisableIfAttribute)attribute;
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
            shouldBeInteractable = (int)(object)attrib.enumValue != variableProperty.enumValueIndex;
        }
        else
        {
            shouldBeInteractable = !variableProperty.boolValue;
        }
        return shouldBeInteractable;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = GUI.enabled && ShouldBeInteractable(property);
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif
