#if UNITY_EDITOR

using System;
using UnityEditor;
using VT.IO;
using VT.Logger;

namespace VT.Tools.ScriptCreator
{
    public enum PathError
    {
        None,
        InvalidPath,
        FileAlreadyExists,
        DirectoryNotFound,
        AccessDenied
    }

    public static class ScriptCreatorIOService
    {
        public static PathError ValidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return PathError.InvalidPath;
            }

            if (System.IO.File.Exists(path))
            {
                return PathError.FileAlreadyExists;
            }

            try
            {
                var directory = IOManager.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && !IOManager.DirectoryExists(directory))
                {
                    return PathError.DirectoryNotFound;
                }
            }
            catch (Exception)
            {
                return PathError.AccessDenied;
            }

            return PathError.None;
        }

        public static string GetPath(ScriptData scriptData, string directory)
        {
            if (string.IsNullOrWhiteSpace(directory) || !IOManager.DirectoryExists(directory))
            {
                throw new ArgumentException("Invalid directory path.", nameof(directory));
            }

            var fileName = $"{scriptData.ClassName}.cs";
            return IOManager.CombinePaths(directory, fileName);
        }

        public static void Save(string path, ScriptData data)
        {
            path = IOManager.NormalizePath(path);

            try
            {
                IOManager.WriteAllText(path, data.Content);
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"[Model] Error saving script: {ex.Message}");
            }
        }
    }
}

#endif
