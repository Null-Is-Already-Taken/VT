#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VT.IO;
using VT.Utils;

namespace VT.Tools.EssentialAssetsImporter
{
    public class EssentialAssetsImporterWindow : EditorWindow
    {
        private string parentPath;
        private AssetsConfig assetsConfig;

        private int currentPage = 0;
        private const int itemsPerPage = 3;
        private const float averageCharWidth = 6f;
        private const float spacing = 10;
        private const string removeButtonText = EmbeddedIcons.Wastebasket_Unicode;
        private const string addButtonText = EmbeddedIcons.PlusSign_Unicode;
        private const float buttonSize = 24;

        private ReorderableList reorderableList;
        
        private Dictionary<string, bool> fileExistenceCache = new();
        private double lastCacheRefreshTime;
        private const double cacheRefreshInterval = 10.0; // in seconds

        [MenuItem("Tools/Essential Assets Importer")]
        public static void OpenWindow()
        {
            var window = GetWindow<EssentialAssetsImporterWindow>("Essential Assets Importer");
            window.Show();
        }

        private void OnEnable()
        {
            parentPath = GetAssetStoreBasePath();
            if (assetsConfig == null) LoadConfig();
        }

        private void OnGUI()
        {
            DrawCachePath();
            GUILayout.Space(spacing);

            if (assetsConfig == null)
            {
                if (GUILayout.Button("Load Config")) LoadConfig();
            }
            else
            {
                DrawConfigObject();
                DrawPackageList();
                //DrawReorderablePackageList();
                DrawImportAssetButton();
            }
        }

