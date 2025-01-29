using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateShaderTemplates
{
    [MenuItem("Assets/TechModule/Shader Templates/Unlit")]
    private static void CreateTemplate_Unlit()
    {
        const string FILE_NAME = "NewUnlitShader";

        string newFilePath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (File.Exists(newFilePath))
        {
            newFilePath = Path.GetDirectoryName(newFilePath);
        }

        newFilePath = $"{newFilePath}\\{FILE_NAME}.shader";

        string relativePath = "ShaderTemplates/ShaderTemplate_Unlit.txt";
        string assetName = Path.GetFileNameWithoutExtension(relativePath);

        string[] matches = AssetDatabase.FindAssets(assetName);

        for (int i = 0; i < matches.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(matches[0]);
            if (assetPath.Contains(relativePath))
            {
                string text = File.ReadAllText(assetPath);
                File.WriteAllText(newFilePath, text);
                AssetDatabase.Refresh();
                return;
            }
        }

        Debug.LogError($"Couldn't find template: {relativePath}");

        

    }
}
