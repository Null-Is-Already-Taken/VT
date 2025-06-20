using UnityEditor;
using UnityEngine;
using VT.Editor.Utils;
using VT.IO;
using VT.Logger;

namespace VT.Editor.GUI
{
    public static class PathPicker
    {
        public static string Draw(GUIContent label, string currentPath, PathType filter, GUIStyle labelStyle = null, params GUILayoutOption[] options)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (label != null)
                {
                    currentPath = DragAndDropTextField.Draw(
                        content: label,
                        value: currentPath,
                        filter: filter,
                        labelStyle: labelStyle,
                        options: options
                    );
                }
                
                GUILayout.FlexibleSpace();

                Button.Draw(
                    content: new GUIContent(EmbeddedIcons.OpenFolder_Unicode),
                    backgroundColor: Color.white,
                    onClick: () =>
                    {
                        string absolutePath = EditorUtility.OpenFolderPanel(
                            title: "Select Save Path",
                            folder: PathUtils.GetProjectPath(),
                            defaultName: ""
                        );

                        if (!string.IsNullOrEmpty(absolutePath))
                        {
                            currentPath = IOManager.CombinePaths("Assets", IOManager.GetRelativePath(PathUtils.GetProjectPath(), absolutePath));
                            InternalLogger.Instance.LogDebug($"Selected save path: {currentPath}");
                            UnityEngine.GUI.changed = true;
                        }
                    },
                    style: ButtonStyles.Inline
                );
            }

            return currentPath;
        }
    }
}