        private void DrawCachePath()
        {
            EditorGUILayout.BeginHorizontal("helpBox");
            EditorGUILayout.LabelField("Asset Store Cache Path:", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(parentPath);
            EditorGUI.EndDisabledGroup();
        }

        private void DrawConfigObject()
        {
            EditorGUILayout.BeginHorizontal("helpBox");
            EditorGUILayout.LabelField("Config Loaded:", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(assetsConfig, typeof(AssetsConfig), false);
            EditorGUI.EndDisabledGroup();
        }

        private void InitializePackageList()
        {
            reorderableList = new(assetsConfig.assetsPaths, typeof(AssetsConfig.AssetEntry), true, true, false, false);
            reorderableList.drawHeaderCallback = rect =>
            {
                // Split the header rect: left for label, right for + button
                Rect labelRect = new(rect.x, rect.y, rect.width - buttonSize - spacing, EditorGUIUtility.singleLineHeight);
                Rect buttonRect = new(rect.x + rect.width - buttonSize - spacing, rect.y, buttonSize, EditorGUIUtility.singleLineHeight);

                EditorGUI.LabelField(labelRect, "Configured Package List");
                GUILayout.FlexibleSpace();
                GUIContent addIcon = new(addButtonText, "Add Package");
                if (GUI.Button(buttonRect, addIcon))
                {
                    AddPackage();
                }
                GUILayout.Space(spacing);
            };
            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var entry = assetsConfig.assetsPaths[index];
                string fullText = entry.ToString();
                string truncated = TruncateWithEllipsis(fullText, EstimateMaxChars(rect.width - buttonSize - spacing));

                Rect labelRect = new(rect.x, rect.y + 2, rect.width - buttonSize - spacing * 2, EditorGUIUtility.singleLineHeight);
                GUIContent label = new(truncated, fullText);
                EditorGUI.LabelField(labelRect, label);

                Rect buttonRect = new(rect.xMax - buttonSize - spacing, rect.y + 1, buttonSize, buttonSize);
                GUIContent removeIcon = new(removeButtonText, "Remove Package");
                if (GUI.Button(buttonRect, removeIcon))
                {
                    assetsConfig.assetsPaths.RemoveAt(index);
                    EditorUtility.SetDirty(assetsConfig);
                    AssetDatabase.SaveAssets();
                    reorderableList.list = assetsConfig.assetsPaths; // Refresh
                }
            };
        }

        private void DrawReorderablePackageList()
        {
            if (assetsConfig == null || reorderableList == null)
                return;

            GUILayout.Space(spacing);
            reorderableList.DoLayoutList();
        }

        private void DrawPackageList()
        {
            if (assetsConfig.assetsPaths == null) return;

            double currentTime = EditorApplication.timeSinceStartup;

            bool ShouldRefreshCache() => currentTime - lastCacheRefreshTime > cacheRefreshInterval;

            GUILayout.Space(spacing);
            EditorGUILayout.BeginHorizontal("helpBox");
            EditorGUILayout.LabelField("Configured Package List:", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            
            GUIContent addButton = new(addButtonText, "Add Package");
            var addButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fixedWidth = 18,
                fixedHeight = 18,
                alignment = TextAnchor.MiddleCenter
            };
            StyledButton(addButton, new(.5f, .8f, 1f), AddPackage, addButtonStyle);

            GUILayout.Space(spacing);
            EditorGUILayout.EndHorizontal();

            if (assetsConfig.assetsPaths.Count == 0) return;

            int totalItems = assetsConfig.assetsPaths.Count;
            int totalPages = Mathf.CeilToInt(totalItems / (float)itemsPerPage);
            currentPage = Mathf.Clamp(currentPage, 0, Mathf.Max(totalPages - 1, 0));

            int startIndex = currentPage * itemsPerPage;
            int endIndex = Mathf.Min(startIndex + itemsPerPage, totalItems);

            int? pendingRemoveIndex = null;

            var removeButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fixedWidth = buttonSize,
                fixedHeight = buttonSize,
                alignment = TextAnchor.MiddleCenter
            };

            for (int i = startIndex; i < endIndex; i++)
            {
                var entry = assetsConfig.assetsPaths[i];

                string fullPath = IOManager.CombinePaths(parentPath, entry.relativePath);
                if (ShouldRefreshCache() || !fileExistenceCache.ContainsKey(fullPath))
                {
                    fileExistenceCache[fullPath] = IOManager.FileExists(fullPath);
                    lastCacheRefreshTime = currentTime;
                }
                bool fileExists = fileExistenceCache[fullPath];

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();

                float labelWidth = position.width - buttonSize - spacing * 2;
                string fullText = entry.ToString();
                string truncated = TruncateWithEllipsis(fullText, EstimateMaxChars(labelWidth));
                
                string tooltip = fileExists ? fullText : $"{fullText} (Missing file: {fullPath})";
                GUIContent labelContent = new(truncated, tooltip);
                var entryLabelStyle = new GUIStyle(EditorStyles.label)
                {
                    fixedWidth = labelWidth,
                    normal = { textColor = fileExists ? Color.white : Color.red },
                };
                StyledLabel(labelContent, entryLabelStyle);
                //EditorGUILayout.LabelField(labelContent, GUILayout.Width(labelWidth));

                GUILayout.FlexibleSpace();

                GUIContent removeButton = new(removeButtonText, $"Remove Package");
                StyledButton(removeButton, new(.9f, .3f, .2f), () => pendingRemoveIndex = i, removeButtonStyle);
                
                GUILayout.Space(spacing);

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            if (pendingRemoveIndex.HasValue)
            {
                assetsConfig.assetsPaths.RemoveAt(pendingRemoveIndex.Value);
                fileExistenceCache.Clear();
                EditorUtility.SetDirty(assetsConfig);
                AssetDatabase.SaveAssets();
            }

            DrawPagination(totalPages);
        }

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
            if (assetsConfig != null && assetsConfig.assetsPaths != null && assetsConfig.assetsPaths.Count > 0)
            {
                var importButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 12,
                    fixedHeight = 32,
                    alignment = TextAnchor.MiddleCenter
                };

                StyledButton("Import All Assets", new Color(0.2f, 0.9f, 0.4f), ImportAllPackages, importButtonStyle);
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
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                assetsConfig = AssetDatabase.LoadAssetAtPath<AssetsConfig>(path);
                Debug.Log("Loaded config: " + path);
            }
            else if (guids.Length > 1)
            {
                Debug.LogWarning("Multiple AssetsConfig files found. Please resolve manually.");
                string folder = IOManager.GetDirectoryName(AssetDatabase.GUIDToAssetPath(guids[0]));
                EditorUtility.RevealInFinder(folder);
            }
            else
            {
                string configFolder = ToolPathResolver.GetToolRelativePath("Config Data", "toolmarker-essential-assets-importer.txt");
                string fullFolder = IOManager.GetAbsolutePath(Application.dataPath, configFolder.Replace("Assets/", ""));

                if (!IOManager.DirectoryExists(fullFolder))
                {
                    IOManager.CreateDirectory(fullFolder);
                    AssetDatabase.Refresh();
                }

                string assetPath = IOManager.CombinePaths(configFolder, "DefaultAssetsConfig.asset");
                assetsConfig = CreateInstance<AssetsConfig>();
                AssetDatabase.CreateAsset(assetsConfig, assetPath);
                AssetDatabase.SaveAssets();
                Debug.Log("Created new AssetsConfig at: " + assetPath);
            }

