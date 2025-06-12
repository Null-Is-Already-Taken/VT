#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VT.EditorUtils;

namespace VT.Tools.EssentialAssetsImporter
{
    public class AssetEntryViewModel
    {
        public AssetEntry Entry { get; set; }
        public bool Exists { get; set; }
    }

    // Views/IEssentialAssetsImporterView.cs
    public interface IEssentialAssetsImporterView
    {
        event Action OnLoadConfigRequested;
        event Action<int> OnLocateRequested;
        event Action<int> OnRemoveRequested;
        event Action OnAddLocalRequested;
        event Action OnAddGitRequested;
        event Action OnImportAllRequested;
        event Action OnRefreshRequested;
        event Action<int> OnPageChanged;
        void UpdateConfigAsset(AssetsConfig config);
        void UpdateEntriesViewModels(List<AssetEntryViewModel> entries);
        void ShowMissingFileWarning(AssetEntryViewModel vm);
        void UpdatePageInfo(int currentPage, int totalPages);
        void Refresh();
    }

    // Views/EssentialAssetsImporterWindow.cs
    public class EssentialAssetsImporterWindow : EditorWindow, IEssentialAssetsImporterView
    {
        // Implement view interface, raise events in OnGUI, call DisplayEntries
        public event Action OnLoadConfigRequested;
        public event Action<int> OnLocateRequested;
        public event Action<int> OnRemoveRequested;
        public event Action OnAddLocalRequested;
        public event Action OnAddGitRequested;
        public event Action OnImportAllRequested;
        public event Action OnRefreshRequested;
        public event Action<int> OnPageChanged;

        public void UpdateConfigAsset(AssetsConfig config)
        {
            this.config = config;
        }

        public void UpdateEntriesViewModels(List<AssetEntryViewModel> entries)
        {
            if (entries == null)
            {
                return;
            }

            currentEntries = entries;
        }

        public void ShowMissingFileWarning(AssetEntryViewModel vm)
        {
            throw new NotImplementedException();
        }

        public void UpdatePageInfo(int currentPage, int totalPages)
        {
            this.currentPage = currentPage;
            this.totalPages = totalPages;
        }

        public void Refresh()
        {
            OnRefreshRequested?.Invoke();
        }

        [MenuItem("Tools/Essential Assets Importer")]
        public static void OpenWindow()
        {
            var window = GetWindow<EssentialAssetsImporterWindow>("Essential Assets Importer");
            var model = new AssetsConfigModel();
            var presenter = new EssentialAssetsImporterPresenter(window, model);
            window.Show();
        }

        // backing store for the currently displayed entries
        private List<AssetEntryViewModel> currentEntries;

        // pagination state
        private int currentPage = 0;
        private int totalPages = 0;
        private const int itemsPerPage = 3;

        // layout constants
        private const float padding = 3f;
        private const float spacing = 10f;
        private const float averageCharWidth = 6f;
        private const float buttonSize = 24f;

        // icon keys
        private const string removeButtonText = EmbeddedIcons.Wastebasket_Unicode;
        private const string addLocalButtonText = EmbeddedIcons.Package_Unicode;
        private const string addGlobalButtonText = EmbeddedIcons.Internet_Unicode;
        private const string loadButtonText = EmbeddedIcons.FileFolder_Unicode;
        private const string locateButtonText = EmbeddedIcons.LeftPointingMagnifyingGlass_Unicode;

        // the currently loaded config
        private AssetsConfig config;

        private void OnEnable()
        {
            OnLoadConfigRequested?.Invoke();
        }

        private void OnGUI()
        {
            DrawConfigHeader();
            DrawEntries();
            DrawImportButton();
            DrawRefreshButton();
        }

