using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DictionaryValues<T> where T : Component
{
    public string Key;
    public T Component;
}

//This script is used for Dictionary serialization on Inspector. By default, the Dictionaries cannot be serialized

public class ComponentDictionary<T> : MonoBehaviour where T : Component
{
    private List<DictionaryValues<T>> _componentsList;
    public Dictionary<string, T> Dictionary;

    public void Initialize(List<DictionaryValues<T>> stringComponent)
    {
        _componentsList = stringComponent;
        Dictionary = new Dictionary<string, T>();

        for (int i = 0; i < _componentsList.Count; i++)
        {
            if (!Dictionary.ContainsKey(_componentsList[i].Key)) Dictionary.Add(_componentsList[i].Key, _componentsList[i].Component);
        }
    }

    public T GetComponentByKey(string key)
    {
        return Dictionary.TryGetValue(key, out var component) ? component : null;
    }
}
