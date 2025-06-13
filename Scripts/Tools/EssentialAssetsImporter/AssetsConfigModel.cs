#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VT.Editor.Utils;
using VT.IO;
using VT.Logger;

namespace VT.Tools.EssentialAssetsImporter
{
    // Models/IAssetsConfigModel.cs
    public interface IAssetsConfigModel
    {
        List<AssetEntry> Entries { get; }
        string ParentPath { get; }
        string ConfigFolderPath { get; }
        AssetsConfig Config { get; }
        IEnumerable<AssetsConfig> GetAllConfigs();
        void SelectConfig(AssetsConfig config);
        void ReloadCurrentConfig();
        void LoadConfig();
        void SaveConfig();
        void AddEntry(AssetEntry entry);
        void RemoveEntry(AssetEntry entry);
        bool FileExists(AssetEntry entry);
    }

    // Models/AssetsConfigModel.cs
    public class AssetsConfigModel : IAssetsConfigModel
    {
        public List<AssetEntry> Entries => config.assetsEntries;
        public string ParentPath => parentPath;
        public string ConfigFolderPath => configFolderPath;
        public AssetsConfig Config => config;

        //public AssetsConfigModel()
        //{
        //    parentPath = PathUtils.GetAssetStoreBasePath();
        //}

        public IEnumerable<AssetsConfig> GetAllConfigs()
        {
            // find every AssetsConfig.asset under that folder
            var guids = AssetDatabase.FindAssets("t:AssetsConfig", new[] { configFolderPath });
            foreach (var g in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                yield return AssetDatabase.LoadAssetAtPath<AssetsConfig>(path);
            }
        }

        public void SelectConfig(AssetsConfig config)
        {
            // directly set your private `config` instance
            this.config = config;
        }

        /// <summary>
        /// Loads entries from whatever config instance is already assigned to `config`.
        /// Does NOT touch the configFolderPath or pick a new asset.
        /// </summary>
        public void ReloadCurrentConfig()
        {
            if (config == null) return;

            // simply re-load the same ScriptableObject from disk, so any external edits are picked up
            var path = AssetDatabase.GetAssetPath(config);
            config = AssetDatabase.LoadAssetAtPath<AssetsConfig>(path);

            fileExistenceCache.Clear();
        }

        public void LoadConfig()
        {
            string[] guids = AssetDatabase.FindAssets("t:AssetsConfig", new[] { configFolderPath });

            if (guids.Length == 1)
            {
                config = LoadConfigAtGUID(guids[0]);
            }
            else if (guids.Length > 1)
            {
                //config = HandleMultipleConfigs(guids);

                // multiple configs found—load the first and warn
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                InternalLogger.Instance.LogWarning($"[AssetsConfigModel] Multiple configs found in '{configFolderPath}'. " + $"Using '{path}'.");
                config = LoadConfigAtGUID(guids[0]);
            }
            else
            {
                // none found → create default in that folder
                InternalLogger.Instance.LogDebug($"[AssetsConfigModel] No config in '{configFolderPath}', creating default.");
                CreateDefaultConfig();
            }

            // reset cache so that the UI will re-poll existence
            fileExistenceCache.Clear();
        }

        public void SaveConfig()
        {
            if (config == null) return;

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            fileExistenceCache.Clear();
        }

        public void AddEntry(AssetEntry entry)
        {
            InternalLogger.Instance.LogDebug($"[AssetsConfigModel] AddEntry called with: {entry}");

            if (entry == null)
            {
                InternalLogger.Instance.LogError("[AssetsConfigModel] AddEntry: entry is null!");
                return;
            }

            if (config == null)
            {
                InternalLogger.Instance.LogDebug("[AssetsConfigModel] config is null, calling LoadConfig()");
                LoadConfig();
            }

            if (config.assetsEntries == null)
            {
                InternalLogger.Instance.LogDebug("[AssetsConfigModel] assetsEntries was null, initializing list");
                config.assetsEntries = new List<AssetEntry>();
            }

            bool exists = config.assetsEntries.Exists(e =>
                e.sourceType == entry.sourceType &&
                e.path == entry.path);
            InternalLogger.Instance.LogDebug($"[AssetsConfigModel] Entry exists? {exists}");

            if (!exists)
            {
                config.assetsEntries.Add(entry);
                InternalLogger.Instance.LogDebug($"[AssetsConfigModel] Added entry: {entry}");
                SaveConfig();
                InternalLogger.Instance.LogDebug($"[AssetsConfigModel] SaveConfig complete, total entries now: {config.assetsEntries.Count}");
            }
            else
            {
                InternalLogger.Instance.LogWarning($"[AssetsConfigModel] Skipping AddEntry because it already exists: {entry}");
            }
        }

        public void RemoveEntry(AssetEntry entry)
        {
            // Remove logic
            if (config == null || config.assetsEntries == null) return;

            bool removed = config.assetsEntries.Remove(entry);
            if (removed)
            {
                SaveConfig();
            }
            else
            {
                InternalLogger.Instance.LogWarning($"Tried to remove non-existent entry: {entry}");
            }
        }

        public bool FileExists(AssetEntry entry)
        {
            var full = IOManager.CombinePaths(parentPath, entry.path);
            if (!fileExistenceCache.TryGetValue(full, out var exists))
            {
                exists = IOManager.FileExists(full);
                fileExistenceCache[full] = exists;
            }
            return exists;
        }

        private AssetsConfig config;
        private readonly string parentPath = PathUtils.GetAssetStoreBasePath();
        private const string configFolderPath = "Assets/EssentialAssetsImporter/ConfigData";
        private readonly Dictionary<string, bool> fileExistenceCache = new();

        private AssetsConfig LoadConfigAtGUID(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            InternalLogger.Instance.LogDebug("Loaded config: " + path);
            return AssetDatabase.LoadAssetAtPath<AssetsConfig>(path);
        }

        private void CreateDefaultConfig()
        {
            if (!IOManager.AssetDirectoryExists(configFolderPath))
            {
                IOManager.CreateAssetDirectoryRecursive(configFolderPath);
            }

            if (IOManager.AssetDirectoryExists(configFolderPath))
            {
                var assetPath = IOManager.CombinePaths(configFolderPath, "DefaultAssetsConfig.asset");
                config = ScriptableObject.CreateInstance<AssetsConfig>();
                AssetDatabase.CreateAsset(config, assetPath);
                AssetDatabase.SaveAssets();

                EditorGUIUtility.PingObject(config);

                InternalLogger.Instance.LogDebug("Created new AssetsConfig at: " + assetPath);
            }
        }
    }
}
#endif
