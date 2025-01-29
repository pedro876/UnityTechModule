////https://vintay.medium.com/creating-custom-unity-attributes-readonly-d279e1e545c9
//#if UNITY_EDITOR
//using UnityEditor;
//#endif
//using System;
//using UnityEngine;
//using System.Linq;


///// <summary>
///// Will force a serialized array to match an enum.
///// </summary>
//[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
//public class EnumArrayAttribute : PropertyAttribute
//{
//    public Type enumType;
//    public string nameProperty;

//    public EnumArrayAttribute(Type enumType, string nameProperty = "name")
//    {
//        this.enumType = enumType;
//        this.nameProperty = nameProperty;
//    }
//}

//#if UNITY_EDITOR
//[CustomPropertyDrawer(typeof(EnumArrayAttribute))]
//public class EnumArrayPropertyDrawer : PropertyDrawer
//{
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        if (!property.isArray) return;

//        EnumArrayAttribute attr = (EnumArrayAttribute)attribute;

//        string[] names = Enum.GetNames(attr.enumType);

//        //Match names length
//        //while(property.arraySize < names.Length)
//        //{
//        //    property.InsertArrayElementAtIndex(property.arraySize);
//        //    property.GetArrayElementAtIndex(property.arraySize - 1).FindPropertyRelative(attr.nameProperty).stringValue = "";
//        //}

//        for (int i = 0; i < property.arraySize; i++)
//        {
//            SerializedProperty elem = property.GetArrayElementAtIndex(i);
//            string name = elem.FindPropertyRelative(attr.nameProperty).stringValue;
//            if (!names.Contains(name))
//            {
//                property.DeleteArrayElementAtIndex(i);
//                i--;
//            }
//        }

//        for (int i = 0; i < names.Length; i++)
//        {
//            int index = GetIndexOf(names[i]);
//            if(index < 0)
//            {
//                property.InsertArrayElementAtIndex(i);
//                property.GetArrayElementAtIndex(i).FindPropertyRelative(attr.nameProperty).stringValue = names[i];
//            }
//            else
//            {
//                property.MoveArrayElement(index, i);
//            }
//            //SerializedProperty elem = property.GetArrayElementAtIndex(i);

//        }

//        //int arraySize = property.arraySize;
//        //for(int i = 0; i < arraySize; i++)
//        //{
//        //    SerializedProperty elem = property.GetArrayElementAtIndex(i);
//        //    SerializedProperty name = elem.FindPropertyRelative(attr.nameProperty);
//        //    if (name == null)
//        //    {
//        //        Debug.LogError($"Array elements for enum {attr.enumType.Name} do not have a field named {attr.nameProperty}");
//        //        break;
//        //    }
//        //    else
//        //    {
//        //        name.stringValue = names[i];
//        //    }
//        //}

//        int GetIndexOf(string enumName)
//        {
//            for (int i = 0; i < property.arraySize; i++)
//            {
//                if(property.FindPropertyRelative(attr.nameProperty).stringValue == enumName)
//                {
//                    return i;
//                }
//            }
//            return -1;
//        }

//        EditorGUI.PropertyField(position, property, label, true);
//    }
//}
//#endif