            fileExistenceCache.Clear();
        }

        private void AddPackage()
        {
            string selectedPath = EditorUtility.OpenFilePanel("Select UnityPackage", parentPath, "unitypackage");

            if (!string.IsNullOrEmpty(selectedPath) && selectedPath.EndsWith(".unitypackage"))
            {
                string relativePath = IOManager.GetRelativePath(parentPath, selectedPath);

                if (string.IsNullOrEmpty(relativePath))
                {
                    Debug.LogError("Failed to convert selected path to relative form.");
                    return;
                }

                if (assetsConfig.assetsPaths.Exists(entry => entry.relativePath == relativePath))
                {
                    Debug.LogWarning("This asset is already in the config.");
                    return;
                }

                assetsConfig.assetsPaths.Add(new AssetsConfig.AssetEntry { relativePath = relativePath });
                EditorUtility.SetDirty(assetsConfig);
                AssetDatabase.SaveAssets();
                Debug.Log("Added: " + relativePath);
            }

            fileExistenceCache.Clear();
        }

        private void ImportAllPackages()
        {
            if (assetsConfig.assetsPaths == null || assetsConfig.assetsPaths.Count == 0)
            {
                Debug.LogWarning("No asset paths configured.");
                return;
            }

            foreach (var entry in assetsConfig.assetsPaths)
            {
                string fullPath = IOManager.CombinePaths(parentPath, entry.relativePath);

                if (!IOManager.FileExists(fullPath))
                {
                    Debug.LogWarning($"Package not found: {fullPath}");
                    continue;
                }

                Debug.Log($"Importing package: {entry.relativePath}");
                AssetDatabase.ImportPackage(fullPath, false);
            }
        }

        public static string GetAssetStoreBasePath()
        {
            return Application.platform switch
            {
                RuntimePlatform.WindowsEditor => IOManager.CombinePaths(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                    "Unity", "Asset Store-5.x"),

                RuntimePlatform.OSXEditor => IOManager.CombinePaths(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                    "Library", "Unity", "Asset Store-5.x"),

                _ => throw new System.NotSupportedException("Unsupported platform.")
            };
        }

        private string TruncateWithEllipsis(string text, int maxLength)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : (text.Length <= maxLength ? text : text[..(Mathf.Max(maxLength - 4, 0))] + "...");
        }

        private int EstimateMaxChars(float width) => Mathf.FloorToInt(width / averageCharWidth);

        private void StyledButton(string label, Color backgroundColor, Action onClick, GUIStyle style = null)
        {
            Color previousColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;

            if (style != null)
            {
                if (GUILayout.Button(label, style))
                    onClick?.Invoke();
            }
            else
            {
                if (GUILayout.Button(label))
                    onClick?.Invoke();
            }

            GUI.backgroundColor = previousColor;
        }

        private void StyledButton(GUIContent content, Color backgroundColor, Action onClick, GUIStyle style = null)
        {
            Color previousColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;

            if (style != null)
            {
                if (GUILayout.Button(content, style))
                    onClick?.Invoke();
            }
            else
            {
                if (GUILayout.Button(content))
                    onClick?.Invoke();
            }

            GUI.backgroundColor = previousColor;
        }

        private void StyledLabel(string label, GUIStyle style = null)
        {
            if (style != null)
            {
                EditorGUILayout.LabelField(label, style);
            }
            else
            {
                EditorGUILayout.LabelField(label);
            }
        }

        private void StyledLabel(GUIContent content, GUIStyle style = null)
        {
            if (style != null)
            {
                EditorGUILayout.LabelField(content, style);
            }
            else
            {
                EditorGUILayout.LabelField(content);
            }
        }
    }
}

#endif
