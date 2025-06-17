using System;
using System.Diagnostics;
using VT.Editor.Utils;
using VT.IO;
using VT.Logger;

namespace VT.Tools.EssentialAssetsImporter
{
    public enum PackageSourceType
    {
        LocalUnityPackage,
        GitURL
    }

    [Serializable]
    public class AssetEntry : IEquatable<AssetEntry>
    {
        public AssetEntry(PackageSourceType sourceType, string absolutePath)
        {
            this.sourceType = sourceType;

            switch (sourceType)
            {
                case PackageSourceType.LocalUnityPackage:
                    this.absolutePath = PathUtils.ToAlias(absolutePath);
                    relativePath = IOManager.GetRelativePath(PathUtils.GetAssetStorePath(), absolutePath);
                    InternalLogger.Instance.LogDebug($"relativePath: {relativePath}");
                    break;
                case PackageSourceType.GitURL:
                    this.absolutePath = absolutePath;
                    relativePath = absolutePath;
                    break;
                default:
                    this.absolutePath = string.Empty;
                    relativePath = string.Empty;
                    break;
            }
        }

        public string FullPath => IOManager.CombinePaths(PathUtils.GetAssetStorePath(), relativePath);

        public PackageSourceType sourceType = PackageSourceType.LocalUnityPackage;

        public string absolutePath; // with path alias
        public string relativePath;

        /// <summary>
        /// Returns a formatted summary of the package source, such as "Cysharp - UniTask".
        /// </summary>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;

            if (sourceType == PackageSourceType.GitURL)
                return EmbeddedIcons.Internet_Unicode + " " + ExtractGitSummary(relativePath);

            if (sourceType == PackageSourceType.LocalUnityPackage)
                return EmbeddedIcons.Package_Unicode + " " + ExtractLocalSummary(relativePath);

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

        public bool Equals(AssetEntry other)
        {
            return sourceType == other.sourceType
            && string.Equals(absolutePath, other.absolutePath, StringComparison.OrdinalIgnoreCase)
            && string.Equals(relativePath, other.relativePath, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj) => Equals(obj as AssetEntry);

        public override int GetHashCode()
        {
            return HashCode.Combine(
                sourceType,
                absolutePath?.ToLowerInvariant(),
                relativePath?.ToLowerInvariant()
            );
        }
    }
}
