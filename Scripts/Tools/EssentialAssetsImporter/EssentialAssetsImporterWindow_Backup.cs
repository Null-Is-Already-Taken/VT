#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VT.EditorUtils;
using VT.IO;
using VT.Logger;

namespace VT.Tools.EssentialAssetsImporter
{
    public class EssentialAssetsImporterWindow_Backup : EditorWindow
    {
        private string parentPath;
        private AssetsConfig assetsConfig;

        private int currentPage = 0;
        private const int itemsPerPage = 3;
        private const float averageCharWidth = 6f;
        private const float spacing = 10;
        private const string removeButtonText = EmbeddedIcons.Wastebasket_Unicode;
        private const string addLocalButtonText = EmbeddedIcons.Package_Unicode;
        private const string addGlobalButtonText = EmbeddedIcons.Internet_Unicode;
        private const string openButtonText = EmbeddedIcons.FileFolder_Unicode;
        private const string locateButtonText = EmbeddedIcons.LeftPointingMagnifyingGlass_Unicode;
        private const float buttonSize = 24;
        private const string configFolderPath = "Assets/EssentialAssetsImporter/Config Data";
        
        private Dictionary<string, bool> fileExistenceCache = new();
        private double lastCacheRefreshTime;
        private const double cacheRefreshInterval = 10.0; // in seconds
        private int? pendingRemoveIndex = null;


        [MenuItem("Tools/Essential Assets Importer Legacy")]
        public static void OpenWindow()
        {
            var window = GetWindow<EssentialAssetsImporterWindow_Backup>("Essential Assets Importer Legacy");
            window.Show();
        }

        private void OnEnable()
        {
            parentPath = Backend.GetAssetStoreBasePath();
        }

        private void OnGUI()
        {
            DrawConfigObject();
            
            if (assetsConfig != null)
            { 
                DrawPackageList();
                DrawImportAssetButton();
            }
        }

