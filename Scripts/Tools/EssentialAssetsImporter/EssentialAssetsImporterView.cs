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

    /// <summary>
    /// Editor window for importing and managing essential asset packages.
    /// </summary>
    public class EssentialAssetsImporterView : EditorWindow
    {
        //--- Serialized Fields ---//

        [SerializeField] private EssentialAssetsImporterModel model;
        [SerializeField] private EssentialAssetsImporterPresenter presenter;

        //--- View Events ---//

        public event Action OnLoadConfigRequested;
        public event Action<int, string> OnLocateRequested;
        public event Action<int> OnRemoveRequested;
        public event Action OnSaveConfigToJSONRequested;
        public event Action<string> OnLoadConfigFromJSONRequested;
        public event Action<string> OnAddLocalRequested;
        public event Action<string> OnAddGitRequested;
        public event Action OnImportAllRequested;
        public event Action OnRefreshRequested;
        public event Action<int> OnPageChanged;
        public event Action<int> OnSelectConfigRequested;

        //--- State ---//

        private List<AssetsConfig> configList;
        private int currentConfigIndex;
        private AssetsConfig config;

        private List<AssetEntryViewModel> currentEntries;
        private int currentPage;
        private int totalPages;

        private Vector2 scrollPos;
        private bool addingGitUrl;
        private string newGitUrl = string.Empty;

        //--- Constants ---//

        private const int itemsPerPage = 5;
        public float ItemPerPage => itemsPerPage;

        private const float padding = 3f;
        private const float spacing = 10f;
        private const float averageCharWidth = 6f;
        private const float buttonSize = 24f;

        // Unicode icons
        private const string removeButtonIcon = EmbeddedIcons.Wastebasket_Unicode;
        private const string addLocalButtonIcon = EmbeddedIcons.Package_Unicode;
        private const string addGlobalButtonIcon = EmbeddedIcons.Internet_Unicode;
        private const string locateButtonIcon = EmbeddedIcons.MagnifyingGlass_Unicode;
        private const string saveButtonIcon = EmbeddedIcons.Save_Unicode;
        private const string loadButtonIcon = EmbeddedIcons.Load_Unicode;

        private double lastAutoRefreshTime;
        private const double autoRefreshInterval = 15.0;

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

        //--- Menu ---//

        [MenuItem("Tools/VT/Essential Assets Importer")]
        public static void OpenWindow()
        {
            GetWindow<EssentialAssetsImporterView>("Essential Assets Importer").Show();
        }

        //--- Lifecycle ---//

        private void OnEnable()
        {
            // Ensure model & presenter survive domain reloads
            model ??= new EssentialAssetsImporterModel();
            presenter ??= new EssentialAssetsImporterPresenter(this, model);

            // Start auto-refresh timer
            lastAutoRefreshTime = EditorApplication.timeSinceStartup;
            EditorApplication.update += AutoRefresh;
        }

        private void OnDisable()
        {
            // Unsubscribe update
            EditorApplication.update -= AutoRefresh;
            presenter?.DisposeIfNeeded();
        }

        //--- GUI Rendering ---//

        private void OnGUI()
        {
            // Scrollable content
            using var sv = new EditorGUILayout.ScrollViewScope(scrollPos);
            scrollPos = sv.scrollPosition;

            DrawConfigHeader();
            DrawEntries();
            DrawActionButtons();
        }

        /// <summary>
        /// Renders the config profiles header with profile selector.
        /// </summary>
        private void DrawConfigHeader()
        {
            using (new EditorGUILayout.VerticalScope("helpBox"))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    Label.Draw("Config Profiles", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    PageNavigator.Draw(
                        currentIndex: currentConfigIndex,
                        total: configList?.Count ?? 0,
                        onPageChanged: OnSelectConfigRequested,
                        label: "Profile"
                    );
                }

                GUILayout.Space(spacing);

                using (new EditorGUILayout.HorizontalScope())
                {
                    // Display current AssetsConfig asset
                    using (new EditorGUI.DisabledGroupScope(true))
                    {
                        EditorGUILayout.ObjectField(config, typeof(AssetsConfig), false, GUILayout.ExpandWidth(expand: true));
                    }

                    // if (config.assetsEntries.Count > 0)
                    // {
                    //     Button.Draw(
                    //         content: new GUIContent(saveButtonIcon, "Save config to JSON"),
                    //         backgroundColor: Color.white,
                    //         onClick: () => { OnSaveConfigToJSONRequested?.Invoke(); },
                    //         style: ButtonStyles.Inline
                    //     );

                    //     Button.Draw(
                    //         content: new GUIContent(loadButtonIcon, "Load config JSON"),
                    //         backgroundColor: Color.white,
                    //         onClick: () => {
                    //             string absolutePath = EditorUtility.OpenFilePanel(
                    //                 "Select UnityPackage",
                    //                 PathUtils.GetAssetStorePath(),
                    //                 "unitypackage"
                    //             );
                    //             OnLoadConfigFromJSONRequested?.Invoke(absolutePath);
                    //         },
                    //         style: ButtonStyles.Inline
                    //     );
                    // }
                }
            }
        }

        /// <summary>
        /// Renders the list of asset entries with pagination.
        /// </summary>
        private void DrawEntries()
        {
            DrawHeaderBar();

            if (currentEntries == null || currentEntries.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    message: "No assets configured. Please add assets first.",
                    type: MessageType.Warning
                );
            }
            else
            {
                // Render up to itemsPerPage entries
                for (int i = 0; i < itemsPerPage; i++)
                {
                    if (i < currentEntries.Count)
                        DrawPackageEntry(i, currentEntries[i]);
                    else
                        DrawPlaceholderEntry();
                }
                DrawPagination();
            }
        }

        /// <summary>
        /// Renders the top toolbar for adding local or Git entries.
        /// </summary>
        private void DrawHeaderBar()
        {
            GUILayout.Space(spacing);
            using (new EditorGUILayout.VerticalScope("helpBox"))
            using (new EditorGUILayout.HorizontalScope())
            {
                Label.Draw("Configured Package List", LabelStyles.BoldLabel);
                GUILayout.FlexibleSpace();

                // Add local
                Button.Draw(
                    content: new GUIContent(addLocalButtonIcon, "Add local package"),
                    backgroundColor: Color.white,
                    onClick: () => {
                        string absolutePath = EditorUtility.OpenFilePanel(
                            "Select UnityPackage",
                            PathUtils.GetAssetStorePath(),
                            "unitypackage"
                        );
                        OnAddLocalRequested?.Invoke(absolutePath);
                    },
                    style: ButtonStyles.Inline
                );

                // Add Git
                Button.Draw(
                    content: new GUIContent(addGlobalButtonIcon, "Add git package"),
                    backgroundColor: Color.white,
                    onClick: () => addingGitUrl = true,
                    style: ButtonStyles.Inline
                );
            }

            if (addingGitUrl)
                DrawGitUrlSection();
        }

        /// <summary>
        /// Renders the Git URL input section.
        /// </summary>
        private void DrawGitUrlSection()
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField("Enter Git URL:", EditorStyles.boldLabel);
                newGitUrl = EditorGUILayout.TextField(newGitUrl);
                using (new EditorGUILayout.HorizontalScope())
                {
                    Button.Draw(
                        content: new GUIContent("Add"),
                        backgroundColor: Color.white,
                        onClick: () => {
                            OnAddGitRequested?.Invoke(newGitUrl);
                            addingGitUrl = false;
                            newGitUrl = string.Empty;
                        },
                        style: ButtonStyles.Compact
                    );
                    Button.Draw(
                        content: new GUIContent("Cancel"),
                        backgroundColor: Color.white,
                        onClick: () => {
                            addingGitUrl = false;
                            newGitUrl = string.Empty;
                        },
                        style: ButtonStyles.Compact
                    );
                }
            }
        }

        /// <summary>
        /// Renders a single package entry row.
        /// </summary>
        private void DrawPackageEntry(int index, AssetEntryViewModel vm)
        {
            bool exists = vm.Exists;
            int buttonCount = exists ? 1 : 2;
            float labelWidth = position.width
                                - (buttonCount * buttonSize)
                                - (spacing * buttonCount);

            using (new EditorGUILayout.VerticalScope("box"))
            using (new EditorGUILayout.HorizontalScope())
            {
                Label.DrawTruncatedLabel(
                    fullText: vm.Entry.ToString(),
                    textColor: exists ? Color.white : Color.red,
                    tooltip: exists ? vm.Entry.relativePath : $"Missing file: {vm.Entry.relativePath}",
                    availableWidth: labelWidth,
                    averageCharWidth: averageCharWidth,
                    options: GUILayout.ExpandWidth(expand: true)
                );

                if (!exists)
                    Button.Draw(
                        content: new GUIContent(locateButtonIcon, "Locate missing package"),
                        backgroundColor: Color.white,
                        onClick: () =>
                        {
                            string absolutePath = EditorUtility.OpenFilePanel(
                                "Locate UnityPackage",
                                PathUtils.GetAssetStorePath(),
                                "unitypackage"
                            );
                            OnLocateRequested?.Invoke(currentPage * itemsPerPage + index, absolutePath);
                        },
                        style: ButtonStyles.Inline
                    );

                Button.Draw(
                    content: new GUIContent(removeButtonIcon, "Remove package"),
                    backgroundColor: Color.white,
                    onClick: () => OnRemoveRequested?.Invoke(currentPage * itemsPerPage + index),
                    style: ButtonStyles.Inline
                );
            }
        }

        /// <summary>
        /// Placeholder row to maintain consistent layout.
        /// </summary>
        private void DrawPlaceholderEntry()
        {
            using (new EditorGUILayout.VerticalScope("box", GUILayout.Height(buttonSize + padding * 2)))
            using (new EditorGUILayout.HorizontalScope())
            {
                Label.Draw(string.Empty);
            }
        }

        /// <summary>
        /// Renders pagination controls for entries.
        /// </summary>
        private void DrawPagination()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                PageNavigator.Draw(
                    currentIndex: currentPage,
                    total: totalPages,
                    onPageChanged: OnPageChanged,
                    label: "Page"
                );
            }
        }

        /// <summary>
        /// Renders the Import and Refresh buttons.
        /// </summary>
        private void DrawActionButtons()
        {
            if (currentEntries?.Count > 0)
            {
                Button.Draw(
                    content: new GUIContent("Import", "Import all configured packages"),
                    backgroundColor: new Color(0.5f, 1f, 0.5f),
                    onClick: () => OnImportAllRequested?.Invoke(),
                    style: null,
                    GUILayout.Height(32)
                );
            }
        }

        /// <summary>
        /// Automatically refreshes when the window is focused and interval elapsed.
        /// </summary>
        private void AutoRefresh()
        {
            if (focusedWindow != this)
                return;

            double now = EditorApplication.timeSinceStartup;
            if (now - lastAutoRefreshTime >= autoRefreshInterval)
            {
                lastAutoRefreshTime = now;
                OnRefreshRequested?.Invoke();
            }
        }
    }
}
#endif
