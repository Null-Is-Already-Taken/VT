using VT.IO;

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
            catch (System.Exception)
            {
                return PathError.AccessDenied;
            }

            return PathError.None;
        }

        public static string GetPath(ScriptData scriptData, string directory)
        {
            if (string.IsNullOrWhiteSpace(directory) || !IOManager.DirectoryExists(directory))
            {
                throw new System.ArgumentException("Invalid directory path.", nameof(directory));
            }

            var fileName = $"{scriptData.ClassName}.cs";
            return IOManager.CombinePaths(directory, fileName);
        }
    }
}
