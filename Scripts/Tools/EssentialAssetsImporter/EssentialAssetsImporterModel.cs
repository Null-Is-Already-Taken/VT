﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using VT.Editor.Utils;
using VT.IO;
using VT.Logger;

namespace VT.Tools.EssentialAssetsImporter
{
    /// <summary>
    /// Encapsulates all data operations for the Essential Assets Importer,
    /// including config loading, entry management, and file-existence caching.
    /// </summary>
    public class EssentialAssetsImporterModel
    {
        //--- Public Properties ---//

        /// <summary>Currently loaded asset entries.</summary>
        public AssetEntryList Entries => Config.Entries;

        /// <summary>Base path for local asset store packages.</summary>
        //public string ParentPath => parentPath;

        /// <summary>Folder where AssetsConfig assets are stored.</summary>
        public string ConfigFolderPath => configFolderPath;

        /// <summary>Currently selected AssetsConfig ScriptableObject.</summary>
        public AssetsConfig Config
        {
            get
            {
                if (config == null)
                {
                    LoadConfig();
                }
                return config;
            }
        }

        //--- Constructor ---//

        /// <summary>
        /// Constructs the model and initializes the config folder.
        /// </summary>
        public EssentialAssetsImporterModel()
        {
            configFolderPath = IOManager.CombinePaths("Assets", "EssentialAssetsImporter", "ConfigData");
            configFilePath = IOManager.CombinePaths(configFolderPath, "ConfigData.json");
            upmRegistryFilePath = IOManager.CombinePaths(configFolderPath, "UnityPackageRegistry.json");

            fileExistenceCache = new();

            // Ensure the config folder exists
            if (!IOManager.AssetDirectoryExists(configFolderPath))
            {
                IOManager.CreateAssetDirectoryRecursive(configFolderPath);
            }
        }

        //--- Config Enumeration ---//

        /// <summary>
        /// Enumerates all AssetsConfig assets under the config folder.
        /// </summary>
        public IEnumerable<AssetsConfig> GetAllConfigs()
        {
            var guids = AssetDatabase.FindAssets(
                $"t:{nameof(AssetsConfig)}",
                new[] { configFolderPath }
            );
            foreach (var g in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                yield return AssetDatabase.LoadAssetAtPath<AssetsConfig>(path);
            }
        }

        /// <summary>
        /// Selects the given AssetsConfig as the active config.
        /// </summary>
        public void SelectConfig(AssetsConfig config)
        {
            this.config = config;
        }

        //--- Config Loading & Saving ---//

        /// <summary>
        /// Loads the active config from disk, or creates a default if none exist.
        /// </summary>
        public void LoadConfig()
        {
            string[] guids = AssetDatabase.FindAssets(
                $"t:{nameof(AssetsConfig)}",
                new[] { configFolderPath }
            );

            if (guids.Length == 1)
            {
                config = LoadConfigAtGUID(guids[0]);
            }
            else if (guids.Length > 1)
            {
                string firstPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                InternalLogger.Instance.LogWarning(
                    $"[Model] Multiple configs in '{configFolderPath}'. Using '{firstPath}'."
                );
                config = LoadConfigAtGUID(guids[0]);
            }
            else
            {
                InternalLogger.Instance.LogDebug(
                    $"[Model] No config found. Creating default in '{configFolderPath}'."
                );
                CreateDefaultConfig();
            }

            fileExistenceCache.Clear();
        }

        /// <summary>
        /// Saves the currently active config asset.
        /// </summary>
        public void SaveConfig()
        {
            if (config == null)
                return;

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            fileExistenceCache.Clear();
        }

        /// <summary>
        /// Reloads the entries of the currently selected config from disk.
        /// </summary>
        public void ReloadCurrentConfig()
        {
            if (config == null)
                return;

            string path = AssetDatabase.GetAssetPath(config);
            config = AssetDatabase.LoadAssetAtPath<AssetsConfig>(path);
            fileExistenceCache.Clear();
        }

        //--- Entry Management ---//

        /// <summary>
        /// Adds a new entry if it doesn't already exist, then saves config.
        /// </summary>
        public void AddEntry(AssetEntry entry)
        {
            InternalLogger.Instance.LogDebug($"[Model] AddEntry: {entry}");
            if (entry == null)
            {
                InternalLogger.Instance.LogError("[Model] Cannot add null entry.");
                return;
            }

            if (config == null)
                LoadConfig();

            if (config.Entries == null)
                config.Init();

            bool exists = config.Exists(entry);

            if (!exists)
            {
                config.Entries.Add(entry);
                InternalLogger.Instance.LogDebug($"[Model] Entry added. Total now: {config.Entries.Count}");
                SaveConfig();
            }
            else
            {
                InternalLogger.Instance.LogWarning($"[Model] Duplicate entry skipped: {entry}");
            }
        }

        /// <summary>
        /// Removes the given entry if present, then saves config.
        /// </summary>
        public void RemoveEntry(AssetEntry entry)
        {
            if (config == null || config.Entries == null)
                return;

            if (config.Entries.Remove(entry))
                SaveConfig();
            else
                InternalLogger.Instance.LogDebug($"[Model] Attempted remove of missing entry: {entry}");
        }

        /// <summary>
        /// Opens a file panel to add a local .unitypackage entry.
        /// </summary>
        public void AddLocalEntry(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
                return;

            AddEntry(AssetEntry.Create(PackageSourceType.LocalUnityPackage, absolutePath));
        }

