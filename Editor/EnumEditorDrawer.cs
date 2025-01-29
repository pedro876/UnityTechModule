using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnumEditorDrawer
{
    #region Variables

    private List<string> originalNames;
    private List<string> modifiedNames;
    private Action<(string[] original, string[] modified)> applyAction;

    public bool IsDraw { get; private set; }
    private Color backgroundColor;

    //Constants
    private const float buttonsHeigth = 22;
    private const float buttonsWitdth = 25;

    #endregion

    #region Base Methods

    public void Initialize<T>(Action<(string[] original, string[] modified)> applyAction) where T : Enum
    {
        backgroundColor = GUI.backgroundColor;
        this.applyAction = applyAction;

        T[] values = (T[])Enum.GetValues(typeof(T));
        int loop = values.Length == 0 ? -1 : Convert.ToInt32(values[values.Length - 1]);
        bool exist;

        originalNames = new();
        modifiedNames = new();

        for (int i = 0; i <= loop; i++)
        {
            exist = values.Contains((T)Enum.ToObject(typeof(T), i));

            originalNames.Add(exist ? $"{(T)Enum.ToObject(typeof(T), i)}" : string.Empty);
            modifiedNames.Add(exist ? $"{(T)Enum.ToObject(typeof(T), i)}" : string.Empty);
        }

        IsDraw = true;
    }

    public void Dispose()
    {
        originalNames = null;
        modifiedNames = null;
        applyAction = null;

        IsDraw = false;
    }

    public bool Draw()
    {
        if (!IsDraw) return false;

        GUIStyle ts = new(EditorStyles.textField);
        ts.alignment = TextAnchor.MiddleLeft;
        GUIContent c = CustomGUIStyles.RevertContent;
        c.tooltip = "Revert Original Value";

        bool anyModification = false;
        bool blockUpdate = false;

        string name;
        bool[] collisions = GetCollisions(modifiedNames);
        bool newElement;
        bool emptyElement;
        bool diferent;

        for (int i = 0; i < modifiedNames.Count; i++)
        {
            newElement = i >= originalNames.Count;
            diferent = !newElement && modifiedNames[i] != originalNames[i];
            emptyElement = !newElement && string.IsNullOrEmpty(originalNames[i]) && string.IsNullOrEmpty(modifiedNames[i]);
            anyModification = anyModification || diferent;
            blockUpdate = blockUpdate || newElement && string.IsNullOrEmpty(modifiedNames[i]);

            name =
                newElement ? "<New>" :
                emptyElement ? "<Empty>" :
                string.IsNullOrEmpty(modifiedNames[i]) ? $"<Delete: {originalNames[i]}>" :
                diferent ? $"<Modify: {(string.IsNullOrEmpty(originalNames[i]) ? modifiedNames[i] : originalNames[i])}>" :
                originalNames[i];

            GUI.backgroundColor = collisions[i] ? Color.red * new Color(0.9f, 0.9f, 0.9f, 1f) : backgroundColor;
            ts.normal.textColor = collisions[i] ? Color.red : Color.white;
            ts.hover.textColor = collisions[i] ? Color.red : Color.white;

            EditorGUILayout.BeginHorizontal();

            modifiedNames[i] = EditorGUILayout.TextField(name, modifiedNames[i], GUILayout.Height(buttonsHeigth));
            modifiedNames[i] = modifiedNames[i]?.Trim();
            modifiedNames[i] = modifiedNames[i]?.Replace(" ", "");

            if (string.IsNullOrEmpty(modifiedNames[i])) modifiedNames[i] = string.Empty;

            if (diferent && GUILayout.Button(c, GUILayout.Height(buttonsHeigth), GUILayout.Width(buttonsWitdth))) modifiedNames[i] = originalNames[i];

            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = backgroundColor;
        }

        EditorGUILayout.Space(5);

        anyModification = anyModification || originalNames.Count != modifiedNames.Count;
        blockUpdate = blockUpdate || !anyModification || collisions.Contains(true);

        GUILayout.BeginHorizontal();

        c = CustomGUIStyles.AddContent;
        c.tooltip = "Add Enum Element";

        if (GUILayout.Button(c, GUILayout.Height(buttonsHeigth), GUILayout.Width(buttonsWitdth))) modifiedNames.Add(string.Empty);

        c = CustomGUIStyles.DeleteContent;
        c.tooltip = "Delete Enum Element";

        if (GUILayout.Button(c, GUILayout.Height(buttonsHeigth), GUILayout.Width(buttonsWitdth))) modifiedNames.RemoveAt(modifiedNames.Count - 1);

        c = CustomGUIStyles.RevertContent;
        c.tooltip = "Revert Original Values";

        if (anyModification && GUILayout.Button(c, GUILayout.Height(buttonsHeigth), GUILayout.Width(buttonsWitdth)))
        {
            modifiedNames.Clear();
            for (int i = 0; i < originalNames.Count; i++) modifiedNames.Add(originalNames[i]);
        }

        GUILayout.EndHorizontal();

        GUI.enabled = !blockUpdate;

        c = CustomGUIStyles.EditContent;
        c.text = "Update Enum Element";

        if (GUILayout.Button(c, GUILayout.Height(buttonsHeigth)) && EditorUtility.DisplayDialog("Update", $"You wants to {c.text}?", "Continue", "Cancel"))
        {
            applyAction?.Invoke((originalNames.ToArray(), modifiedNames.ToArray()));

            originalNames.Clear();
            for (int i = 0; i < modifiedNames.Count; i++) originalNames.Add(modifiedNames[i]);
        }

        GUI.enabled = true;
        return true;
    }

    #endregion

    #region Utilities

    private bool[] GetCollisions(List<string> a)
    {
        bool[] colisions = new bool[a.Count];
        List<string> seens = new();

        for (int i = 0; i < a.Count; i++)
        {
            colisions[i] = !string.IsNullOrEmpty(a[i]) && seens.Contains(a[i]);
            seens.Add(a[i]);
        }

        seens = new();

        for (int i = a.Count - 1; i >= 0; i--)
        {
            colisions[i] = colisions[i] || !string.IsNullOrEmpty(a[i]) && seens.Contains(a[i]);
            seens.Add(a[i]);
        }

        return colisions;
    }

    #endregion
}
