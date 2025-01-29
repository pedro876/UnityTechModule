using UnityEditor;
using UnityEngine;

public static class CustomGUIStyles
{
    #region Label Styles

    private static GUIStyle titleStyle;
    public static GUIStyle TitleStyle
    {
        get
        {
            if (titleStyle == null)
            {
                titleStyle = new(EditorStyles.linkLabel);
                titleStyle.normal.textColor = Color.white;
                titleStyle.fontStyle = FontStyle.BoldAndItalic;
                titleStyle.fontSize = 15;
            }

            return titleStyle;
        }
    }

    private static GUIStyle subtitleStyle;
    public static GUIStyle SubtitleStyle
    {
        get
        {
            if (subtitleStyle == null)
            {
                subtitleStyle = new(EditorStyles.linkLabel);
                subtitleStyle.normal.textColor = Color.white;
                subtitleStyle.fontStyle = FontStyle.Bold;
            }

            return subtitleStyle;
        }
    }

    #endregion

    #region Foldout Styles

    private static GUIStyle titleFoldoutStyle;
    public static GUIStyle TitleFoldoutStyle
    {
        get
        {
            if (titleFoldoutStyle == null)
            {
                titleFoldoutStyle = new(EditorStyles.foldout);
                titleFoldoutStyle.normal.textColor = Color.white;
                titleFoldoutStyle.fontStyle = FontStyle.BoldAndItalic;
                titleFoldoutStyle.fontSize = 15;
            }

            return titleFoldoutStyle;
        }
    }

    private static GUIStyle subtitleFoldoutStyle;
    public static GUIStyle SubtitleFoldoutStyle
    {
        get
        {
            if (subtitleFoldoutStyle == null)
            {
                subtitleFoldoutStyle = new(EditorStyles.foldout);
                subtitleFoldoutStyle.normal.textColor = Color.white;
                subtitleFoldoutStyle.fontStyle = FontStyle.BoldAndItalic;
            }

            return subtitleFoldoutStyle;
        }
    }

    #endregion

    #region Button Styles

    private static GUIStyle normalButton;
    public static GUIStyle NormalButton
    {
        get
        {
            if (normalButton == null)
            {
                normalButton = new(GUI.skin.button);
                normalButton.hover.textColor = Color.black;
                normalButton.alignment = TextAnchor.MiddleLeft;
            }

            return normalButton;
        }
    }

    #endregion

    #region Content Styles

    private static GUIStyle darkContent;
    public static GUIStyle DarkContent
    {
        get
        {
            if (darkContent == null)
            {
                Color[] pixels = new Color[2 * 2];
                Texture2D texture = new(2, 2);

                for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color(0.15f, 0.15f, 0.15f, 3f);

                texture.SetPixels(pixels);
                texture.Apply();

                darkContent = new GUIStyle(GUI.skin.box);
                darkContent.normal.background = texture;
            }

            return darkContent;
        }
    }

    private static GUIStyle lightContent;
    public static GUIStyle LightContent
    {
        get
        {
            if (lightContent == null)
            {
                Color[] pixels = new Color[2 * 2];
                Texture2D texture = new(2, 2);

                for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color(0.2f, 0.2f, 0.2f, 3f);

                texture.SetPixels(pixels);
                texture.Apply();

                lightContent = new GUIStyle(GUI.skin.box);
                lightContent.normal.background = texture;
            }

            return lightContent;
        }
    }

    private static GUIStyle helpBoxContent;
    public static GUIStyle HelpBoxContent
    {
        get
        {
            if (helpBoxContent == null)
            {
                helpBoxContent = EditorStyles.helpBox;
            }

            return helpBoxContent;
        }
    }

    #endregion

    #region Button GUIContents

    private static GUIContent sceneContent;
    public static GUIContent SceneContent
    {
        get
        {
            if (sceneContent == null) sceneContent = EditorGUIUtility.IconContent("SceneAsset Icon");
            CleanGUIContent(sceneContent);

            return sceneContent;
        }
    }

    private static GUIContent prefabContent;
    public static GUIContent PrefabContent
    {
        get
        {
            if (prefabContent == null) prefabContent = EditorGUIUtility.IconContent("Prefab Icon");
            CleanGUIContent(prefabContent);

            return prefabContent;
        }
    }

    private static GUIContent scriptableContent;
    public static GUIContent ScriptableContent
    {
        get
        {
            if (scriptableContent == null) scriptableContent = EditorGUIUtility.IconContent("ScriptableObject Icon");
            CleanGUIContent(scriptableContent);

            return scriptableContent;
        }
    }

    private static GUIContent settingsContent;
    public static GUIContent SettingsContent
    {
        get
        {
            if (settingsContent == null) settingsContent = EditorGUIUtility.IconContent("SettingsIcon");
            CleanGUIContent(settingsContent);

            return settingsContent;
        }
    }

    private static GUIContent editContent;
    public static GUIContent EditContent
    {
        get
        {
            if (editContent == null) editContent = EditorGUIUtility.IconContent("editicon.sml");
            CleanGUIContent(editContent);

            return editContent;
        }
    }

    private static GUIContent deleteContent;
    public static GUIContent DeleteContent
    {
        get
        {
            if (deleteContent == null) deleteContent = EditorGUIUtility.IconContent("TreeEditor.Trash");
            CleanGUIContent(deleteContent);

            return deleteContent;
        }
    }

    private static GUIContent addContent;
    public static GUIContent AddContent
    {
        get
        {
            if (addContent == null) addContent = EditorGUIUtility.IconContent("Toolbar Plus");
            CleanGUIContent(addContent);

            return addContent;
        }
    }

    private static GUIContent backContent;
    public static GUIContent BackContent
    {
        get
        {
            if (backContent == null) backContent = EditorGUIUtility.IconContent("Animation.PrevKey");
            CleanGUIContent(backContent);

            return backContent;
        }
    }

    private static GUIContent modifyContent;
    public static GUIContent ModifyContent
    {
        get
        {
            if (modifyContent == null) modifyContent = EditorGUIUtility.IconContent("d_winbtn_mac_max");
            CleanGUIContent(modifyContent);

            return modifyContent;
        }
    }

    private static GUIContent revertContent;
    public static GUIContent RevertContent
    {
        get
        {
            if (revertContent == null) revertContent = EditorGUIUtility.IconContent("d_Refresh");
            CleanGUIContent(revertContent);

            return revertContent;
        }
    }

    private static GUIContent cameraContent;
    public static GUIContent CameraContent
    {
        get
        {
            if (cameraContent == null) cameraContent = EditorGUIUtility.IconContent("Camera Icon");
            CleanGUIContent(cameraContent);

            return cameraContent;
        }
    }

    private static void CleanGUIContent(GUIContent content)
    {
        content.text = string.Empty;
        content.tooltip = string.Empty;
    }

    #endregion    
}