        /// <summary>
        /// Validates and adds a Git URL entry.
        /// </summary>
        public void AddGitEntry(string gitURL)
        {
            if (string.IsNullOrWhiteSpace(gitURL))
                return;

            if (!gitURL.StartsWith("https://") && !gitURL.StartsWith("git@"))
            {
                InternalLogger.Instance.LogError("[Model] Invalid Git URL.");
                return;
            }

            var entry = AssetEntry.Create(PackageSourceType.GitURL, gitURL);

            if (config.Exists(entry))
            {
                InternalLogger.Instance.LogWarning("[Model] Duplicate Git URL.");
                return;
            }

            AddEntry(entry);
        }

        /// <summary>
        /// Update a missing local entry.
        /// </summary>
        private bool LocateMissingEntry(int index, string absolutePath)
        {
            var entry = AssetEntry.Create(PackageSourceType.LocalUnityPackage, absolutePath);

            if (Entries.list.Any(e => e.Equals(entry)))
            {
                InternalLogger.Instance.LogWarning($"[Model] Entry [{entry}] already exists in config.");
                return false;
            }

            if (entry.ToString() != config.Entries[index].ToString())
            {
                InternalLogger.Instance.LogWarning($"[Model] Located entry mismatched. Expected: {config.Entries[index]}, Found: {entry}");
                return false;
            }

            config.Entries[index] = entry;

            SaveConfig();

            return true;
        }

        //--- File Existence Caching ---//

        /// <summary>
        /// Returns true if the entry is a Git URL or the local file exists (with caching).
        /// </summary>
        public bool FileExists(AssetEntry entry)
        {
            if (entry == null)
                return false;

            if (entry.sourceType == PackageSourceType.GitURL)
                return true;

            string full = entry.GetFullPath();

            if (!fileExistenceCache.TryGetValue(full, out bool exists))
            {
                exists = IOManager.FileExists(full);
                fileExistenceCache[full] = exists;
            }

            return exists;
        }

        //--- Importing Assets ---//

        /// <summary>
        /// Asynchronously imports all entries: local .unitypackages or Git URLs via UPM.
        /// </summary>
        public async Task ImportAsync(AssetsConfig assetsConfig)
        {
            var entries = assetsConfig.Entries;
            int total = entries.Count;
            for (int i = 0; i < total; i++)
            {
                var e = entries[i];
                float progress = (float)i / total;

                string fullPath;
                if (e.sourceType == PackageSourceType.LocalUnityPackage)
                {
                    fullPath = PathUtils.FromAlias(e.AliasPath);
                }
                else
                {
                    fullPath = e.GetFullPath();
                }

                try
                {
                    if (e.sourceType == PackageSourceType.LocalUnityPackage && FileExists(e))
                    {
                        AssetDatabase.ImportPackage(fullPath, false);
                    }
                    else if (e.sourceType == PackageSourceType.GitURL)
                    {
                        await UPMClientWrapper.AddPackageAsync(e.GetFullPath());
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Model] Import failed for '{fullPath}': {ex.Message}");
                }
            }

            AssetDatabase.Refresh();
        }


        //--- UI Handlers ---//

        public void HandleSaveConfigToJSON()
        {
            string uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(configFilePath);
            IOManager.SaveJson(uniqueAssetPath, config.Entries, true);
            AssetDatabase.Refresh();
        }

        public void HandleLoadConfigFromJSON(string path)
        {
            // should use something like this ScriptableObject.CreateInstance<AssetsConfig>();
            var entries = IOManager.LoadJson<AssetEntryList>(path);
            config.AssignEntries(entries);
        }

        public void HandleAddLocal(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
                return;

            AddLocalEntry(absolutePath);
        }

        public void HandleAddGit(string gitURL)
        {
            AddGitEntry(gitURL);
        }

        public void HandleLocate(int index, string absolutePath)
        {
            if (index < 0 || index >= Entries.Count)
                return;

            if (string.IsNullOrEmpty(absolutePath))
                return;

            LocateMissingEntry(index, absolutePath);
        }

        public async Task HandleImportAsync()
        {
            await ImportAsync(Config);
        }

        //--- Private Fields ---//

        private AssetsConfig config;
        private readonly string configFolderPath;
        private readonly string configFilePath;
        private readonly string upmRegistryFilePath;
        private readonly Dictionary<string, bool> fileExistenceCache;

        //--- Private Helpers ---//

        /// <summary>
        /// Loads an AssetsConfig from a GUID string.
        /// </summary>
        private AssetsConfig LoadConfigAtGUID(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            InternalLogger.Instance.LogDebug($"[Model] Loaded config at: {path}");
            return AssetDatabase.LoadAssetAtPath<AssetsConfig>(path);
        }

        /// <summary>
        /// Creates a default AssetsConfig asset in the config folder.
        /// </summary>
        private void CreateDefaultConfig()
        {
            if (!IOManager.AssetDirectoryExists(configFolderPath))
                IOManager.CreateAssetDirectoryRecursive(configFolderPath);

            string assetPath = IOManager.CombinePaths(configFolderPath, "DefaultAssetsConfig.asset");
            config = ScriptableObject.CreateInstance<AssetsConfig>();
            AssetDatabase.CreateAsset(config, assetPath);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(config);
            InternalLogger.Instance.LogDebug($"[Model] Created new config at: {assetPath}");
        }
    }
}
#endif
