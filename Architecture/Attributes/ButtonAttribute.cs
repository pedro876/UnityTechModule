using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// Place this attribute on a monobehaviour method to display it as a button in the inspector.
/// You can call private and public methods, but not static methods for now.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class ButtonAttribute : PropertyAttribute
{
    public string Label { get; }

    public ButtonAttribute(string label = null)
    {
        Label = label;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MonoBehaviour), true)]
public class ButtonAttributeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();
        GUILayout.Space(6);
        // Get the inspected object
        MonoBehaviour targetMonoBehaviour = (MonoBehaviour)target;

        // Get all methods in the class
        var methods = targetMonoBehaviour.GetType().GetMethods(
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic);

        foreach (var method in methods)
        {
            // Check if the method has the ButtonAttribute
            var buttonAttribute = (ButtonAttribute)Attribute.GetCustomAttribute(method, typeof(ButtonAttribute));
            if (buttonAttribute != null)
            {
                // Use the attribute's label or the method name if no label is provided
                string buttonLabel = string.IsNullOrEmpty(buttonAttribute.Label) ? method.Name : buttonAttribute.Label;

                if (GUILayout.Button(buttonLabel))
                {
                    // Invoke the method when the button is clicked
                    method.Invoke(targetMonoBehaviour, null);
                }
            }
        }
    }
}
#endif