        private void DrawConfigObject()
        {
            EditorGUILayout.BeginHorizontal("helpBox");
            EditorGUILayout.LabelField("Config Loaded", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            GUIContent openFolder = new(openButtonText, "Load Config");
            Backend.StyledButton(openFolder, new Color(1f, 1f, 1f), LoadConfig, Backend.InlineButtonStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(assetsConfig, typeof(AssetsConfig), false);
            EditorGUI.EndDisabledGroup();
        }

        private void DrawPackageList()
        {
            if (assetsConfig.assetsEntries == null) return;

            double currentTime = EditorApplication.timeSinceStartup;

            GUILayout.Space(spacing);
            
            DrawHeaderBar();

            if (assetsConfig.assetsEntries.Count == 0) return;

            var (startIndex, endIndex, totalPages) = GetPagination(assetsConfig.assetsEntries.Count, itemsPerPage, ref currentPage);
            var buttonStyle = Backend.InlineButtonStyle;
            for (int i = startIndex; i < endIndex; i++)
            {
                DrawPackageEntry(i, currentTime, buttonStyle);
            }

            if (pendingRemoveIndex.HasValue)
            {
                RemovePackageAt(pendingRemoveIndex.Value);
            }

            DrawPagination(totalPages);
        }

        private void RemovePackageAt(int index)
        {
            InternalLogger.Instance.LogDebug("Removed: " + assetsConfig.assetsEntries[index].ToString());
            assetsConfig.assetsEntries.RemoveAt(index);
            fileExistenceCache.Clear();
            EditorUtility.SetDirty(assetsConfig);
            AssetDatabase.SaveAssets();
            pendingRemoveIndex = null;
        }

        private void DrawHeaderBar()
        {
            EditorGUILayout.BeginHorizontal("helpBox");
            EditorGUILayout.LabelField("Configured Package List", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            GUIContent addLocalButton = new(addLocalButtonText, "Add local package");
            Backend.StyledButton(addLocalButton, Color.white, AddLocalPackage, Backend.InlineButtonStyle);
            GUIContent addGlobalButton = new(addGlobalButtonText, "Add git package");
            Backend.StyledButton(addGlobalButton, Color.white, AddGitPackage, Backend.InlineButtonStyle);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPackageEntry(int index, double currentTime, GUIStyle buttonStyle)
        {
            var entry = assetsConfig.assetsEntries[index];
            string fullPath = IOManager.CombinePaths(parentPath, entry.path);

            if (ShouldRefreshCache(currentTime) || !fileExistenceCache.ContainsKey(fullPath))
            {
                fileExistenceCache[fullPath] = IOManager.FileExists(fullPath);
                lastCacheRefreshTime = currentTime;
            }

            bool fileExists = entry.sourceType == PackageSourceType.GitURL || fileExistenceCache[fullPath];

            // Compute space based on active buttons
            int buttonCount = fileExists ? 1 : 2;
            float labelWidth = position.width - (buttonCount * buttonSize) - (spacing * buttonCount);

            string fullText = entry.ToString();
            string truncated = Backend.TruncateWithEllipsis(fullText, Backend.EstimateMaxChars(labelWidth, averageCharWidth));
            string tooltip = fileExists ? fullText : $"Missing file: {fullPath}";

            GUIContent labelContent = new(truncated, tooltip);
            GUIStyle entryLabelStyle = new(EditorStyles.label)
            {
                fixedWidth = labelWidth,
                normal = { textColor = fileExists ? Color.white : Color.red }
            };

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();

            // Label
            Backend.StyledLabel(labelContent, entryLabelStyle);
            GUILayout.FlexibleSpace();

            // 🔍 Locate (only if file is missing)
            if (!fileExists)
            {
                GUIContent locateButton = new(locateButtonText, "Locate missing package");
                Backend.StyledButton(
                    locateButton,
                    new Color(1f, 0.8f, 0.3f),
                    () => LocateMissingPackage(index),
                    buttonStyle
                );
            }

            // 🗑 Remove
            GUIContent removeButton = new(removeButtonText, "Remove package");
            Backend.StyledButton(
                removeButton,
                new Color(0.9f, 0.3f, 0.2f),
                () => pendingRemoveIndex = index,
                buttonStyle
            );

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private bool ShouldRefreshCache(double time) => time - lastCacheRefreshTime > cacheRefreshInterval; 

        private void DrawPagination(int totalPages)
        {
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = currentPage > 0;
            if (GUILayout.Button("← Prev", GUILayout.Width(70))) currentPage--;
            GUI.enabled = currentPage < totalPages - 1;
            if (GUILayout.Button("Next →", GUILayout.Width(70))) currentPage++;
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField($"Page {currentPage + 1} of {totalPages}", GUILayout.Width(120));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawImportAssetButton()
        {
            GUILayout.Space(spacing);
            if (assetsConfig != null && assetsConfig.assetsEntries != null && assetsConfig.assetsEntries.Count > 0)
            {
                var importButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 12,
                    fixedHeight = 32,
                    alignment = TextAnchor.MiddleCenter
                };

                Backend.StyledButton("Import All Assets", new Color(0.2f, 0.9f, 0.4f), ImportAllPackages, importButtonStyle);
            }
            else
            {
                EditorGUILayout.HelpBox("No assets configured. Please add assets first.", MessageType.Warning);
            }
        }

        private void LoadConfig()
        {
            string[] guids = AssetDatabase.FindAssets("t:AssetsConfig");

            if (guids.Length == 1)
            {
                assetsConfig = LoadConfigAtGUID(guids[0]);
            }
            else if (guids.Length > 1)
            {
                assetsConfig = HandleMultipleConfigs(guids);
            }
            else
            {
                CreateDefaultConfig();
            }

            fileExistenceCache.Clear();
        }

        private AssetsConfig LoadConfigAtGUID(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            InternalLogger.Instance.LogDebug("Loaded config: " + path);
            return AssetDatabase.LoadAssetAtPath<AssetsConfig>(path);
        }

        private AssetsConfig HandleMultipleConfigs(string[] guids)
        {
            string folder = IOManager.GetDirectoryName(AssetDatabase.GUIDToAssetPath(guids[0]));
            string selectedPath = EditorUtility.OpenFilePanel("Select AssetsConfig", folder, "asset");

            if (string.IsNullOrEmpty(selectedPath))
            {
                InternalLogger.Instance.LogWarning("No config selected. Load aborted.");
                return null;
            }

            string relativePath = IOManager.GetUnityRelativePath(selectedPath);

            var config = AssetDatabase.LoadAssetAtPath<AssetsConfig>(relativePath);

            if (config == null)
            {
                InternalLogger.Instance.LogError("Failed to load selected AssetsConfig. Make sure it's a valid asset.");
            }

            return config;
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
                assetsConfig = CreateInstance<AssetsConfig>();
                AssetDatabase.CreateAsset(assetsConfig, assetPath);
                AssetDatabase.SaveAssets();

                EditorGUIUtility.PingObject(assetsConfig);

                InternalLogger.Instance.LogDebug("Created new AssetsConfig at: " + assetPath);
            }
        }

        private void LocateMissingPackage(int index)
        {
            if (index < 0 || index >= assetsConfig.assetsEntries.Count)
            {
                InternalLogger.Instance.LogError("Invalid index for locating missing package.");
                return;
            }

            var entry = assetsConfig.assetsEntries[index];

            string selectedPath = EditorUtility.OpenFilePanel("Locate UnityPackage", parentPath, "unitypackage");

            if (!string.IsNullOrEmpty(selectedPath) && selectedPath.EndsWith(".unitypackage"))
            {
                string relativePath = IOManager.GetRelativePath(parentPath, selectedPath);

                if (string.IsNullOrEmpty(relativePath))
                {
                    InternalLogger.Instance.LogError("Failed to convert selected path to relative form.");
                    return;
                }

                // Avoid duplicates (excluding self)
                bool isDuplicate = assetsConfig.assetsEntries.Exists(e =>
                    e != entry && e.sourceType == PackageSourceType.LocalUnityPackage && e.path == relativePath);

                if (isDuplicate)
                {
                    InternalLogger.Instance.LogWarning("This asset is already in the config.");
                    return;
                }

                entry.sourceType = PackageSourceType.LocalUnityPackage;
                entry.path = relativePath;

                EditorUtility.SetDirty(assetsConfig);
                AssetDatabase.SaveAssets();

                InternalLogger.Instance.LogDebug("Located: " + entry.ToString());
            }

            fileExistenceCache.Clear();
        }

        private void AddLocalPackage()
        {
            string selectedPath = EditorUtility.OpenFilePanel("Select UnityPackage", parentPath, "unitypackage");

            if (!string.IsNullOrEmpty(selectedPath) && selectedPath.EndsWith(".unitypackage"))
            {
                string relativePath = IOManager.GetRelativePath(parentPath, selectedPath);

                if (string.IsNullOrEmpty(relativePath))
                {
                    InternalLogger.Instance.LogError("Failed to convert selected path to relative form.");
                    return;
                }

                bool isDuplicate = assetsConfig.assetsEntries.Exists(entry =>
                    entry.sourceType == PackageSourceType.LocalUnityPackage &&
                    entry.path == relativePath);

                if (isDuplicate)
                {
                    InternalLogger.Instance.LogWarning("This asset is already in the config.");
                    return;
                }

                assetsConfig.assetsEntries.Add(new AssetEntry
                {
                    sourceType = PackageSourceType.LocalUnityPackage,
                    path = relativePath
                });

                EditorUtility.SetDirty(assetsConfig);
                AssetDatabase.SaveAssets();

                InternalLogger.Instance.LogDebug("Added: " + relativePath);
            }

            fileExistenceCache.Clear();
        }

        private void AddGitPackage()
        {
            GitUrlInputPopup.Show(url =>
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    InternalLogger.Instance.LogWarning("Git URL is empty.");
                    return;
                }

                if (!url.StartsWith("https://") && !url.StartsWith("git@"))
                {
                    InternalLogger.Instance.LogError("Invalid Git URL.");
                    return;
                }

                bool isDuplicate = assetsConfig.assetsEntries.Exists(entry =>
                    entry.sourceType == PackageSourceType.GitURL &&
                    entry.path == url);

                if (isDuplicate)
                {
                    InternalLogger.Instance.LogWarning("This Git URL is already in the config.");
                    return;
                }

                assetsConfig.assetsEntries.Add(new AssetEntry
                {
                    sourceType = PackageSourceType.GitURL,
                    path = url
                });

                EditorUtility.SetDirty(assetsConfig);
                AssetDatabase.SaveAssets();

                InternalLogger.Instance.LogDebug("Added Git package: " + url);
            });

            fileExistenceCache.Clear();
        }

        private void ImportAllPackages()
        {
            if (assetsConfig.assetsEntries == null || assetsConfig.assetsEntries.Count == 0)
            {
                InternalLogger.Instance.LogWarning("No asset paths configured.");
                return;
            }

            foreach (var entry in assetsConfig.assetsEntries)
            {
                if (entry.sourceType == PackageSourceType.LocalUnityPackage)
                {
                    string fullPath = IOManager.CombinePaths(parentPath, entry.path);
                    bool fileExists = fileExistenceCache.ContainsKey(fullPath)
                        ? fileExistenceCache[fullPath]
                        : IOManager.FileExists(fullPath);

                    if (!fileExists)
                    {
                        InternalLogger.Instance.LogWarning($"Package not found: {fullPath}");
                        continue;
                    }

                    InternalLogger.Instance.LogDebug($"Importing local package: {entry}");
                    AssetDatabase.ImportPackage(fullPath, false);
                }
                else if (entry.sourceType == PackageSourceType.GitURL)
                {
                    InternalLogger.Instance.LogDebug($"Registering Git package: {entry.path}");
                    //AddGitPackageToManifest(entry.path);
                }
                else
                {
                    InternalLogger.Instance.LogWarning($"Unsupported package source: {entry}");
                }
            }

            fileExistenceCache.Clear();
        }

        private (int start, int end, int totalPages) GetPagination(int totalItems, int itemsPerPage, ref int currentPage)
        {
            int totalPages = Mathf.CeilToInt(totalItems / (float)itemsPerPage);
            currentPage = Mathf.Clamp(currentPage, 0, Mathf.Max(0, totalPages - 1));
            int start = currentPage * itemsPerPage;
            int end = Mathf.Min(start + itemsPerPage, totalItems);
            return (start, end, totalPages);
        }
    }
}

#endif
