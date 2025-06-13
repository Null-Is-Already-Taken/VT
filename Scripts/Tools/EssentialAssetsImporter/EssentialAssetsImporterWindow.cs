#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VT.Editor.GUI;
using VT.Editor.Utils;

namespace VT.Tools.EssentialAssetsImporter
{
    public class AssetEntryViewModel
    {
        public AssetEntry Entry { get; set; }
        public bool Exists { get; set; }
    }

    //public interface IEssentialAssetsImporterView
    //{
    //    event Action OnLoadConfigRequested;
    //    event Action<int> OnLocateRequested;
    //    event Action<int> OnRemoveRequested;
    //    event Action OnAddLocalRequested;
    //    event Action<string> OnAddGitRequested;
    //    event Action OnImportAllRequested;
    //    event Action OnRefreshRequested;
    //    event Action<int> OnPageChanged;
    //    event Action<int> OnSelectConfigRequested;

    //    void UpdateConfigList(List<AssetsConfig> configList);
    //    void UpdateConfigPageInfo(int selectedIndex, int totalConfigs);
    //    void UpdateConfigAsset(AssetsConfig config);
    //    void UpdateEntriesViewModels(List<AssetEntryViewModel> entries);
    //    void UpdatePageInfo(int currentPage, int totalPages);
    //}

    public class EssentialAssetsImporterWindow : EditorWindow//, IEssentialAssetsImporterView
    {
        // Serialize so they survive domain reload
        [SerializeField] private AssetsConfigModel model;
        [SerializeField] private EssentialAssetsImporterPresenter presenter;

        // Implement view interface events
        public event Action OnLoadConfigRequested;
        public event Action<int> OnLocateRequested;
        public event Action<int> OnRemoveRequested;
        public event Action OnAddLocalRequested;
        public event Action<string> OnAddGitRequested;
        public event Action OnImportAllRequested;
        public event Action OnRefreshRequested;
        public event Action<int> OnPageChanged;
        public event Action<int> OnSelectConfigRequested;

        public void UpdateConfigList(List<AssetsConfig> configList)
        {
            this.configList = configList;
        }

        public void UpdateConfigPageInfo(int selectedIndex, int totalConfigs)
        {
            currentConfigIndex = selectedIndex;
        }

        public void UpdateConfigAsset(AssetsConfig config)
        {
            this.config = config;
        }

        public void UpdateEntriesViewModels(List<AssetEntryViewModel> entries)
        {
            if (entries == null) return;

            currentEntries = entries;
        }

        public void UpdatePageInfo(int currentPage, int totalPages)
        {
            this.currentPage = currentPage;
            this.totalPages = totalPages;
        }

        [MenuItem("Tools/Essential Assets Importer")]
        public static void OpenWindow()
        {
            // This just shows the window; wiring happens in OnEnable
            GetWindow<EssentialAssetsImporterWindow>("Essential Assets Importer").Show();
        }

        // list of available config profiles
        private List<AssetsConfig> configList;

        // index of the currently selected config profile
        private int currentConfigIndex = 0;

        // backing store for the currently displayed entries
        private List<AssetEntryViewModel> currentEntries;

        // pagination state
        private int currentPage = 0;
        private int totalPages = 0;
        private const int itemsPerPage = 5;

        // layout constants
        private const float padding = 3f;
        private const float spacing = 10f;
        private const float averageCharWidth = 6f;
        private const float buttonSize = 24f;

        // icon keys
        private const string removeButtonIcon = EmbeddedIcons.Wastebasket_Unicode;
        private const string addLocalButtonIcon = EmbeddedIcons.Package_Unicode;
        private const string addGlobalButtonIcon = EmbeddedIcons.Internet_Unicode;
        private const string locateButtonIcon = EmbeddedIcons.LeftPointingMagnifyingGlass_Unicode;

        // the currently loaded config
        private AssetsConfig config;

        // scroll view state
        private Vector2 scrollPos;
        
        private bool addingGitUrl = false;
        private string newGitUrl = "";


        private void OnEnable()
        {
            model ??= new AssetsConfigModel();

            // Always re-create the presenter and hook its handlers to your view
            presenter ??= new EssentialAssetsImporterPresenter(this, model);
        }

        private void OnDisable()
        {
            // If your presenter holds unmanaged resources or subscriptions elsewhere, clean up here
            presenter.DisposeIfNeeded();
        }

        private void OnGUI()
        {
            using var sv = new EditorGUILayout.ScrollViewScope(scrollPos);
            scrollPos = sv.scrollPosition;

            DrawConfigHeader();
            DrawEntries();
            DrawImportButton();
            DrawRefreshButton();
        }

