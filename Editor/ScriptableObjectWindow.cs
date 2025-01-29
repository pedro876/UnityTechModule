using System.IO;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Allows to display a window to customize a scriptable object.
/// The scriptable object name must be the same as its type and it must be located
/// in the same directory as the window editor script for its type.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ScriptableObjectWindow<T> : EditorWindow where T : ScriptableObject
{
    //EXAMPLE OF HOW TO GET THE WINDOW TO SHOW UP IN UNITY:
    //[MenuItem("PitGL/Shader Finder")]
    //public static void ShowWindow()
    //{
    //    var window = EditorWindow.GetWindow(typeof(ShaderFinderWindow));
    //    window.name = "Shader Finder";
    //}

    private string relativeFolderPath;
    private string objectPath;

    private SerializedObject serializedObject;
    private T scriptableObj;
    private Editor scriptableObjEditor;

    private Vector2 _scroll;

    private void OnEnable()
    {
        relativeFolderPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));
        objectPath = $"{relativeFolderPath}\\{typeof(T).Name}.asset";

        scriptableObj = AssetDatabase.LoadAssetAtPath<T>(objectPath);

        if (scriptableObj != null)
        {
            serializedObject = new SerializedObject(scriptableObj);
        }

    }

    private void OnGUI()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        if (scriptableObj == null)
        {
            if (GUILayout.Button("Create serialized data"))
            {
                T newInstance = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(newInstance, objectPath);
                scriptableObj = AssetDatabase.LoadAssetAtPath<T>(objectPath);
                serializedObject = new SerializedObject(scriptableObj);
            }
            return;
        }

        if (scriptableObjEditor == null || scriptableObjEditor.target != scriptableObj)
        {
            scriptableObjEditor = Editor.CreateEditor(scriptableObj);
        }

        OnGUIBeforeScriptableObject(scriptableObj);

        scriptableObjEditor.OnInspectorGUI();

        OnGUIAfterScriptableObject(scriptableObj);

        EditorGUILayout.EndScrollView();
    }

    protected virtual void OnGUIAfterScriptableObject(T scriptableObject) { }
    protected virtual void OnGUIBeforeScriptableObject(T scriptableObject) { }
}

