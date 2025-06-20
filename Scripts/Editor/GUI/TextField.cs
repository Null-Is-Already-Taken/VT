using UnityEditor;
using UnityEngine;

namespace VT.Editor.GUI
{
    public static class TextField
    {
        public static string Draw(GUIContent label, string value, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (label != null)
            {
                Label.Draw(label);
            }

            style ??= UnityEngine.GUI.skin.textArea;

            return EditorGUILayout.TextField(value, style, options);
        }
    }
}
