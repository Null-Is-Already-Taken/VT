using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace VT.IO
{
    /// <summary>
    /// A utility class for handling file and directory operations in Unity.
    /// Supports relative paths, absolute paths, and path macros like #persistent, #data, #streaming, etc.
    /// </summary>
    public static class IOManager
    {
        /// <summary>
        /// Default root path used when relative paths are provided without macros.
        /// </summary>
        public static string RootPath => Application.dataPath;

        /// <summary>
        /// Macros that map to commonly used Unity directories.
        /// </summary>
        //private static readonly Dictionary<string, Func<string>> MacroPaths = new()
        //{
        //    {"#persistent", () => Application.persistentDataPath},          // Persistent storage path
        //    {"#data",       () => Application.dataPath},                    // Assets folder
        //    {"#streaming",  () => Application.streamingAssetsPath},         // StreamingAssets
        //    {"#temp",       () => Application.temporaryCachePath},          // Temporary cache
        //    {"#project",    () => Path.GetFullPath(Path.Combine(Application.dataPath, ".."))} // Root of project
        //};

        /// <summary>
        /// Resolves a path by expanding macros and converting relative paths to absolute ones.
        /// </summary>
        /// <param name="path">Relative path, absolute path, or macro-based path.</param>
        /// <returns>Resolved absolute path.</returns>
        //public static string ResolvePath(string path)
        //{
        //    if (string.IsNullOrEmpty(path)) return RootPath;

        //    foreach (var (macro, resolver) in MacroPaths)
        //    {
        //        if (path.StartsWith(macro, StringComparison.OrdinalIgnoreCase))
        //        {
        //            string suffix = path[macro.Length..].TrimStart('/', '\\');
        //            string combined = Path.Combine(resolver.Invoke(), suffix);
        //            return NormalizePathSeparators(combined);
        //        }
        //    }

        //    string fullPath = Path.IsPathRooted(path) ? path : Path.Combine(RootPath, path);
        //    return NormalizePathSeparators(fullPath);
        //}

        /// <summary>
        /// Normalizes the directory separators in a file path to match the current platform's separator.
        /// Replaces all forward and backward slashes with the platform-specific separator,
        /// and removes any duplicate separators.
        /// </summary>
        /// <param name="path">The file path to normalize.</param>
        /// <returns>
        /// A normalized file path string with consistent directory separators,
        /// or the original string if it is null or empty.
        /// </returns>
        public static string NormalizePathSeparators(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            char sep = Path.DirectorySeparatorChar;

            // Replace all slashes with the current platform's separator
            string unified = path.Replace('\\', sep).Replace('/', sep);

            // Remove duplicate slashes (e.g., "//" or "\\")
            return Regex.Replace(unified, $"{Regex.Escape(sep.ToString())}+", sep.ToString());
        }

        public static string GetFileNameWithoutExtension(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            // Normalize the path and get the file name without extension
            string fileName = Path.GetFileNameWithoutExtension(path);
            return NormalizePathSeparators(fileName);
        }

        public static string GetDirectoryName(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            return NormalizePathSeparators(Path.GetDirectoryName(path) ?? string.Empty);
        }

        public static string CombinePaths(params string[] paths)
        {
            if (paths == null || paths.Length == 0) return string.Empty;

            // Normalize each path and combine them
            return NormalizePathSeparators(Path.Combine(paths));
        }

        /// <summary>
        /// Checks if a file exists at the specified path.
        /// </summary>
        public static bool FileExists(string path) => File.Exists(NormalizePathSeparators(path));

        /// <summary>
        /// Checks if a directory exists at the specified path.
        /// </summary>
        public static bool DirectoryExists(string path) => Directory.Exists(NormalizePathSeparators(path));

        /// <summary>
        /// Creates a directory at the specified path if it does not exist.
        /// </summary>
        public static string CreateDirectory(string path)
        {
            var normalizedPath = NormalizePathSeparators(path);
            if (DirectoryExists(normalizedPath))
            {
                Directory.CreateDirectory(normalizedPath);
            }
            return normalizedPath;
        }

        /// <summary>
        /// Creates a directory at the specified path if it does not exist.
        /// </summary>
        public static void CreateAssetDirectory(string parentDir, string newFolderName)
        {
            var newFolderPath = Path.Combine(parentDir, newFolderName);
            newFolderPath = NormalizePathSeparators(newFolderPath);
            if (!AssetDatabase.IsValidFolder(newFolderPath))
                AssetDatabase.CreateFolder(parentDir, newFolderName);
        }

        /// <summary>
        /// Converts a full path into a relative path based on the given root.
        /// Normalizes both paths before comparison.
        /// </summary>
        public static string GetRelativePath(string rootPath, string fullPath)
        {
            if (string.IsNullOrEmpty(rootPath) || string.IsNullOrEmpty(fullPath))
                return null;

            rootPath = NormalizePathSeparators(Path.GetFullPath(rootPath));
            fullPath = NormalizePathSeparators(Path.GetFullPath(fullPath));

            if (!fullPath.StartsWith(rootPath))
            {
                Debug.LogWarning($"GetRelativePath: '{fullPath}' is not under root '{rootPath}'");
                return fullPath; // fallback to full path
            }

            string relative = fullPath.Substring(rootPath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return relative;
        }

        /// <summary>
        /// Combines a root path with a relative subpath to form an absolute path.
        /// </summary>
        public static string GetAbsolutePath(string rootPath, string relativePath)
        {
            if (string.IsNullOrEmpty(rootPath)) return null;
            if (string.IsNullOrEmpty(relativePath)) return NormalizePathSeparators(rootPath);

            string combined = Path.Combine(rootPath, relativePath);
            return NormalizePathSeparators(Path.GetFullPath(combined));
        }

        /// <summary>
        /// Deletes a file at the specified path if it exists.
        /// </summary>
        public static void DeleteFile(string path)
        {
            var normalizedPath = NormalizePathSeparators(path);
            if (FileExists(normalizedPath))
            {
                try
                {
                    File.Delete(normalizedPath);
                }
                catch (IOException)
                {
                }
            }
        }

        /// <summary>
        /// Deletes a directory and all its contents at the specified path.
        /// </summary>
        public static void DeleteDirectory(string path)
        {
            var normalizedPath = NormalizePathSeparators(path);
            if (DirectoryExists(normalizedPath))
            {
                Directory.Delete(normalizedPath, true);
            }
        }

        /// <summary>
        /// Saves string content to a file at the given path.
        /// </summary>
        public static void SaveText(string path, string content)
        {
            var normalizedPath = NormalizePathSeparators(path);
            File.WriteAllText(normalizedPath, content);
        }

        /// <summary>
        /// Loads string content from a file at the given path.
        /// </summary>
        public static string LoadText(string path)
        {
            var normalizedPath = NormalizePathSeparators(path);
            return File.Exists(normalizedPath) ? File.ReadAllText(normalizedPath) : null;
        }

        /// <summary>
        /// Saves binary data to a file at the given path.
        /// </summary>
        public static void SaveBinary(string path, byte[] data)
        {
            var normalizedPath = NormalizePathSeparators(path);
            File.WriteAllBytes(normalizedPath, data);
        }

        /// <summary>
        /// Loads binary data from a file at the given path.
        /// </summary>
        public static byte[] LoadBinary(string path)
        {
            var normalizedPath = NormalizePathSeparators(path);
            return File.Exists(normalizedPath) ? File.ReadAllBytes(normalizedPath) : null;
        }

        /// <summary>
        /// Serializes an object to JSON and saves it to the given path.
        /// </summary>
        public static void SaveJson<T>(string path, T data)
        {
            var normalizedPath = NormalizePathSeparators(path);
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            SaveText(normalizedPath, json);
        }

        /// <summary>
        /// Loads JSON from a file and deserializes it into an object of type T.
        /// </summary>
        public static T LoadJson<T>(string path)
        {
            var normalizedPath = NormalizePathSeparators(path);
            string json = LoadText(normalizedPath);
            return !string.IsNullOrEmpty(json) ? JsonUtility.FromJson<T>(json) : default;
        }
    }
}
#endif
