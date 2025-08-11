#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace VT.Editor.GUI
{
    public static class GenericFields
    {
        public static object Draw(Type fieldType, string fieldName, object value)
        {
            if (fieldType == typeof(int))
                return EditorGUILayout.IntField(fieldName, value is int i ? i : 0);
            if (fieldType == typeof(float))
                return EditorGUILayout.FloatField(fieldName, value is float f ? f : 0f);
            if (fieldType == typeof(bool))
                return EditorGUILayout.Toggle(fieldName, value is bool b && b);
            if (fieldType == typeof(string))
                return EditorGUILayout.TextField(fieldName, value as string ?? "");
            if (fieldType == typeof(Vector2))
                return EditorGUILayout.Vector2Field(fieldName, value is Vector2 v2 ? v2 : Vector2.zero);
            if (fieldType == typeof(Vector3))
                return EditorGUILayout.Vector3Field(fieldName, value is Vector3 v3 ? v3 : Vector3.zero);
            if (fieldType == typeof(Color))
                return EditorGUILayout.ColorField(fieldName, value is Color c ? c : Color.white);
            if (fieldType.IsEnum)
                return EditorGUILayout.EnumPopup(fieldName, (Enum)value);
            if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
                return EditorGUILayout.ObjectField(fieldName, value as UnityEngine.Object, fieldType, true);

            return DrawUnsupportedField(fieldType, fieldName, value);
        }

        private static object DrawUnsupportedField(Type fieldType, string fieldName, object value)
        {
            Label.Draw(fieldName, $"Unsupported type: {fieldType.Name}");
            return value;
        }
    }
}
#endif