using UnityEditor;
using UnityEngine;

namespace VT.Editor.GUI
{
    public static class TextField
    {
        public static string Draw(string label, string value, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (!string.IsNullOrEmpty(label))
            {
                EditorGUILayout.LabelField(label);
            }

            if (style != null)
                return EditorGUILayout.TextField(value, style, options);
            else
                return EditorGUILayout.TextField(value, options);
        }
    }
}
