#if UNITY_EDITOR
using UnityEngine;
using VT.IO;

namespace VT.Editor.Utils
{
    /// <summary>
    /// Asset store path resolution utilities.
    /// </summary>
    public static class PathUtils
    {
        public static string GetAssetStoreBasePath()
        {
            return Application.platform switch
            {
                RuntimePlatform.WindowsEditor => IOManager.CombinePaths(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                    "Unity", "Asset Store-5.x"),

                RuntimePlatform.OSXEditor => IOManager.CombinePaths(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                    "Library", "Unity", "Asset Store-5.x"),

                _ => throw new System.NotSupportedException("Unsupported platform.")
            };
        }
    }
}
#endif
