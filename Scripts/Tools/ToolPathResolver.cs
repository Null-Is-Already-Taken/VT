using UnityEditor;
using UnityEngine;
using VT.IO;

namespace VT.Tools
{
    public static class ToolPathResolver
    {
        /// <summary>
        /// Finds the root folder containing the specified marker file.
        /// </summary>
        public static string GetToolRootByMarker(string markerFile)
        {
            if (string.IsNullOrEmpty(markerFile))
            {
                Debug.LogError("ToolPathResolver: Marker file name cannot be null or empty.");
                return null;
            }

            string[] guids = AssetDatabase.FindAssets(IOManager.GetFileNameWithoutExtension(markerFile));

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(markerFile))
                {
                    return IOManager.GetDirectoryName(path);
                }
            }

            Debug.LogError($"ToolPathResolver: Marker file '{markerFile}' not found in project.");
            return null;
        }

        public static string GetToolRelativePath(string relativeToToolRoot, string markerFile)
        {
            string root = GetToolRootByMarker(markerFile);
            return root != null ? IOManager.CombinePaths(root, relativeToToolRoot) : null;
        }
    }
}
