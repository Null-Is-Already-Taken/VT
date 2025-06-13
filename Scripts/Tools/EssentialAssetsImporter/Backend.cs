//using System;
//using UnityEditor;
//using UnityEngine;
//using VT.Editor.Utils;
//using VT.IO;

//namespace VT.Tools.EssentialAssetsImporter
//{
//    public static class Backend
//    {
//        [Obsolete("Use PathUtils.GetAssetStoreBasePath instead.")]
//        public static string GetAssetStoreBasePath()
//        {
//            return Application.platform switch
//            {
//                RuntimePlatform.WindowsEditor => IOManager.CombinePaths(
//                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
//                    "Unity", "Asset Store-5.x"),

//                RuntimePlatform.OSXEditor => IOManager.CombinePaths(
//                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
//                    "Library", "Unity", "Asset Store-5.x"),

//                _ => throw new System.NotSupportedException("Unsupported platform.")
//            };
//        }

//        [Obsolete("Use TextUtils.TruncateWithEllipsis instead.")]
//        public static string TruncateWithEllipsis(string text, int maxLength)
//        {
//            return string.IsNullOrEmpty(text) ? string.Empty : (text.Length <= maxLength ? text : text[..(Mathf.Max(maxLength - 4, 0))] + "...");
//        }

//        [Obsolete("Use TextUtils.EstimateMaxChars instead.")]
//        public static int EstimateMaxChars(float width, float averageCharWidth) => Mathf.FloorToInt(width / averageCharWidth);

//        [Obsolete("Use GUILayoutUtils.StyledButton instead.")]
//        public static void StyledButton(string label, Color backgroundColor, Action onClick, GUIStyle style = null)
//        {
//            Color previousColor = GUI.backgroundColor;
//            GUI.backgroundColor = backgroundColor;

//            if (style != null)
//            {
//                if (GUILayout.Button(label, style))
//                    onClick?.Invoke();
//            }
//            else
//            {
//                if (GUILayout.Button(label))
//                    onClick?.Invoke();
//            }

//            GUI.backgroundColor = previousColor;
//        }

//        [Obsolete("Use GUILayoutUtils.StyledButton instead.")]
//        public static void StyledButton(GUIContent content, Color backgroundColor, Action onClick, GUIStyle style = null)
//        {
//            Color previousColor = GUI.backgroundColor;
//            GUI.backgroundColor = backgroundColor;

//            if (style != null)
//            {
//                if (GUILayout.Button(content, style))
//                    onClick?.Invoke();
//            }
//            else
//            {
//                if (GUILayout.Button(content))
//                    onClick?.Invoke();
//            }

//            GUI.backgroundColor = previousColor;
//        }

//        [Obsolete("Use GUILayoutUtils.StyledLabel instead.")]
//        public static void StyledLabel(string label, GUIStyle style = null)
//        {
//            if (style != null)
//            {
//                EditorGUILayout.LabelField(label, style);
//            }
//            else
//            {
//                EditorGUILayout.LabelField(label);
//            }
//        }

//        [Obsolete("Use GUILayoutUtils.StyledLabel instead.")]
//        public static void StyledLabel(GUIContent content, GUIStyle style = null)
//        {
//            if (style != null)
//            {
//                EditorGUILayout.LabelField(content, style);
//            }
//            else
//            {
//                EditorGUILayout.LabelField(content);
//            }
//        }

//        [Obsolete("Use GUILayoutUtils.InlineButtonStyle instead.")]
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

//        [Obsolete("Use GUILayoutUtils.PageNavigator instead.")]
//        /// <summary>
//        /// Draws a “← Profile X of N →” selector and invokes onPageChanged when Prev/Next is clicked.
//        /// </summary>
//        public static void DrawPageNavigator(string label, int currentIndex, int total, Action<int> onPageChanged)
//        {
//            GUILayoutUtils.PageNavigator(currentIndex, total, onPageChanged, label);
//        }

//    }
//}
