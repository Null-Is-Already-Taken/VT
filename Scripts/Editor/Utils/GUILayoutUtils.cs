//#if UNITY_EDITOR
//using System;
//using UnityEditor;
//using UnityEngine;

//namespace VT.Editor.Utils
//{
//    /// <summary>
//    /// Extension methods and wrappers around GUILayout.
//    /// </summary>
//    public static class GUILayoutUtils
//    {
//        public static GUIStyle InlineButtonStyle
//        {
//            get
//            {
//                return new GUIStyle(GUI.skin.button)
//                {
//                    fontSize = 12,
//                    fixedWidth = 24,
//                    fixedHeight = 24,
//                    alignment = TextAnchor.MiddleCenter,
//                    padding = new RectOffset(0, 0, 0, 0),
//                    margin = new RectOffset(0, 0, 0, 0),
//                    normal = { textColor = Color.white }
//                };
//            }
//        }

//        public static GUIStyle FlexibleCenteredLabelStyle
//        {
//            get
//            {
//                return new GUIStyle(EditorStyles.label)
//                {
//                    alignment = TextAnchor.MiddleCenter
//                };
//            }
//        }

//        public static void StyledButton(string label, Color bg, Action onClick, GUIStyle style = null)
//        {
//            var prev = GUI.backgroundColor;
//            GUI.backgroundColor = bg;
//            if (style != null)
//            {
//                if (GUILayout.Button(label, style)) onClick?.Invoke();
//            }
//            else
//            {
//                if (GUILayout.Button(label)) onClick?.Invoke();
//            }
//            GUI.backgroundColor = prev;
//        }

//        public static void StyledButton(GUIContent content, Color bg, Action onClick, GUIStyle style = null)
//        {
//            var prev = GUI.backgroundColor;
//            GUI.backgroundColor = bg;
//            if (style != null)
//            {
//                if (GUILayout.Button(content, style)) onClick?.Invoke();
//            }
//            else
//            {
//                if (GUILayout.Button(content)) onClick?.Invoke();
//            }
//            GUI.backgroundColor = prev;
//        }

//        public static void StyledLabel(string text, GUIStyle style = null)
//        {
//            if (style != null)
//                EditorGUILayout.LabelField(text, style);
//            else
//                EditorGUILayout.LabelField(text);
//        }

//        public static void StyledLabel(GUIContent content, GUIStyle style = null)
//        {
//            if (style != null)
//                EditorGUILayout.LabelField(content, style);
//            else
//                EditorGUILayout.LabelField(content);
//        }

//        public static void StyledCenterLabelContent(string text)
//        {
//            var content = new GUIContent(text);
//            var style = FlexibleCenteredLabelStyle;
//            var width = style.CalcSize(content).x + 8f; // add padding on x
//            EditorGUILayout.LabelField(text, FlexibleCenteredLabelStyle, GUILayout.Width(width));
//        }

//        public static void PageNavigator(int currentIndex, int total, Action<int> onPageChanged, string label)
//        {
//            using (new EditorGUILayout.HorizontalScope())
//            {
//                GUI.enabled = currentIndex > 0;
//                if (GUILayout.Button("←", GUILayout.Width(24)))
//                    onPageChanged?.Invoke(currentIndex - 1);

//                // Build our text and style
//                string text = $"{label} {currentIndex + 1} of {total}";
//                StyledCenterLabelContent(text);

//                GUI.enabled = currentIndex < total - 1;
//                if (GUILayout.Button("→", GUILayout.Width(24)))
//                    onPageChanged?.Invoke(currentIndex + 1);

//                GUI.enabled = true;
//            }
//        }
//    }
//}
//#endif
