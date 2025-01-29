using Codice.CM.Common;
using Rendering;
using UnityEditor;
using UnityEngine;
using static TreeEditor.TextureAtlas;


public class Noise3DObject : ScriptableObject
{
    [SerializeField] public bool autoSelect = true;
    [SerializeField] public string textureName = "";
    [SerializeField] public int resolution = 64;
    [SerializeField] public bool generateMipMaps = false;
    [SerializeField] public int seed = 0;
    [SerializeField] public int cells = 10;
    [SerializeField] public bool smoothInterpolation = true;
    [SerializeField] public float edge0 = 0f;
    [SerializeField] public float edge1 = 1f;
    [SerializeField] public float power = 1f;
    [SerializeField] public int octaveLayers = 1;
    [SerializeField] public bool withNormals = false;
    [SerializeField] public float normalsDisplacement = 0.05f;
    [SerializeField] public int normalSamples = 64;
    [SerializeField] public Noise3DGenerator.Method method = Noise3DGenerator.Method.Gradient;
}

[CustomEditor(typeof(Noise3DObject))]
public class Noise3DObjectEditor : Editor
{
    SerializedProperty autoSelect;
    SerializedProperty textureName;
    SerializedProperty resolution;
    SerializedProperty generateMipMaps;
    SerializedProperty seed;
    SerializedProperty cells;
    SerializedProperty smoothInterpolation;
    SerializedProperty edge0;
    SerializedProperty edge1;
    SerializedProperty power;
    SerializedProperty octaveLayers;
    SerializedProperty withNormals;
    SerializedProperty normalsDisplacement;
    SerializedProperty normalSamples;
    SerializedProperty method;

    private void OnEnable()
    {
        autoSelect = serializedObject.FindProperty(nameof(autoSelect));
        textureName = serializedObject.FindProperty(nameof(textureName));
        resolution = serializedObject.FindProperty(nameof(resolution));
        generateMipMaps = serializedObject.FindProperty(nameof(generateMipMaps));
        seed = serializedObject.FindProperty(nameof(seed));
        cells = serializedObject.FindProperty(nameof(cells));
        smoothInterpolation = serializedObject.FindProperty(nameof(smoothInterpolation));
        edge0 = serializedObject.FindProperty(nameof(edge0));
        edge1 = serializedObject.FindProperty(nameof(edge1));
        power = serializedObject.FindProperty(nameof(power));
        octaveLayers = serializedObject.FindProperty(nameof(octaveLayers));
        withNormals = serializedObject.FindProperty(nameof(withNormals));
        normalsDisplacement = serializedObject.FindProperty(nameof(normalsDisplacement));
        normalSamples = serializedObject.FindProperty(nameof(normalSamples));
        method = serializedObject.FindProperty(nameof(method));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(autoSelect);
        EditorGUILayout.PropertyField(textureName);
        EditorGUILayout.PropertyField(resolution);
        EditorGUILayout.PropertyField(generateMipMaps);
        EditorGUILayout.PropertyField(seed);
        EditorGUILayout.PropertyField(cells);
        EditorGUILayout.PropertyField(smoothInterpolation);

        if (smoothInterpolation.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(edge0);
            EditorGUILayout.PropertyField(edge1);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(power);
        EditorGUILayout.PropertyField(octaveLayers);
        GUILayout.Space(20);
        EditorGUILayout.PropertyField(withNormals);

        if (withNormals.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(normalsDisplacement);
            EditorGUILayout.PropertyField(normalSamples);
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.PropertyField(method);


        GUILayout.Space(20);
        if (GUILayout.Button("GENERATE"))
        {
            Texture3D tex = Noise3DGenerator.Generate(textureName.stringValue, resolution.intValue, seed.intValue, cells.intValue, edge0.floatValue, edge1.floatValue,
                power.floatValue, smoothInterpolation.boolValue, octaveLayers.intValue, generateMipMaps.boolValue, (Noise3DGenerator.Method)method.enumValueIndex, withNormals.boolValue,
                normalsDisplacement.floatValue, normalSamples.intValue);
            if (autoSelect.boolValue)
            {
                EditorGUIUtility.PingObject(tex);
                Selection.activeObject = tex;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
