#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

/// <summary>
/// Use this attribute to force a shader to be referenced. The path specified 
/// is the full name of the shader, which can be seen
/// at the beginning of the shader file.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ForceShaderAttribute : PropertyAttribute
{
    public string shaderPath;

    public ForceShaderAttribute(string shaderPath)
    {
        this.shaderPath = shaderPath;
    }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ForceShaderAttribute))]
public class ForceShaderPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ForceShaderAttribute forceShaderAttribute = attribute as ForceShaderAttribute;
        Shader currentShader = property.objectReferenceValue as Shader;
        bool needsFind = (currentShader == null || currentShader.name != forceShaderAttribute.shaderPath)
            && forceShaderAttribute.shaderPath != null && forceShaderAttribute.shaderPath.Length > 0;

        if (needsFind)
        {
            property.objectReferenceValue = Shader.Find(forceShaderAttribute.shaderPath);
        }

        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;
    }
}
#endif
}