        private void DrawConfigHeader()
        {
            EditorGUILayout.BeginHorizontal("helpBox");
            EditorGUILayout.LabelField("Config Profile", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (config == null)
            {
                GUIContent loadConfigButton = new(loadButtonText, "Load Config");
                Backend.StyledButton
                (
                    loadConfigButton,
                    Color.white,
                    () => OnLoadConfigRequested?.Invoke(),
                    Backend.InlineButtonStyle
                );
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.ObjectField(config, typeof(AssetsConfig), false);
        }

        private void DrawEntries()
        {
            DrawHeaderBar();

            if (currentEntries == null || currentEntries.Count == 0)
                return;

            for (int i = 0; i < itemsPerPage; i++)
            {
                if (i < currentEntries.Count)
                    DrawPackageEntry(i, currentEntries[i]);
                else
                    DrawDummyEntry();
            }

            DrawPagination();
        }

        private void DrawHeaderBar()
        {
            GUILayout.Space(spacing);
            EditorGUILayout.BeginHorizontal("helpBox");
            EditorGUILayout.LabelField("Configured Package List", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            GUIContent addLocalButton = new(addLocalButtonText, "Add local package");
            Backend.StyledButton
            (
                addLocalButton,
                Color.white,
                () =>
                {
                    OnAddLocalRequested?.Invoke();
                    Refresh();
                },
                Backend.InlineButtonStyle
            );
            GUIContent addGlobalButton = new(addGlobalButtonText, "Add git package");
            Backend.StyledButton
            (
                addGlobalButton,
                Color.white,
                () =>
                {
                    OnAddGitRequested?.Invoke();
                    Refresh();
                },
                Backend.InlineButtonStyle
            );
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPackageEntry(int index, AssetEntryViewModel vm)
        {
            // 1) Use the presenter‐computed flag
            bool exists = vm.Exists;

            // 2) Compute label width (buttons shown based on exists)
            int buttonCount = exists ? 1 : 2;
            float labelWidth = position.width
                               - (buttonCount * buttonSize)
                               - (spacing * buttonCount);

            // 3) Prepare text & tooltip
            string fullText = vm.Entry.ToString();
            string truncated = Backend.TruncateWithEllipsis(
                fullText,
                Backend.EstimateMaxChars(labelWidth, averageCharWidth)
            );
            string tooltip = exists
                               ? fullText
                               : $"Missing file: {vm.Entry.path}";

            var labelContent = new GUIContent(truncated, tooltip);
            var labelStyle = new GUIStyle(EditorStyles.label)
            {
                fixedWidth = labelWidth,
                normal = { textColor = exists ? Color.white : Color.red }
            };

            // 4) Render
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();

            Backend.StyledLabel(labelContent, labelStyle);
            GUILayout.FlexibleSpace();

            if (!exists)
            {
                GUIContent locateLocalButton = new(locateButtonText, "Locate missing package");
                Backend.StyledButton
                (
                    locateLocalButton,
                    Color.white,
                    () =>
                    {
                        OnLocateRequested?.Invoke(currentPage * itemsPerPage + index);
                        Refresh();
                    },
                    Backend.InlineButtonStyle
                );
            }

            GUIContent removeLocalButton = new(removeButtonText, "Remove package");
            Backend.StyledButton
            (
                removeLocalButton,
                Color.white,
                () =>
                {
                    OnRemoveRequested?.Invoke(currentPage * itemsPerPage + index);
                    Refresh();
                },
                Backend.InlineButtonStyle
            );
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawDummyEntry()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Height(buttonSize + padding * 2));
            EditorGUILayout.BeginHorizontal();
            Backend.StyledLabel("");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawPagination()
        {
            EditorGUILayout.BeginHorizontal();

            GUI.enabled = currentPage > 0;
            if (GUILayout.Button("← Prev", GUILayout.Width(70)))
                OnPageChanged?.Invoke(currentPage - 1);

            GUI.enabled = currentPage < totalPages - 1;
            if (GUILayout.Button("Next →", GUILayout.Width(70)))
                OnPageChanged?.Invoke(currentPage + 1);

            GUI.enabled = true;

            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField($"Page {currentPage + 1} of {totalPages}", GUILayout.Width(120));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawImportButton()
        {
            GUILayout.Space(spacing);
            if (currentEntries != null && currentEntries.Count > 0)
            {
                if (GUILayout.Button("Import All Assets", GUILayout.Height(32)))
                {
                    OnImportAllRequested?.Invoke();
                    Refresh();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No assets configured. Please add assets first.", MessageType.Warning);
            }
        }

        private void DrawRefreshButton()
        {
            GUILayout.Space(spacing);

            if (GUILayout.Button("Refresh (Debug)", GUILayout.Height(32)))
                Refresh();
        }
    }
}
#endif
