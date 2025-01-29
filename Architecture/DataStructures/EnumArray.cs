#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

namespace Architecture
{
    /// <summary>
    /// Array wrapper that automatically creates one entry for each enum value of the
    /// specified TEnum. This class supports indexing using integers and enums. For loops,
    /// foreach is implemented and traditional for loops can also be used.
    /// </summary>
    [System.Serializable]
    public abstract class EnumArray
    {
        public abstract Type GetEnumType();

        [System.Serializable]
        public class EnumEntry
        {
            [SerializeField, HideInInspector] public string name;
        }
        
        [System.Serializable]
        public class EnumEntry<TClass> : EnumEntry
        {
            public TClass value;
        }
    }

    /// <summary>
    /// Array wrapper that automatically creates one entry for each enum value of the
    /// specified TEnum. This class supports indexing using integers and enums. For loops,
    /// foreach is implemented and traditional for loops can also be used.
    /// </summary>
    [System.Serializable]
    public class EnumArray<TEnum, TClass> : EnumArray, IEnumerable<TClass>
        where TEnum : Enum 
    {
        [SerializeField] EnumEntry<TClass>[] array;

        public TClass this[int index]
        {
            get => array[index].value;
            set => array[index].value = value;
        }

        public TClass this[TEnum index]
        {
            get => array[(int)(object)index].value;
            set => array[(int)(object)index].value = value;
        }

        public int Length => array.Length;

        public EnumArray()
        {
            string[] names = Enum.GetNames(typeof(TEnum));
            var values = Enum.GetValues(typeof(TEnum));
            array = new EnumEntry<TClass>[names.Length];
            for(int i = 0; i <  names.Length; i++)
            {
                array[i] = new EnumEntry<TClass>();
                array[i].name = names[i];
                array[i].value = default;
            }
        }

        public override Type GetEnumType() => typeof(TEnum);

        public IEnumerator GetEnumerator()
        {
            for(int i =0; i < array.Length; i++)
            {
                yield return array[i].value;
            }
        }

        IEnumerator<TClass> IEnumerable<TClass>.GetEnumerator()
        {
            for (int i = 0; i < array.Length; i++)
            {
                yield return array[i].value;
            }
        }

        public IEnumerable<(int, TEnum, TClass)> EnumerateIndicesEnumsAndValues()
        {
            for (int i = 0; i < array.Length; i++)
            {
                yield return (i, (TEnum)Enum.ToObject(typeof(TEnum), i), array[i].value);
            }
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(EnumArray), true)]
    public class EnumArrayPropertyDrawer : PropertyDrawer
    {
        private const string nameProperty = nameof(EnumArray.EnumEntry.name);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumArray enumArray = (EnumArray)property.boxedValue;
            string[] enumNames = Enum.GetNames(enumArray.GetEnumType()); //HERE: HOW TO GET THE TEnum TYPE?

            SerializedProperty array = property.FindPropertyRelative("array");

            CheckCorrectArrayState(property, array, enumNames);

            GUILayout.Label($"{property.displayName}:", EditorStyles.boldLabel);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Space(4);
                for (int i = 0; i < array.arraySize; i++)
                {
                    SerializedProperty elem = array.GetArrayElementAtIndex(i);
                    SerializedProperty name = elem.FindPropertyRelative("name");
                    SerializedProperty value = elem.FindPropertyRelative("value");
                    if (i > 0) GUILayout.Space(10);
                    EditorGUILayout.LabelField($" {name.stringValue}:", EditorStyles.boldLabel);

                    EditorGUI.indentLevel++;
                    int depth = value.depth;
                    while (value.NextVisible(true))
                    {
                        if (value.depth <= depth) break;
                        EditorGUILayout.PropertyField(value);
                    }
                    EditorGUI.indentLevel--;

                    //EditorGUILayout.PropertyField(value, true);
                    //EditorGUI.PropertyField(position, value, true);
                }
                GUILayout.Space(4);
            }

            //EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0f;
            //return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private void CheckCorrectArrayState(SerializedProperty property, SerializedProperty array, string[] enumNames)
        {
            //Prune obsolete serialized data
            for (int i = 0; i < array.arraySize; i++)
            {
                SerializedProperty elem = array.GetArrayElementAtIndex(i);
                string name = elem.FindPropertyRelative(nameProperty).stringValue;
                if (!enumNames.Contains(name))
                {
                    array.DeleteArrayElementAtIndex(i);
                    i--;
                }
            }

            //Sort array based on enum order and add missing enum values
            for (int i = 0; i < enumNames.Length; i++)
            {
                int index = GetIndexOf(enumNames[i]);
                if (index < 0)
                {
                    array.InsertArrayElementAtIndex(i);
                    array.GetArrayElementAtIndex(i).FindPropertyRelative(nameProperty).stringValue = enumNames[i];
                }
                else
                {
                    array.MoveArrayElement(index, i);
                }
            }

            int GetIndexOf(string enumName)
            {
                for (int i = 0; i < array.arraySize; i++)
                {
                    SerializedProperty elem = array.GetArrayElementAtIndex(i);
                    if (elem.FindPropertyRelative(nameProperty).stringValue == enumName)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }
    }
#endif
}
