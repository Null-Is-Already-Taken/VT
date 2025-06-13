using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VT.IO;
using VT.Logger;

namespace VT.Tools.EssentialAssetsImporter
{
    // Presenters/EssentialAssetsImporterPresenter.cs
    public class EssentialAssetsImporterPresenter : IDisposable
    {
        public EssentialAssetsImporterPresenter(IEssentialAssetsImporterView view, IAssetsConfigModel model)
        {
            this.view = view;
            this.model = model;

            view.OnLoadConfigRequested += Refresh;
            view.OnAddLocalRequested += HandleAddLocal;
            view.OnAddGitRequested += HandleAddGit;
            view.OnLocateRequested += HandleLocate;
            view.OnRemoveRequested += HandleRemove;
            view.OnImportAllRequested += HandleImport;
            view.OnRefreshRequested += HandleRefreshRequested;
            view.OnPageChanged += HandlePageChange;
            view.OnSelectConfigRequested += HandleSelectConfig;

            Init();   // first load init
        }

        /// <summary>
        /// Unsubscribe all view events and mark disposed.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            view.OnLoadConfigRequested -= Refresh;
            view.OnAddLocalRequested -= HandleAddLocal;
            view.OnAddGitRequested -= HandleAddGit;
            view.OnLocateRequested -= HandleLocate;
            view.OnRemoveRequested -= HandleRemove;
            view.OnImportAllRequested -= HandleImport;
            view.OnRefreshRequested -= HandleRefreshRequested;
            view.OnPageChanged -= HandlePageChange;
            view.OnSelectConfigRequested -= HandleSelectConfig;

            isDisposed = true;
        }

        /// <summary>
        /// Call this from your EditorWindow.OnDisable()
        /// </summary>
        public void DisposeIfNeeded()
        {
            Dispose();
        }

        private bool hasInit = false;
        private readonly IEssentialAssetsImporterView view;
        private readonly IAssetsConfigModel model;
        private List<AssetsConfig> configs;
        private int currentConfigIndex;
        private int currentPage = 0;
        private int totalPages = 0;
        private const int pageSize = 3;
        private bool isDisposed = false;

        private void Init()
        {
            // Only the first time: scan/possibly-create the config asset(s)
            if (!hasInit)
            {
                model.LoadConfig();            // create default if none
                RefreshAllConfigs();           // populate configs[] and select first
                hasInit = true;
            }

            // Always do a normal refresh of the *current* config’s entries
            Refresh();
        }

        private void RefreshAllConfigs()
        {
            configs = model
                .GetAllConfigs()
                .ToList();

            // auto-select first if nothing selected
            currentConfigIndex = Mathf.Clamp(currentConfigIndex, 0, configs.Count - 1);
            if (configs.Count > 0)
                model.SelectConfig(configs[currentConfigIndex]);

            view.UpdateConfigList(configs);
            view.UpdateConfigPageInfo(currentConfigIndex, configs.Count);
        }

        private void Refresh()
        {
            InternalLogger.Instance.LogDebug("[Presenter] ▶ Refresh() called");

            // 1) Reload only the *current* config’s entries
            model.ReloadCurrentConfig();

            // 2) Push the selected asset into the view header
            view.UpdateConfigAsset(model.Config);

            // 3) Build & page‐slice view‐models exactly like before
            var all = model.Entries;
            totalPages = Mathf.CeilToInt(all.Count / (float)pageSize);
            currentPage = Mathf.Clamp(currentPage, 0, Mathf.Max(0, totalPages - 1));

            var vms = all
              .Skip(currentPage * pageSize)
              .Take(pageSize)
              .Select(e => new AssetEntryViewModel
              {
                  Entry = e,
                  Exists = model.FileExists(e)
              })
              .ToList();

            // 4) Render
            view.UpdateEntriesViewModels(vms);
            view.UpdatePageInfo(currentPage, totalPages);

            InternalLogger.Instance.LogDebug("[Presenter] ✔ Refresh complete");
        }

        private void HandleAddLocal()
        {
            // ask the view to pop up a file dialog
            string absolute = EditorUtility.OpenFilePanel(
                "Select UnityPackage",
                model.ParentPath,
                "unitypackage"
            );
            if (string.IsNullOrEmpty(absolute)) return;

            string relative = IOManager.GetRelativePath(model.ParentPath, absolute);
            var entry = new AssetEntry
            {
                sourceType = PackageSourceType.LocalUnityPackage,
                path = relative
            };
            model.AddEntry(entry);
            Refresh();
        }

        private void HandleAddGit()
        {
            var entry = new AssetEntry(); /* popup for git URL */
            if (entry != null) model.AddEntry(entry);
            Refresh();
        }

        private void HandleLocate(int index)
        {
            // 1) bounds‐check
            var all = model.Entries;
            if (index < 0 || index >= all.Count)
            {
                InternalLogger.Instance.LogError($"[Presenter] Invalid locate index: {index}");
                return;
            }

            // 2) pick the entry to update
            var entry = all[index];

            // 3) show file panel
            string selected = EditorUtility.OpenFilePanel(
                "Locate UnityPackage",
                model.ParentPath,
                "unitypackage"
            );
            if (string.IsNullOrEmpty(selected) || !selected.EndsWith(".unitypackage"))
                return;

            // 4) convert to relative
            string relative = IOManager.GetRelativePath(model.ParentPath, selected);
            if (string.IsNullOrEmpty(relative))
            {
                InternalLogger.Instance.LogError("[Presenter] Failed to compute relative path");
                return;
            }

            // 5) duplicate check (excluding this entry)
            bool dup = all.Exists(e =>
                e != entry &&
                e.sourceType == PackageSourceType.LocalUnityPackage &&
                e.path == relative
            );
            if (dup)
            {
                InternalLogger.Instance.LogWarning("[Presenter] This asset is already in the config.");
                return;
            }

            // 6) apply the update on the model
            entry.sourceType = PackageSourceType.LocalUnityPackage;
            entry.path = relative;
            model.SaveConfig();

            InternalLogger.Instance.LogDebug($"[Presenter] Located package #{index} → {relative}");

            // 7) refresh the view
            Refresh();
        }

        private void HandleRemove(int index)
        {
            var entry = model.Entries[index];
            model.RemoveEntry(entry);
            Refresh();
        }

        private void HandleImport()
        {
            foreach (var e in model.Entries)
            {
                if (e.sourceType == PackageSourceType.LocalUnityPackage && model.FileExists(e))
                {
                    AssetDatabase.ImportPackage(
                       IOManager.CombinePaths(model.ParentPath, e.path),
                       false
                    );
                }
                else if (e.sourceType == PackageSourceType.GitURL)
                {
                    /* register to manifest… */
                }
            }
        }

        private void HandlePageChange(int page)
        {
            currentPage = page;
            Refresh();
        }

        private void HandleSelectConfig(int newIndex)
        {
            if (newIndex < 0 || newIndex >= configs.Count) return;
            currentConfigIndex = newIndex;
            model.SelectConfig(configs[newIndex]);
            view.UpdateConfigPageInfo(currentConfigIndex, configs.Count);
            Refresh();
        }

        private void HandleRefreshRequested()
        {
            // if you have a “Refresh” button, it should only reload entries
            RefreshAllConfigs();
            Refresh();
        }
    }
}