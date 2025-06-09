#if UNITY_EDITOR

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using VT.IO;

namespace VT.Tools.EssentialAssetsImporter
{
    public class EssentialAssetsImporterWindow : OdinEditorWindow
    {
        [ReadOnly]
        [ShowInInspector]
        private string parentPath = GetAssetStoreBasePath();

        [ShowInInspector]
        [InlineEditor]
        private AssetsConfig assetsConfig;

        [Sirenix.OdinInspector.FilePath(Extensions = "unitypackage", RequireExistingPath = true, ParentFolder = "$parentPath")]
        [LabelText("Select UnityPackage")]
        [ShowInInspector]
        private string selectedRelativePath;

        [MenuItem("Tools/Essential Assets Importer")]
        public static void OpenWindow()
        {
            GetWindow<EssentialAssetsImporterWindow>().Show();
        }

        protected override void OnEnable()
        {
            parentPath = GetAssetStoreBasePath();
        }

        [Button("Load Config")]
        [HideIf("$assetsConfig")]
        private void LoadConfig()
        {
            string selectedPath = EditorUtility.OpenFilePanel("Select AssetsConfig", "Assets", "asset");
            if (string.IsNullOrEmpty(selectedPath))
                return;

            // Get base folder of this script
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            string baseFolder = IOManager.GetDirectoryName(scriptPath);
            string configFolder = IOManager.CombinePaths(baseFolder, "Config Data");

            // Convert to project-relative path
            if (selectedPath.StartsWith(Application.dataPath))
            {
                string unityRelativePath = "Assets" + selectedPath.Substring(Application.dataPath.Length);

                // Ensure inside Config Data
                if (!unityRelativePath.Replace("\\", "/").StartsWith(configFolder.Replace("\\", "/")))
                {
                    Debug.LogError("Selected file must be inside the 'Config Data' folder next to this script.");
                    return;
                }

                assetsConfig = AssetDatabase.LoadAssetAtPath<AssetsConfig>(unityRelativePath);
                Debug.Log("Loaded config: " + unityRelativePath);
            }
            else
            {
                Debug.LogError("Selected path is not inside the project.");
            }
        }

        [Button("Add Selected Package To Config")]
        private void AddSelected()
        {
            if (assetsConfig == null)
            {
                Debug.LogWarning("No config loaded.");
                return;
            }

            var fullPath = IOManager.CombinePaths(parentPath, selectedRelativePath);
            
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
            selectedRelativePath = string.Empty; // Clear selection after adding
        }

        [Button("Import Assets")]
        [EnableIf("$assetsConfig")]
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
                AssetDatabase.ImportPackage(fullPath, false); // false = no import dialog
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
