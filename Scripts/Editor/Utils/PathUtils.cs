#if UNITY_EDITOR
using System;
using UnityEngine;
using VT.IO;

namespace VT.Editor.Utils
{
    /// <summary>
    /// Asset store path resolution utilities.
    /// </summary>
    public static class PathUtils
    {
        public static string GetUserProfilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        public static string GetProjectPath()
        {
            return Application.dataPath.Replace("/Assets", "");
        }

        public static string GetAssetStorePath()
        {
            return Application.platform switch
            {
                RuntimePlatform.WindowsEditor => IOManager.CombinePaths(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Unity", "Asset Store-5.x"),

                RuntimePlatform.OSXEditor => IOManager.CombinePaths(
                    Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    "Library", "Unity", "Asset Store-5.x"),

                _ => throw new System.NotSupportedException("Unsupported platform.")
            };
        }

        public static string FromAlias(string path)
        {
            return path
                .Replace("$USER$", GetUserProfilePath())
                .Replace("$PROJECT$", GetProjectPath())
                .Replace("$TEMP$", IOManager.GetTempPath())
                .Replace("$ASSETS_STORE$", GetAssetStorePath());
        }

        public static string ToAlias(string path)
        {
            return path
                .Replace(GetUserProfilePath(), "$USER$")
                .Replace(GetProjectPath(), "$PROJECT$")
                .Replace(IOManager.GetTempPath(), "$TEMP$")
                .Replace(GetAssetStorePath(), "$ASSETS_STORE$");
        }
    }
}
#endif
