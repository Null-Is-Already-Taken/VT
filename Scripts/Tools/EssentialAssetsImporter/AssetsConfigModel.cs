#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using VT.Editor.Utils;
using VT.IO;
using VT.Logger;

namespace VT.Tools.EssentialAssetsImporter
{
    // Models/IAssetsConfigModel.cs
    //public interface IAssetsConfigModel
    //{
    //    List<AssetEntry> Entries { get; }
    //    string ParentPath { get; }
    //    string ConfigFolderPath { get; }
    //    AssetsConfig Config { get; }
    //    IEnumerable<AssetsConfig> GetAllConfigs();
    //    void SelectConfig(AssetsConfig config);
    //    void ReloadCurrentConfig();
    //    void LoadConfig();
    //    void SaveConfig();
    //    void AddEntry(AssetEntry entry);
    //    void RemoveEntry(AssetEntry entry);
    //    bool FileExists(AssetEntry entry);
    //    void HandleAddLocal();
    //    void HandleAddGit(string gitURL);
    //    void HandleLocate(int index);
    //    void HandleImport();
    //}

    // Models/AssetsConfigModel.cs
    public class AssetsConfigModel //: IAssetsConfigModel
    {
        public List<AssetEntry> Entries => config.assetsEntries;
        public string ParentPath => parentPath;
        public string ConfigFolderPath => configFolderPath;
        public AssetsConfig Config => config;

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
                InternalLogger.Instance.LogDebug($"[AssetsConfigModel] Tried to remove non-existent entry: {entry}");
            }
        }

        public bool FileExists(AssetEntry entry)
        {
            if (entry.sourceType == PackageSourceType.GitURL) return true; // Git URLs are always considered "exists" since they are not local files

            var full = IOManager.CombinePaths(parentPath, entry.path);
            if (!fileExistenceCache.TryGetValue(full, out var exists))
            {
                exists = IOManager.FileExists(full);
                fileExistenceCache[full] = exists;
            }
            return exists;
        }

        public void HandleAddLocal()
        {
            // open a file dialog to get file path
            string absolute = EditorUtility.OpenFilePanel(
                "Select UnityPackage",
                ParentPath,
                "unitypackage"
            );

            if (string.IsNullOrEmpty(absolute)) return;

            string relative = IOManager.GetRelativePath(ParentPath, absolute);

            if (string.IsNullOrEmpty(relative) || !relative.EndsWith(".unitypackage"))
            {
                return;
            }

            var entry = new AssetEntry
            {
                sourceType = PackageSourceType.LocalUnityPackage,
                path = relative
            };

            AddEntry(entry);
        }

        public void HandleAddGit(string gitURL)
        {
            if (string.IsNullOrWhiteSpace(gitURL))
            {
                InternalLogger.Instance.LogWarning("Git URL is empty.");
                return;
            }

            if (!gitURL.StartsWith("https://") && !gitURL.StartsWith("git@"))
            {
                InternalLogger.Instance.LogError("Invalid Git URL.");
                return;
            }

            bool isDuplicate = Entries.Exists(entry => entry.sourceType == PackageSourceType.GitURL && entry.path == gitURL);

            if (isDuplicate)
            {
                InternalLogger.Instance.LogWarning("This Git URL is already in the config.");
                return;
            }

            var entry = new AssetEntry
            {
                sourceType = PackageSourceType.GitURL,
                path = gitURL
            };

            AddEntry(entry);
        }

        public void HandleLocate(int index)
        {
            // 1) bounds‐check
            if (index < 0 || index >= Entries.Count)
            {
                InternalLogger.Instance.LogError($"[AssetsConfigModel] Invalid locate index: {index}");
                return;
            }

            // 2) pick the entry to update
            var entry = Entries[index];

            // 3) show file panel
            string selected = EditorUtility.OpenFilePanel(
                "Locate UnityPackage",
                ParentPath,
                "unitypackage"
            );
            if (string.IsNullOrEmpty(selected) || !selected.EndsWith(".unitypackage"))
                return;

            // 4) convert to relative
            string relative = IOManager.GetRelativePath(ParentPath, selected);
            if (string.IsNullOrEmpty(relative))
            {
                InternalLogger.Instance.LogError("[AssetsConfigModel] Failed to compute relative path");
                return;
            }

            // 5) duplicate check (excluding this entry)
            bool dup = Entries.Exists(e =>
                e != entry &&
                e.sourceType == PackageSourceType.LocalUnityPackage &&
                e.path == relative
            );
            if (dup)
            {
                InternalLogger.Instance.LogWarning("[AssetsConfigModel] This asset is already in the config.");
                return;
            }

            // 6) apply the update on the model
            entry.sourceType = PackageSourceType.LocalUnityPackage;
            entry.path = relative;
            SaveConfig();

            InternalLogger.Instance.LogDebug($"[AssetsConfigModel] Located package #{index} → {relative}");
        }

        public async Task HandleImportAsync()
        {
            int total = Entries.Count;
            for (int i = 0; i < total; i++)
            {
                var e = Entries[i];
                float progress = (float)i / total;

                try
                {
                    if (e.sourceType == PackageSourceType.LocalUnityPackage && FileExists(e))
                    {
                        string fullPath = IOManager.CombinePaths(ParentPath, e.path);
                        AssetDatabase.ImportPackage(fullPath, false);
                    }
                    else if (e.sourceType == PackageSourceType.GitURL)
                    {
                        await UPMClientWrapper.AddPackageAsync(e.path);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to import “{e.path}”: {ex.Message}");
                }
            }

            AssetDatabase.Refresh();
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
