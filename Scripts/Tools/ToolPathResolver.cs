using UnityEditor;
using UnityEngine;
using VT.IO;

namespace VT.Tools
{
    public static class ToolPathResolver
    {
        /// <summary>
        /// Finds the folder containing the given marker file.
        /// </summary>
        public static string GetToolRootByMarker(string markerFile)
        {
            if (string.IsNullOrEmpty(markerFile))
            {
                Debug.LogError("ToolPathResolver: Marker file name cannot be null or empty.");
                return null;
            }

            string[] guids = AssetDatabase.FindAssets(markerFile);
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(markerFile))
                    return IOManager.GetDirectoryName(path);
            }

            Debug.LogError($"ToolPathResolver: Marker file '{markerFile}' not found.");
            return null;
        }

        public static string GetToolRelativePath(string relativePath, string markerFile)
        {
            string root = GetToolRootByMarker(markerFile);

            if (string.IsNullOrEmpty(relativePath) || string.IsNullOrEmpty(root))
            {
                Debug.LogError("ToolPathResolver: Relative path or root path is null or empty.");
                return null;
            }

            return root != null ? IOManager.CombinePaths(root, relativePath) : null;
        }
    }

}
