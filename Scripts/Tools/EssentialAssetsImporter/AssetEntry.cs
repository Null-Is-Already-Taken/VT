using System;
using VT.Editor.Utils;
using VT.IO;

namespace VT.Tools.EssentialAssetsImporter
{
    public enum PackageSourceType
    {
        LocalUnityPackage,
        GitURL
    }

    [Serializable]
    public class AssetEntry
    {
        public PackageSourceType sourceType = PackageSourceType.LocalUnityPackage;

        public string path;

        /// <summary>
        /// Returns a formatted summary of the package source, such as "Cysharp - UniTask".
        /// </summary>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            if (sourceType == PackageSourceType.GitURL)
                return EmbeddedIcons.Internet_Unicode + " " + ExtractGitSummary(path);

            if (sourceType == PackageSourceType.LocalUnityPackage)
                return EmbeddedIcons.Package_Unicode + " " + ExtractLocalSummary(path);

            return string.Empty;
        }

        private string ExtractLocalSummary(string localPath)
        {
            var parts = localPath.Split('/', '\\');
            if (parts.Length < 2)
                return string.Join(" - ", parts);

            string author = parts[0];
            string fileName = parts[^1];
            string packageName = IOManager.GetFileNameWithoutExtension(fileName);

            return $"{author} - {packageName}";
        }

        private string ExtractGitSummary(string gitURL)
        {
            if (string.IsNullOrWhiteSpace(gitURL))
                return string.Empty;

            string author = string.Empty;
            string repoName = string.Empty;

            if (gitURL.StartsWith("git@"))
            {
                // SSH format: git@github.com:Org/Repo.git
                var parts = gitURL.Split(':');
                if (parts.Length == 2)
                {
                    var pathParts = parts[1].Split('/');
                    if (pathParts.Length >= 2)
                    {
                        author = pathParts[0];
                        repoName = pathParts[1];
                    }
                }
            }
            else if (Uri.TryCreate(gitURL, UriKind.Absolute, out var uri))
            {
                var pathParts = uri.AbsolutePath.Trim('/').Split('/');
                if (pathParts.Length >= 2)
                {
                    author = pathParts[0];
                    repoName = pathParts[1];
                }
            }

            if (repoName.EndsWith(".git"))
                repoName = repoName[..^4];

            return string.IsNullOrEmpty(author) || string.IsNullOrEmpty(repoName)
                ? gitURL
                : $"{author} - {repoName}";
        }
    }
}
