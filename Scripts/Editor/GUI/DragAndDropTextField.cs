using UnityEditor;
using UnityEngine;
using VT.Editor.Utils;

namespace VT.Editor.GUI
{
    public static class DragAndDropTextField
    {
        public static string Draw(GUIContent content, string value, PathUtils.PathType filter, GUIStyle labelStyle = null, params GUILayoutOption[] options)
        {
            Event evt = Event.current;

            using (new EditorGUILayout.HorizontalScope())
            {
                if (content != null)
                {
                    labelStyle ??= LabelStyles.Label;
                    Label.Draw(
                        content: content,
                        style: labelStyle,
                        options: GUILayout.Width(80)
                    );
                }

                float desiredWidth = EditorGUIUtility.currentViewWidth - 140f; // subtract for padding/margin accounting for vertical scrollbars width
                Rect dropRect = GUILayoutUtility.GetRect(new GUIContent(value), EditorStyles.textField, GUILayout.Width(desiredWidth));
                value = EditorGUI.TextField(dropRect, value);

                if (!dropRect.Contains(evt.mousePosition))
                    return value;

                // Handle drag and drop
                if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
                {
                    if (DragAndDrop.paths != null && DragAndDrop.objectReferences.Length > 0)
                    {
                        string draggedPath = DragAndDrop.paths[0];

                        if (PathUtils.IsValidPath(draggedPath, filter))
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            if (evt.type == EventType.DragPerform)
                            {
                                DragAndDrop.AcceptDrag();
                                value = draggedPath;
                                UnityEngine.GUI.changed = true;
                            }
                        }
                    }
                }
            }

            return value;
        }
    }
}
