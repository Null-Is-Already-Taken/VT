using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using VT.Logger;

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
        public static string NormalizePath(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path)) return null;

                char sep = Path.DirectorySeparatorChar;

                // Replace all slashes with the current platform's separator
                string unified = path.Replace('\\', sep).Replace('/', sep);

                // Remove duplicate slashes (e.g., "//" or "\\")
                return Regex.Replace(unified, $"{Regex.Escape(sep.ToString())}+", sep.ToString());
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error normalizing path '{path}': {ex.Message}");
                return path; // Fallback to original path on error
            }
        }

        public static string GetURIFileName(Uri uri)
        {
            try
            {
                return Path.GetFileName(uri.LocalPath);
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error getting file name from URI '{uri}': {ex.Message}");
                return null; // Fallback to null on error
            }
        }

        public static string GetTempPath()
        {
            try
            {
                // Use the system's temporary path and normalize it
                return NormalizePath(Path.GetTempPath());
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error getting temp path: {ex.Message}");
                return null; // Fallback to null on error
            }
        }

        public static string GetURIVersion(Uri uri)
        {
            try
            { 
                var segments = uri.AbsoluteUri.Split('/');
                for (int i = 0; i < segments.Length - 1; i++)
                {
                    if (segments[i].Equals("download", StringComparison.OrdinalIgnoreCase))
                        return segments[i + 1];
                }
                return "unknown";
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error getting version from URI '{uri}': {ex.Message}");
                return null;
            }
        }

        public static string GetURIPackageName(Uri uri)
        {
            try
            {
                string fileName = GetURIFileName(uri);
                string version = GetURIVersion(uri);
                if (fileName.Contains(version))
                    return fileName
                        .Replace(version, "")
                        .Replace(".unitypackage", "")
                        .TrimEnd('.');
                return GetFileNameWithoutExtension(fileName);
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error getting package name from URI '{uri}': {ex.Message}");
                return null; // Fallback to null on error
            }
        }

        public static string GetFileNameWithoutExtension(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path)) return null;

                // Normalize the path and get the file name without extension
                string fileName = Path.GetFileNameWithoutExtension(path);
                return NormalizePath(fileName);
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error getting file name without extension from '{path}': {ex.Message}");
                return null; // Fallback to null on error
            }
        }

        public static string GetDirectoryName(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path)) return null;
                return NormalizePath(Path.GetDirectoryName(path) ?? string.Empty);
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error getting directory name from '{path}': {ex.Message}");
                return null; // Fallback to null on error
            }
        }

        public static string CombinePaths(params string[] paths)
        {
            try
            { 
                if (paths == null || paths.Length == 0) return string.Empty;
                // Normalize each path and combine them
                return NormalizePath(Path.Combine(paths));
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error combining paths '{string.Join(", ", paths)}': {ex.Message}");
                return string.Empty; // Fallback to empty string on error
            }
        }

        /// <summary>
        /// Checks if a file exists at the specified path.
        /// </summary>
        public static bool FileExists(string path)
        {
            try
            { 
                return File.Exists(NormalizePath(path));
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error checking file existence at '{path}': {ex.Message}");
                return false; // Fallback to false on error
            }
        }

        /// <summary>
        /// Creates a directory at the specified path if it does not exist.
        /// </summary>
        public static bool AssetFileExists(string path)
        {
            try
            {
                return !AssetDatabase.IsValidFolder(NormalizePath(path))
                    && AssetDatabase.AssetPathExists(NormalizePath(path));
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error checking asset file existence at '{path}': {ex.Message}");
                return false; // Fallback to false on error
            }
        }

        /// <summary>
        /// Checks if a directory exists at the specified path.
        /// </summary>
        public static bool DirectoryExists(string path)
        {
            try
            {
                return Directory.Exists(NormalizePath(path));
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error checking directory existence at '{path}': {ex.Message}");
                return false; // Fallback to false on error
            }
        }

        /// <summary>
        /// Creates a directory at the specified path if it does not exist.
        /// </summary>
        public static bool AssetDirectoryExists(string path)
        {
            try
            {
                return AssetDatabase.IsValidFolder(NormalizePath(path))
                    && AssetDatabase.AssetPathExists(NormalizePath(path));
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error checking asset directory existence at '{path}': {ex.Message}");
                return false; // Fallback to false on error
            }
        }

        /// <summary>
        /// Creates a directory at the specified path if it does not exist.
        /// </summary>
        public static string CreateDirectory(string path)
        {
            try
            {
                var normalizedPath = NormalizePath(path);
                if (DirectoryExists(normalizedPath))
                {
                    Directory.CreateDirectory(normalizedPath);
                }
                return normalizedPath;
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error creating directory at '{path}': {ex.Message}");
                return null; // Fallback to null on error
            }
        }

        /// <summary>
        /// Creates a directory at the specified path if it does not exist.
        /// </summary>
        public static string CreateAssetDirectory(string parentDir, string newFolderName)
        {
            try
            {
                var newFolderPath = Path.Combine(parentDir, newFolderName);
                newFolderPath = NormalizePath(newFolderPath);
                if (!AssetDatabase.IsValidFolder(newFolderPath))
                    return AssetDatabase.CreateFolder(parentDir, newFolderName);
                return newFolderPath;
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error creating asset directory '{newFolderName}' in '{parentDir}': {ex.Message}");
                return null; // Fallback to null on error
            }
        }

        /// <summary>
        /// Creates the full directory path in the Unity project (relative to 'Assets/').
        /// Recursively creates all intermediate folders if they don't exist.
        /// Returns the final folder path created or confirmed.
        /// </summary>
        public static string CreateAssetDirectoryRecursive(string unityPath)
        {
            try
            {
                var path = NormalizePath(unityPath);

                if (string.IsNullOrEmpty(path))
                    throw new ArgumentException("Path cannot be null or empty.", nameof(path));

                string[] parts = path.Split(Path.DirectorySeparatorChar);
                if (parts.Length == 0 || parts[0] != "Assets")
                    throw new ArgumentException($"Path ({path}) must start with 'Assets'", nameof(path));

                string currentPath = parts[0]; // "Assets"

                for (int i = 1; i < parts.Length; i++)
                {
                    string nextPath = CombinePaths(currentPath, parts[i]);
                    if (!AssetDatabase.IsValidFolder(nextPath))
                    {
                        AssetDatabase.CreateFolder(currentPath, parts[i]);
                    }
                    currentPath = nextPath;
                }

                return currentPath;
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error creating asset directory '{unityPath}': {ex.Message}");
                return null; // Fallback to null on error
            }
        }

        /// <summary>
        /// Converts a full path into a relative path based on the given root.
        /// Normalizes both paths before comparison.
        /// </summary>
        public static string GetRelativePath(string rootPath, string fullPath)
        {
            try
            {
                if (string.IsNullOrEmpty(rootPath) || string.IsNullOrEmpty(fullPath))
                    return null;

                rootPath = NormalizePath(Path.GetFullPath(rootPath));
                fullPath = NormalizePath(Path.GetFullPath(fullPath));

                if (!fullPath.StartsWith(rootPath))
                {
                    Debug.LogWarning($"GetRelativePath: '{fullPath}' is not under root '{rootPath}'");
                    return fullPath; // fallback to full path
                }

                string relative = fullPath[rootPath.Length..].TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                return relative;
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error getting relative path from '{fullPath}' with root '{rootPath}': {ex.Message}");
                return null; // Fallback to null on error
            }
        }

        /// <summary>
        /// Converts an absolute path to a Unity-relative path starting with \"Assets/\".
        /// Returns null if the path is outside the Unity project folder.
        /// </summary>
        public static string GetUnityRelativePath(string absolutePath)
        {
            try
            {
                if (string.IsNullOrEmpty(absolutePath)) return null;

                string projectPath = Path.GetFullPath(Application.dataPath).Replace("\\", "/");

                //projectPath = projectPath.Substring(0, projectPath.Length - "Assets".Length); // Project root
                projectPath = projectPath[..^"Assets".Length]; // Project root

                string normalizedFullPath = Path.GetFullPath(absolutePath).Replace("\\", "/");

                if (!normalizedFullPath.StartsWith(projectPath))
                {
                    Debug.LogWarning($"Path '{normalizedFullPath}' is outside the Unity project.");
                    return null;
                }

                return normalizedFullPath[projectPath.Length..];
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error getting Unity-relative path from '{absolutePath}': {ex.Message}");
                return null; // Fallback to null on error
            }
        }

        /// <summary>
        /// Combines a root path with a relative subpath to form an absolute path.
        /// </summary>
        public static string GetAbsolutePath(string rootPath, string relativePath)
        {
            try
            {
                if (string.IsNullOrEmpty(rootPath)) return null;
                if (string.IsNullOrEmpty(relativePath)) return NormalizePath(rootPath);

                string combined = Path.Combine(rootPath, relativePath);
                return NormalizePath(Path.GetFullPath(combined));
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Error getting absolute path from '{rootPath}' and '{relativePath}': {ex.Message}");
                return null; // Fallback to null on error
            }
        }

        /// <summary>
        /// Deletes a file at the specified path if it exists.
        /// </summary>
        public static void DeleteFile(string path)
        {
            try
            {
                var normalizedPath = NormalizePath(path);
                if (FileExists(normalizedPath))
                {
                    File.Delete(normalizedPath);
                }
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Unexpected error deleting file '{path}': {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a directory and all its contents at the specified path.
        /// </summary>
        public static void DeleteDirectory(string path)
        {
            try
            {
                var normalizedPath = NormalizePath(path);
                if (DirectoryExists(normalizedPath))
                {
                    Directory.Delete(normalizedPath, true);
                }
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Unexpected error deleting directory '{path}': {ex.Message}");
            }
        }

        /// <summary>
        /// Saves string content to a file at the given path.
        /// </summary>
        public static void WriteAllText(string path, string content)
        {
            try
            {
                var normalizedPath = NormalizePath(path);
                if (!string.IsNullOrEmpty(content))
                {
                    File.WriteAllText(normalizedPath, content);
                }
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Unexpected error writing to file '{path}': {ex.Message}");
            }
        }

        /// <summary>
        /// Loads string content from a file at the given path.
        /// </summary>
        public static string ReadAllText(string path)
        {
            try
            {
                var normalizedPath = NormalizePath(path);

                if (FileExists(normalizedPath))
                {
                    return File.ReadAllText(normalizedPath);
                }

                return null;
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Unexpected error reading from file '{path}': {ex.Message}");
                return null; // Fallback to null on error
            }
        }

        /// <summary>
        /// Saves binary data to a file at the given path.
        /// </summary>
        public static void SaveBinary(string path, byte[] data)
        {
            try
            {
                var normalizedPath = NormalizePath(path);

                if (data != null && data.Length > 0)
                {
                    File.WriteAllBytes(normalizedPath, data);
                }
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Unexpected error writing binary data to file '{path}': {ex.Message}");
            }
        }

        /// <summary>
        /// Loads binary data from a file at the given path.
        /// </summary>
        public static byte[] LoadBinary(string path)
        {
            try
            {
                var normalizedPath = NormalizePath(path);
                return FileExists(normalizedPath) ? File.ReadAllBytes(normalizedPath) : null;
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Unexpected error reading binary data from file '{path}': {ex.Message}");
                return null; // Fallback to null on error
            }
        }

        /// <summary>
        /// Serializes an object to JSON and saves it to the given path using Newtonsoft.Json.
        /// </summary>
        public static void SaveJson<T>(string path, T data, bool prettyPrint = true)
        {
            try
            {
                var normalizedPath = NormalizePath(path);
                string json = JsonConvert.SerializeObject(data, prettyPrint ? Formatting.Indented : Formatting.None);
                WriteAllText(normalizedPath, json);
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Unexpected error saving JSON to file '{path}': {ex.Message}");
            }
        }

        /// <summary>
        /// Loads JSON from a file and deserializes it into an object of type T using Newtonsoft.Json.
        /// </summary>
        public static T LoadJson<T>(string path)
        {
            try
            {
                var normalizedPath = NormalizePath(path);
                string json = ReadAllText(normalizedPath);
                return !string.IsNullOrEmpty(json) ? JsonConvert.DeserializeObject<T>(json) : default;
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"Unexpected error loading JSON from file '{path}': {ex.Message}");
                return default; // Fallback to default value on error
            }
        }
    }
}
#endif
