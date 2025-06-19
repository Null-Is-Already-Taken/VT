using UnityEditor;
using UnityEngine;

namespace VT.Editor.GUI
{
    public static class TextArea
    {
        public static string Draw(string label, string value, float height = 100f, GUIStyle style = null)
        {
            if (!string.IsNullOrEmpty(label))
                EditorGUILayout.LabelField(label);

            var scroll = Vector2.zero;
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(height));

            string result;
            if (style != null)
                result = EditorGUILayout.TextArea(value, style, GUILayout.ExpandHeight(true));
            else
                result = EditorGUILayout.TextArea(value, GUILayout.ExpandHeight(true));

            EditorGUILayout.EndScrollView();
            return result;
        }
    }
}