        private void DrawConfigHeader()
        {
            using (new EditorGUILayout.HorizontalScope("helpBox"))
            {
                Label.Draw("Config Profiles", EditorStyles.boldLabel);

                GUILayout.FlexibleSpace();
                
                PageNavigator.Draw(currentConfigIndex, configList.Count, OnSelectConfigRequested, "Profile");
            }

            using (new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUILayout.ObjectField(config, typeof(AssetsConfig), false);
            }
        }

        private void DrawEntries()
        {
            DrawHeaderBar();

            if (currentEntries == null || currentEntries.Count == 0)
            { 
                EditorGUILayout.HelpBox("No assets configured. Please add assets first.", MessageType.Warning);
                return;
            }

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

            using (new EditorGUILayout.VerticalScope("helpBox"))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    Label.Draw("Configured Package List", LabelStyles.BoldLabel);

                    GUILayout.FlexibleSpace();

                    Button.Draw
                    (
                        content: new(addLocalButtonIcon, "Add local package"),
                        backgroundColor: Color.white,
                        onClick: () => OnAddLocalRequested?.Invoke(),
                        style: ButtonStyles.Inline
                    );

                    Button.Draw
                    (
                        content: new(addGlobalButtonIcon, "Add git package"),
                        backgroundColor: Color.white,
                        onClick: () =>
                        {
                            addingGitUrl = true; // Toggle visibility
                        },
                        style: ButtonStyles.Inline
                    );
                }

                if (addingGitUrl)
                {
                    DrawGitUrlFieldSection();
                }
            }
        }

        private void DrawGitUrlFieldSection()
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                GUILayout.Label("Enter Git URL:", EditorStyles.boldLabel);

                newGitUrl = EditorGUILayout.TextField(newGitUrl);

                using (new EditorGUILayout.HorizontalScope())
                {
                    Button.Draw
                    (
                        content: new GUIContent("Add"),
                        backgroundColor: Color.white,
                        onClick: () =>
                        {
                            OnAddGitRequested?.Invoke(newGitUrl);
                            addingGitUrl = false;
                            newGitUrl = "";
                        },
                        style: ButtonStyles.Compact
                    );

                    Button.Draw
                    (
                        content: new GUIContent("Cancel"),
                        backgroundColor: Color.white,
                        onClick: () =>
                        {
                            addingGitUrl = false;
                            newGitUrl = "";
                        },
                        style: ButtonStyles.Compact
                    );
                }
            }
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

            // 3) Render
            using (new EditorGUILayout.VerticalScope("box"))
            using (new EditorGUILayout.HorizontalScope())
            {
                Label.DrawTruncatedLabel(
                    fullText: vm.Entry.ToString(),
                    tooltip: exists ? vm.Entry.path : $"Missing file: {vm.Entry.path}",
                    availableWidth: labelWidth,
                    averageCharWidth: averageCharWidth,
                    exists: exists
                );

                GUILayout.FlexibleSpace();

                if (!exists)
                {
                    Button.Draw
                    (
                        content: new(locateButtonIcon, "Locate missing package"),
                        backgroundColor: Color.white,
                        onClick: () => OnLocateRequested?.Invoke(currentPage * itemsPerPage + index),
                        style: ButtonStyles.Inline
                    );
                }

                Button.Draw
                (
                    content: new(removeButtonIcon, "Remove package"),
                    backgroundColor: Color.white,
                    onClick: () => OnRemoveRequested?.Invoke(currentPage * itemsPerPage + index),
                    style: ButtonStyles.Inline
                );
            }
        }

        private void DrawDummyEntry()
        {
            using (new EditorGUILayout.VerticalScope("box", GUILayout.Height(buttonSize + padding * 2)))
            using (new EditorGUILayout.HorizontalScope())
            {
                Label.Draw("");
            }
        }

        private void DrawPagination()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                PageNavigator.Draw(currentPage, totalPages, OnPageChanged, "Page");
            }
        }

        private void DrawImportButton()
        {
            if (currentEntries != null && currentEntries.Count > 0)
            {
                Button.Draw
                (
                    content: new GUIContent("Import", "Import all configured assets"),
                    backgroundColor: new Color(0.3f, 0.8f, 0.5f),
                    onClick: () => OnImportAllRequested?.Invoke(),
                    style: null,
                    GUILayout.Height(32)
                );
            }
        }

        private void DrawRefreshButton()
        {
            Button.Draw
            (
                content: new GUIContent("Refresh (Debug)", "Reload the current config and entries"),
                backgroundColor: Color.white,
                onClick: () => OnRefreshRequested?.Invoke(),
                style: null,
                GUILayout.Height(32)
            );
        }
    }
}
#endif
