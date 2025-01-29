using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MinMaxAttribute : PropertyAttribute
{
    public float min, max;

    public MinMaxAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(MinMaxAttribute))]
public class MinMaxDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //if (property.serializedObject.isEditingMultipleObjects) return 0f;
        //return base.GetPropertyHeight(property, label) + 16f;
        return 0f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        MinMaxAttribute minMax = (MinMaxAttribute)attribute;

        // Start drawing the property field
        EditorGUI.BeginProperty(position, label, property);

        // Ensure the property is a Vector2, since we're storing min and max values in one field
        if (property.propertyType == SerializedPropertyType.Vector2)
        {
            Vector2 range = property.vector2Value;

            // Draw the label
            //Rect controlRect = EditorGUI.PrefixLabel(position, label);

            //// Calculate the position of the slider and the fields for min/max
            const float fieldWidth = 40f;
            const float sliderPadding = 5f;
            //float propertyHeight = EditorGUI.GetPropertyHeight(property);
            //Rect minFieldRect = new Rect(controlRect.x, controlRect.y, fieldWidth, propertyHeight);
            //Rect maxFieldRect = new Rect(controlRect.xMax - fieldWidth, controlRect.y, fieldWidth, propertyHeight);
            //Rect sliderRect = new Rect(minFieldRect.xMax + sliderPadding, controlRect.y, controlRect.width - (2 * fieldWidth + 2 * sliderPadding), propertyHeight);

            //// Draw float fields for min and max
            //range.x = EditorGUI.FloatField(minFieldRect, range.x);
            //range.y = EditorGUI.FloatField(maxFieldRect, range.y);

            //// Draw the MinMaxSlider
            //EditorGUI.MinMaxSlider(sliderRect, ref range.x, ref range.y, minMax.min, minMax.max);
            //// Clamp the values to the specified range
            //range.x = Mathf.Clamp(range.x, minMax.min, range.y);
            //range.y = Mathf.Clamp(range.y, range.x, minMax.max);

            //// Assign the modified value back to the property
            //property.vector2Value = range;

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth-1));
                range.x = EditorGUILayout.FloatField(range.x, GUILayout.Width(fieldWidth));
                EditorGUILayout.MinMaxSlider(ref range.x, ref range.y, minMax.min, minMax.max);
                range.y = EditorGUILayout.FloatField(range.y, GUILayout.Width(fieldWidth));
                range.x = Mathf.Clamp(range.x, minMax.min, range.y);
                range.y = Mathf.Clamp(range.y, range.x, minMax.max);
                property.vector2Value = range;
            }
        }
        else
        {
            // Show an error if the property is not a Vector2
            EditorGUI.LabelField(position, label.text, "Use MinMaxSlider with Vector2.");
        }

        EditorGUI.EndProperty();
    }
}
#endif