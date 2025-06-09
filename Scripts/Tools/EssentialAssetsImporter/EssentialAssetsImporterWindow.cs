#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using VT.IO;

namespace VT.Tools.EssentialAssetsImporter
{
    public class EssentialAssetsImporterWindow : EditorWindow
    {
        private string parentPath;
        private AssetsConfig assetsConfig;
        private string selectedRelativePath;

        private int currentPage = 0;
        private const int itemsPerPage = 2;


        [MenuItem("Tools/Essential Assets Importer")]
        public static void OpenWindow()
        {
            var window = GetWindow<EssentialAssetsImporterWindow>("Essential Assets Importer");
            window.Show();
        }

        private void OnEnable()
        {
            parentPath = GetAssetStoreBasePath();
            assetsConfig = null;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Asset Store Cache Path:", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(parentPath);
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);

            if (assetsConfig == null)
            {
                if (GUILayout.Button("Load Config"))
                {
                    LoadConfig();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Config Loaded:", EditorStyles.boldLabel);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(assetsConfig, typeof(AssetsConfig), false);
                EditorGUI.EndDisabledGroup();

                // Draw paginated list
                if (assetsConfig.assetsPaths != null && assetsConfig.assetsPaths.Count > 0)
                {
                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("Configured UnityPackages:", EditorStyles.boldLabel);

                    int totalItems = assetsConfig.assetsPaths.Count;
                    int totalPages = Mathf.CeilToInt(totalItems / (float)itemsPerPage);
                    currentPage = Mathf.Clamp(currentPage, 0, Mathf.Max(totalPages - 1, 0));

                    int startIndex = currentPage * itemsPerPage;
                    int endIndex = Mathf.Min(startIndex + itemsPerPage, totalItems);

                    EditorGUI.indentLevel++;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        EditorGUILayout.LabelField($"• {assetsConfig.assetsPaths[i].relativePath}");
                    }
                    EditorGUI.indentLevel--;

                    GUILayout.Space(5);

                    // Page controls
                    EditorGUILayout.BeginHorizontal();
                    GUI.enabled = currentPage > 0;
                    if (GUILayout.Button("← Prev", GUILayout.Width(70)))
                        currentPage--;
                    GUI.enabled = currentPage < totalPages - 1;
                    if (GUILayout.Button("Next →", GUILayout.Width(70)))
                        currentPage++;
                    GUI.enabled = true;

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField($"Page {currentPage + 1} of {totalPages}", GUILayout.Width(120));
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(10);

                EditorGUILayout.LabelField("Select UnityPackage (relative to above path):", EditorStyles.boldLabel);
                selectedRelativePath = EditorGUILayout.TextField(selectedRelativePath);

                if (GUILayout.Button("Add Selected Package To Config"))
                {
                    AddSelected();
                }

                GUILayout.Space(10);

                if (GUILayout.Button("Import Assets"))
                {
                    ImportAllPackages();
                }
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
                // None found, offer to create one
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
        }


        private void AddSelected()
        {
            if (assetsConfig == null)
            {
                Debug.LogWarning("No config loaded.");
                return;
            }

            string fullPath = IOManager.CombinePaths(parentPath, selectedRelativePath);
            if (string.IsNullOrEmpty(fullPath) || !IOManager.FileExists(fullPath))
            {
                Debug.LogWarning($"Selected path [{fullPath}] is invalid.");
                return;
            }

            if (assetsConfig.assetsPaths.Exists(entry => entry.relativePath == selectedRelativePath))
            {
                Debug.LogWarning("This asset is already in the config.");
                return;
            }

            assetsConfig.assetsPaths.Add(new AssetsConfig.AssetEntry { relativePath = selectedRelativePath });
            EditorUtility.SetDirty(assetsConfig);
            AssetDatabase.SaveAssets();

            Debug.Log("Added: " + selectedRelativePath);
            selectedRelativePath = string.Empty;
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
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return IOManager.CombinePaths(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                    "Unity",
                    "Asset Store-5.x"
                );
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return IOManager.CombinePaths(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                    "Library",
                    "Unity",
                    "Asset Store-5.x"
                );
            }

            Debug.LogError("Unsupported platform.");
            return null;
        }
    }
}

#endif
