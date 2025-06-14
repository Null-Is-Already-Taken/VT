#if UNITY_EDITOR
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
        public List<AssetEntry> Entries => Config.assetsEntries;

        /// <summary>Base path for local asset store packages.</summary>
        public string ParentPath => parentPath;

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
            parentPath = PathUtils.GetAssetStoreBasePath();
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

            config.assetsEntries ??= new List<AssetEntry>();

            bool exists = config.assetsEntries.Any(e =>
                e.sourceType == entry.sourceType && e.path == entry.path
            );

            if (!exists)
            {
                config.assetsEntries.Add(entry);
                InternalLogger.Instance.LogDebug($"[Model] Entry added. Total now: {config.assetsEntries.Count}");
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
            if (config?.assetsEntries == null)
                return;

            if (config.assetsEntries.Remove(entry))
                SaveConfig();
            else
                InternalLogger.Instance.LogDebug($"[Model] Attempted remove of missing entry: {entry}");
        }

        //--- File Existence Caching ---//

        /// <summary>
        /// Returns true if the entry is a Git URL or the local file exists (with caching).
        /// </summary>
        public bool FileExists(AssetEntry entry)
        {
            if (entry.sourceType == PackageSourceType.GitURL)
                return true;

            string full = IOManager.CombinePaths(ParentPath, entry.path);
            if (!fileExistenceCache.TryGetValue(full, out bool exists))
            {
                exists = IOManager.FileExists(full);
                fileExistenceCache[full] = exists;
            }

            return exists;
        }

        //--- UI Handlers ---//

        /// <summary>
        /// Opens a file panel to add a local .unitypackage entry.
        /// </summary>
        public void HandleAddLocal()
        {
            string absolute = EditorUtility.OpenFilePanel(
                "Select UnityPackage",
                ParentPath,
                "unitypackage"
            );
            if (string.IsNullOrEmpty(absolute))
                return;

            string relative = IOManager.GetRelativePath(ParentPath, absolute);
            if (string.IsNullOrEmpty(relative))
                return;

            var entry = new AssetEntry
            {
                sourceType = PackageSourceType.LocalUnityPackage,
                path = relative
            };
            AddEntry(entry);
        }

        /// <summary>
        /// Validates and adds a Git URL entry.
        /// </summary>
        public void HandleAddGit(string gitURL)
        {
            if (string.IsNullOrWhiteSpace(gitURL))
                return;

            if (!gitURL.StartsWith("https://") && !gitURL.StartsWith("git@"))
            {
                InternalLogger.Instance.LogError("[Model] Invalid Git URL.");
                return;
            }

            if (Entries.Any(e => e.sourceType == PackageSourceType.GitURL && e.path == gitURL))
            {
                InternalLogger.Instance.LogWarning("[Model] Duplicate Git URL.");
                return;
            }

            var entry = new AssetEntry
            {
                sourceType = PackageSourceType.GitURL,
                path = gitURL
            };
            AddEntry(entry);
        }

        /// <summary>
        /// Opens a file panel to locate and update a missing local entry.
        /// </summary>
        public void HandleLocate(int index)
        {
            if (index < 0 || index >= Entries.Count)
                return;

            var entry = Entries[index];
            string selected = EditorUtility.OpenFilePanel(
                "Locate UnityPackage",
                ParentPath,
                "unitypackage"
            );
            if (string.IsNullOrEmpty(selected))
                return;

            string relative = IOManager.GetRelativePath(ParentPath, selected);
            if (string.IsNullOrEmpty(relative))
                return;

            bool duplicate = Entries.Any(e =>
                e != entry && e.sourceType == PackageSourceType.LocalUnityPackage && e.path == relative
            );
            if (duplicate)
                return;

            entry.sourceType = PackageSourceType.LocalUnityPackage;
            entry.path = relative;
            SaveConfig();
        }

        /// <summary>
        /// Asynchronously imports all entries: local .unitypackages or Git URLs via UPM.
        /// </summary>
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
                    Debug.LogError($"[Model] Import failed for '{e.path}': {ex.Message}");
                }
            }
            AssetDatabase.Refresh();
        }

        //--- Private Fields ---//

        private AssetsConfig config;
        private readonly string parentPath;
        private const string configFolderPath = "Assets/EssentialAssetsImporter/ConfigData";
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
