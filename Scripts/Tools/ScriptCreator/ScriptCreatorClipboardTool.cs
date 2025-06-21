#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using VT.IO;
using VT.Logger;
using VT.Editor.Utils;

namespace VT.Tools.ScriptCreator
{
    public static class ScriptCreatorClipboardTool
    {
        private const string menuItemPath = "Assets/Clipboard to Script";

        [MenuItem(itemName: menuItemPath, isValidateFunction: false, priority: 1000)]
        private static void SaveClipboardScript()
        {
            var selectedPath = GetSelectedFolderPath();

            if (string.IsNullOrEmpty(selectedPath))
            {
                Debug.LogError("No folder selected.");
                return;
            }

            string clipboard = EditorGUIUtility.systemCopyBuffer;

            if (string.IsNullOrWhiteSpace(clipboard))
            {
                Debug.LogError("Clipboard is empty.");
                return;
            }

            InternalLogger.Instance.LogDebug($"[ScriptCreator] Clipboard content: {clipboard}");

            var scriptData = ScriptDataFactory.FromContent(clipboard);

            if (string.IsNullOrEmpty(scriptData.ClassName) || string.IsNullOrEmpty(scriptData.Content))
            {
                InternalLogger.Instance.LogError("[ScriptCreator] Invalid script data from clipboard.");
                return;
            }

            var filePath = ScriptCreatorIOService.GetPath(scriptData, selectedPath);

            try
            {
                ScriptCreatorIOService.Save(filePath, scriptData);
                InternalLogger.Instance.LogDebug($"[ScriptCreator] Script {scriptData.ClassName} saved at: {selectedPath}");
            }
            catch (System.Exception ex)
            {
                InternalLogger.Instance.LogError($"[ScriptCreator] Error saving script: {ex.Message}");
            }
        }

        [MenuItem(itemName: menuItemPath, isValidateFunction: true)]
        private static bool ValidateSelectedPath()
        {
            InternalLogger.Instance.LogDebug($"[ScriptCreator] Validate...");
            string path = GetSelectedFolderPath();
            return !string.IsNullOrEmpty(path);
        }

        private static string GetSelectedFolderPath()
        {
            var guids = Selection.assetGUIDs;

            if (guids.Length == 0 || guids.Length > 1)
            {
                return string.Empty;
            }

            var path = IOManager.NormalizePath(AssetDatabase.GUIDToAssetPath(guids[0]));

            if (!PathUtils.IsValidPath(path, PathUtils.PathType.Folder))
            {
                return string.Empty;
            }

            return path;
        }
    }
}

#endif
