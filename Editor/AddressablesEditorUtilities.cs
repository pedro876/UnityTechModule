using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressablesEditorUtilities
{
    #region Group Methods

    public static bool ExistGroup(string groupName) => AddressableAssetSettingsDefaultObject.Settings.FindGroup(groupName) != null;

    public static void CreateGroup(string groupName)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        if (ExistGroup(groupName))
        {
            Debug.LogWarning($"The {groupName} addressable group already exists");
            return;
        }

        AddressableAssetGroup group = settings.CreateGroup(groupName, false, false, false, null);

        group.AddSchema<BundledAssetGroupSchema>();
        group.AddSchema<ContentUpdateGroupSchema>();

        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryAdded, group, true);

        Debug.Log($"Create {groupName} addressable group");
    }

    public static void DeleteGroup(string groupName)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        if (ExistGroup(groupName))
        {
            AddressableAssetGroup group = GetGroup(groupName);

            settings.RemoveGroup(group);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupRemoved, group, true);
        }
    }

    public static AddressableAssetGroup GetGroup(string groupName) => AddressableAssetSettingsDefaultObject.Settings.FindGroup(groupName);

    #endregion

    #region Entry Methods

    public static AddressableAssetEntry GetEntry(Object obj)
    {
        if (obj == null)
        {
            Debug.LogError("Please select an asset.");
            return null;
        }

        return GetEntry(AssetDatabase.GetAssetPath(obj));
    }

    public static AddressableAssetEntry GetEntry(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Please select an asset.");
            return null;
        }

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        foreach (var group in settings.groups)
        {
            foreach (var entry in group.entries)
            {
                if (entry.AssetPath == path)
                {
                    return entry;
                }
            }
        }

        return null;
    }

    public static void CreateEntry(string address, string groupName, Object obj)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        if (!ExistGroup(groupName)) CreateGroup(groupName);

        var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));

        new AssetReference(guid);

        var entry = settings.CreateOrMoveEntry(guid, GetGroup(groupName));

        entry.SetAddress(address);

        AssetDatabase.Refresh();
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryAdded, entry, true);
        AssetDatabase.SaveAssets();
    }

    public static bool CheckObjectAddress(string key, string group, Object obj)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
        AddressableAssetEntry entry = settings.FindAssetEntry(guid);

        return entry != null && entry.address == key && entry.parentGroup.name == group;
    }

    #endregion
}
