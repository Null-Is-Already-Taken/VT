using System;
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
                return ExtractGitSummary(path);

            var parts = path.Split('/', '\\');
            if (parts.Length < 2)
                return string.Join(" - ", parts);

            string author = parts[0];
            string fileName = parts[^1];
            string packageName = IOManager.GetFileNameWithoutExtension(fileName);

            return $"{author} - {packageName}";
        }

        private string ExtractGitSummary(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            string author = string.Empty;
            string repoName = string.Empty;

            if (url.StartsWith("git@"))
            {
                // SSH format: git@github.com:Org/Repo.git
                var parts = url.Split(':');
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
            else if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
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
                ? url
                : $"{author} - {repoName}";
        }
    }
}
