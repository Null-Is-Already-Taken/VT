#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace VT.Editor.GUI
{
    public static class TextArea
    {
        public static string Draw(GUIContent label, string value, ref Vector2 scroll, Action<string> onValueChanged, float height = 100f, GUIStyle style = null)
        {
            if (label != null)
                EditorGUILayout.LabelField(label);

            style ??= UnityEngine.GUI.skin.textArea;

            using var sv = new EditorGUILayout.ScrollViewScope(scroll);
            scroll = sv.scrollPosition;

            string newValue = EditorGUILayout.TextArea(value, style, GUILayout.ExpandHeight(true));

            if (newValue != value)
            {
                onValueChanged?.Invoke(newValue);
            }

            return newValue;
        }
    }
}
#endif
