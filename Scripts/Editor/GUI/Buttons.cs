using System;
using UnityEditor;
using UnityEngine;

namespace VT.Editor.GUI
{
    public static class ButtonStyles
    {
        public static GUIStyle RadioButton => new (EditorStyles.radioButton);
        public static GUIStyle MiniButton => new (EditorStyles.miniButton);
        public static GUIStyle MiniButtonLeft => new (EditorStyles.miniButtonLeft);
        public static GUIStyle MiniButtonMid => new (EditorStyles.miniButtonMid);
        public static GUIStyle MiniButtonRight => new (EditorStyles.miniButtonRight);
        public static GUIStyle MiniPullDown => new (EditorStyles.miniPullDown);

        public static GUIStyle Inline => new (UnityEngine.GUI.skin.button)
        {
            fontSize = 12,
            fixedWidth = 24,
            fixedHeight = 24,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(0, 0, 0, 0),
            normal = { textColor = Color.white }
        };

        public static GUIStyle Compact => new(UnityEngine.GUI.skin.button)
        {
            fontSize = 12,
            fixedWidth = 60,
            fixedHeight = 24,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(0, 0, 0, 0),
            normal = { textColor = Color.white }
        };
    }

    public static class Button
    {
        public static void Draw(string label, Color backgroundColor, Action onClick, GUIStyle style = null, params GUILayoutOption[] options)
        {
            Draw(new GUIContent(label), backgroundColor, onClick, style, options);
        }

        public static void Draw(GUIContent content, Color backgroundColor, Action onClick, GUIStyle style = null, params GUILayoutOption[] options)
        {
            var prevColor = UnityEngine.GUI.backgroundColor;
            UnityEngine.GUI.backgroundColor = backgroundColor;

            bool clicked = false;
            if (style != null)
                clicked = GUILayout.Button(content, style, options);
            else
                clicked = GUILayout.Button(content, options);

            if (clicked)
                onClick?.Invoke();

            UnityEngine.GUI.backgroundColor = prevColor;
        }
    }
}
