using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VT.EditorUtils
{
    public static class PathListDrawer<T>
    {
        public static void Draw(
            IList<T> items,
            Func<T, string> getPath,
            Func<T, string> getDisplayText,
            Func<string, bool> fileExists,
            Action onAdd,
            Action<int> onRemove,
            ref int currentPage,
            ref Dictionary<string, bool> existenceCache,
            ref double lastCacheRefreshTime,
            string header = "Path List",
            int itemsPerPage = 10,
            float buttonSize = 18f,
            float spacing = 5f,
            double currentTime = 0,
            double refreshInterval = 2.0)
        {
            if (items == null) return;

            GUILayout.Space(spacing);
            EditorGUILayout.BeginHorizontal("helpBox");
            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            if (onAdd != null)
            {
                var addButton = new GUIContent("+", "Add Path");
                var addButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 12,
                    fixedWidth = buttonSize,
                    fixedHeight = buttonSize,
                    alignment = TextAnchor.MiddleCenter,
                    margin = new RectOffset(0, 0, 0, 0)
                };
                if (GUILayout.Button(addButton, addButtonStyle))
                {
                    onAdd.Invoke();
                    return;
                }
            }

            EditorGUILayout.EndHorizontal();

            if (items.Count == 0) return;

            int totalPages = Mathf.CeilToInt(items.Count / (float)itemsPerPage);
            currentPage = Mathf.Clamp(currentPage, 0, Mathf.Max(0, totalPages - 1));
            int startIndex = currentPage * itemsPerPage;
            int endIndex = Mathf.Min(startIndex + itemsPerPage, items.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                T item = items[i];
                string path = getPath(item);
                bool exists = false;

                if (currentTime - lastCacheRefreshTime > refreshInterval || !existenceCache.ContainsKey(path))
                {
                    exists = fileExists(path);
                    existenceCache[path] = exists;
                    lastCacheRefreshTime = currentTime;
                }
                else
                {
                    exists = existenceCache[path];
                }

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();

                float labelWidth = EditorGUIUtility.currentViewWidth - buttonSize - spacing * 4;

                string display = getDisplayText(item);
                string tooltip = exists ? path : $"{path} (Missing)";
                var labelContent = new GUIContent(display, tooltip);
                var labelStyle = new GUIStyle(EditorStyles.label)
                {
                    normal = { textColor = exists ? Color.white : Color.red },
                    fixedWidth = labelWidth
                };
                EditorGUILayout.LabelField(labelContent, labelStyle);

                GUILayout.FlexibleSpace();

                var removeContent = new GUIContent("-", "Remove Path");
                var removeStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 12,
                    fixedWidth = buttonSize,
                    fixedHeight = buttonSize,
                    alignment = TextAnchor.MiddleCenter
                };
                if (GUILayout.Button(removeContent, removeStyle))
                {
                    onRemove?.Invoke(i);
                    return;
                }

                GUILayout.Space(spacing);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            if (totalPages > 1)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("<", GUILayout.Width(25)))
                    currentPage = Mathf.Max(currentPage - 1, 0);
                EditorGUILayout.LabelField($"Page {currentPage + 1} / {totalPages}", GUILayout.Width(100));
                if (GUILayout.Button(">", GUILayout.Width(25)))
                    currentPage = Mathf.Min(currentPage + 1, totalPages - 1);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
