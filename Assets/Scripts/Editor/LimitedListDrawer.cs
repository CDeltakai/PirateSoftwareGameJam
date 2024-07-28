using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LimitedListAttribute))]
public class LimitedListDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Debug.Log($"Property name: {property.name}, Property type: {property.propertyType}, Is array: {property.isArray}");

        if (property.isArray && property.propertyType == SerializedPropertyType.Generic)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw the label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Calculate rects
            var elementCountRect = new Rect(position.x, position.y, 30, position.height);
            var listRect = new Rect(position.x + 35, position.y, position.width - 35, position.height);

            // Display the element count
            EditorGUI.LabelField(elementCountRect, property.arraySize.ToString());

            // Display the list
            EditorGUI.PropertyField(listRect, property, GUIContent.none, true);

            // Limit the number of elements
            if (property.arraySize > ((LimitedListAttribute)attribute).MaxElements)
            {
                property.arraySize = ((LimitedListAttribute)attribute).MaxElements;
            }

            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use LimitedList with arrays or lists.");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isArray && property.propertyType == SerializedPropertyType.Generic)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        else
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
