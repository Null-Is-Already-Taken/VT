using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AssetEntryList
    {
        public List<AssetEntry> list = new();
        public int Count => list?.Count ?? 0;
        public void Clear() => list?.Clear();
        public bool IsNullOrEmpty() => list.IsNullOrEmpty();

        public void Add(AssetEntry entry) => list.Add(entry);
        public bool Remove(AssetEntry entry) => list.Remove(entry);

        public AssetEntry this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        public bool Exists(AssetEntry entry)
        {
            return list.Any(e => e.Equals(entry));
        }
    }

    [Serializable]
    public class AssetEntry : IEquatable<AssetEntry>
    {
        //public AssetEntry(PackageSourceType sourceType, string absolutePath)
        //{
        //    this.sourceType = sourceType;

        //    switch (sourceType)
        //    {
        //        case PackageSourceType.LocalUnityPackage:
        //            RelativePath = IOManager.GetRelativePath(PathUtils.GetAssetStorePath(), absolutePath);
        //            AliasPath = PathUtils.ToAlias(absolutePath);
        //            InternalLogger.Instance.LogDebug($"FileExists test: {IOManager.FileExists(PathUtils.FromAlias(AliasPath))}");
        //            break;
        //        case PackageSourceType.GitURL:
        //            RelativePath = absolutePath;
        //            AliasPath = absolutePath;
        //            break;
        //        default:
        //            RelativePath = string.Empty;
        //            AliasPath = string.Empty;
        //            break;
        //    }
        //}

        public static AssetEntry Create(PackageSourceType sourceType, string absolutePath)
        {
            var entry = new AssetEntry
            {
                sourceType = sourceType
            };

            switch (sourceType)
            {
                case PackageSourceType.LocalUnityPackage:
                    entry.RelativePath = IOManager.GetRelativePath(PathUtils.GetAssetStorePath(), absolutePath);
                    entry.AliasPath = PathUtils.ToAlias(absolutePath);
                    break;
                case PackageSourceType.GitURL:
                    entry.RelativePath = absolutePath;
                    entry.AliasPath = absolutePath;
                    break;
            }

            return entry;
        }

        public PackageSourceType sourceType = PackageSourceType.LocalUnityPackage;

        public string GetFullPath() => PathUtils.FromAlias(AliasPath);
        public string RelativePath;
        public string AliasPath;

        /// <summary>
        /// Returns a formatted summary of the package source, such as "Cysharp - UniTask".
        /// </summary>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(RelativePath))
                return string.Empty;

            if (sourceType == PackageSourceType.GitURL)
                return EmbeddedIcons.Internet_Unicode + " " + ExtractGitSummary(RelativePath);

            if (sourceType == PackageSourceType.LocalUnityPackage)
                return EmbeddedIcons.Package_Unicode + " " + ExtractLocalSummary(RelativePath);

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
            && string.Equals(GetFullPath(), other.GetFullPath(), StringComparison.OrdinalIgnoreCase)
            && string.Equals(RelativePath, other.RelativePath, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj) => Equals(obj as AssetEntry);

        public override int GetHashCode()
        {
            return HashCode.Combine(
                sourceType,
                RelativePath?.ToLowerInvariant(),
                AliasPath?.ToLowerInvariant()
            );
        }
    }
}
