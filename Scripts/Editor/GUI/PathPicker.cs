using System;
using UnityEditor;
using UnityEngine;
using VT.Editor.Utils;
using VT.IO;
using VT.Logger;

namespace VT.Editor.GUI
{
    public enum PathType
    {
        Any,
        File,
        Folder
    }

    public static class PathPicker
    {
        public static void Draw(string label, ref string currentPath, PathType filter, GUIStyle labelStyle = null, params GUILayoutOption[] options)
        {
            Rect dropRect;

            using (new EditorGUILayout.HorizontalScope())
            {
                if (!string.IsNullOrEmpty(label))
                {
                    labelStyle ??= LabelStyles.Label;
                    Label.Draw(
                        text: label,
                        style: labelStyle,
                        options: GUILayout.Width(80)
                    );
                }

                // Text field with room for inline button
                dropRect = GUILayoutUtility.GetRect(new GUIContent(currentPath), EditorStyles.textField, options);
                Rect textFieldRect = new(dropRect.x, dropRect.y, dropRect.width - EditorGUIUtility.singleLineHeight, dropRect.height);
                Rect buttonRect = new(dropRect.x + dropRect.width - EditorGUIUtility.singleLineHeight, dropRect.y, EditorGUIUtility.singleLineHeight, dropRect.height);

                currentPath = EditorGUI.TextField(textFieldRect, currentPath);

                string newPath = currentPath;

                Button.Draw(
                    content: new GUIContent(EmbeddedIcons.OpenFolder_Unicode),
                    backgroundColor: Color.white,
                    onClick: () =>
                    {
                        string absolutePath = EditorUtility.OpenFolderPanel(
                            title: "Select Path",
                            folder: PathUtils.GetProjectPath(),
                            defaultName: ""
                        );

                        if (!string.IsNullOrEmpty(absolutePath))
                        {
                            newPath = IOManager.GetRelativePath(PathUtils.GetProjectPath(), absolutePath);
                            InternalLogger.Instance.LogDebug($"Selected path: {newPath}");
                            UnityEngine.GUI.changed = true;
                        }
                    },
                    style: ButtonStyles.Inline
                );

                currentPath = newPath;
            }

            HandleDragAndDrop(dropRect, ref currentPath, filter);
        }

        private static void HandleDragAndDrop(Rect dropRect, ref string path, PathType filter)
        {
            Event evt = Event.current;

            if (!dropRect.Contains(evt.mousePosition))
                return;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                    {
                        string draggedPath = DragAndDrop.paths[0];

                        if (IsValidPath(draggedPath, filter))
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                            if (evt.type == EventType.DragPerform)
                            {
                                DragAndDrop.AcceptDrag();
                                path = draggedPath;
                                UnityEngine.GUI.changed = true;
                            }
                        }
                    }
                    break;
            }
        }

        private static bool IsValidPath(string path, PathType filter)
        {
            switch (filter)
            {
                case PathType.Folder: return AssetDatabase.IsValidFolder(path);
                case PathType.File: return !AssetDatabase.IsValidFolder(path);
                case PathType.Any: return true;
                default: return true;
            }
        }
    }
}
