#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VT.IO;
using VT.Logger;

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

        public enum PathAlias
        {
            USER,
            PROJECT,
            TEMP,
            ASSETS_STORE
        }

        private static readonly Dictionary<PathAlias, Func<string>> AliasToPath = new()
        {
            { PathAlias.USER, () => GetUserProfilePath() },
            { PathAlias.PROJECT, () => GetProjectPath() },
            { PathAlias.TEMP, () => IOManager.GetTempPath() },
            { PathAlias.ASSETS_STORE, () => GetAssetStorePath() }
        };

        private static string GetAliasString(PathAlias alias) => $"${alias}$";

        public static string FromAlias(string path)
        {
            string normalizedPath = IOManager.NormalizePath(path);

            foreach (var kvp in AliasToPath)
            {
                string alias = GetAliasString(kvp.Key);
                string realPath = kvp.Value();
                path = path.Replace(alias, realPath);
            }

            return path;
        }

        public static string ToAlias(string path)
        {
            string normalizedPath = IOManager.NormalizePath(path);

            foreach (var kvp in AliasToPath)
            {
                string alias = GetAliasString(kvp.Key);
                string realPath = kvp.Value();
                path = normalizedPath.Replace(realPath, alias);
            }

            return path;
        }

        public static bool IsAlias(string path)
        {
            foreach (PathAlias alias in Enum.GetValues(typeof(PathAlias)))
            {
                if (path.Contains(GetAliasString(alias)))
                    return true;
            }
            return false;
        }
    }
}
#endif
