﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VT.Logger;

namespace VT.Tools.EssentialAssetsImporter
{
    /// <summary>
    /// Presenter for the Essential Assets Importer.
    /// Handles UI events, orchestrates model actions, and updates the view.
    /// Implements IDisposable to clean up event subscriptions.
    /// </summary>
    public class EssentialAssetsImporterPresenter : IDisposable
    {
        //--- Fields ---//

        private readonly EssentialAssetsImporterView view;
        private readonly EssentialAssetsImporterModel model;
        private List<AssetsConfig> configs;
        private int currentConfigIndex;
        private int currentPage;
        private int totalPages;
        private const int pageSize = 5;
        private bool hasInit;
        private bool isDisposed;
        
        //--- Constructor ---//

        /// <summary>
        /// Creates the presenter, hooks view events, and performs initial load.
        /// </summary>
        public EssentialAssetsImporterPresenter(
            EssentialAssetsImporterView view,
            EssentialAssetsImporterModel model)
        {
            this.view = view;
            this.model = model;

            // Kick off initial load
            Init();
        }

        //--- Initialization & Refresh ---//

        /// <summary>
        /// Performs the first-time setup: loads or creates config assets, then refreshes UI.
        /// </summary>
        private void Init()
        {
            if (!hasInit)
            {
                model.LoadConfig();      // Scan or create config
                RefreshAllConfigs();     // Populate config list

                SubscribeViewEvents(view);     // hook up view events

                hasInit = true;
            }

            // Always do a normal refresh of the *current* config’s entries
            Refresh();
        }

        private void SubscribeViewEvents(EssentialAssetsImporterView view)
        {
            // Subscribe to view events
            view.OnLoadConfigRequested += Refresh;
            view.OnLoadConfigFromJSONRequested += HandleLoadConfigFromJSON;
            view.OnAddLocalRequested += HandleAddLocal;
            view.OnAddGitRequested += HandleAddGit;
            view.OnLocateRequested += HandleLocate;
            view.OnRemoveRequested += HandleRemove;
            view.OnImportAllRequested += HandleImportAsync;
            view.OnRefreshRequested += HandleRefreshRequested;
            view.OnPageChanged += HandlePageChange;
            view.OnSelectConfigRequested += HandleSelectConfig;
        }

        /// <summary>
        /// Refreshes all available config profiles and UI.
        /// </summary>
        private void RefreshAllConfigs()
        {
            configs = model.GetAllConfigs().ToList();
            currentConfigIndex = Mathf.Clamp(currentConfigIndex, 0, configs.Count - 1);

            if (configs.Count > 0)
                model.SelectConfig(configs[currentConfigIndex]);

            view.UpdateConfigList(configs);
            view.UpdateConfigPageInfo(currentConfigIndex, configs.Count);
        }

        /// <summary>
        /// Reloads the current config entries and updates the view.
        /// </summary>
        private void Refresh()
        {
            InternalLogger.Instance.LogDebug("[Presenter] Refresh() starting");

            model.ReloadCurrentConfig();
            view.UpdateConfigAsset(model.Config);

            var entryVMs = BuildEntryViewModels();
            view.UpdateEntriesViewModels(entryVMs);
            view.UpdatePageInfo(currentPage, totalPages);

            InternalLogger.Instance.LogDebug("[Presenter] Refresh() complete");
        }

        /// <summary>
        /// Constructs paginated AssetEntryViewModel list based on model entries.
        /// </summary>
        private List<AssetEntryViewModel> BuildEntryViewModels()
        {
            var allEntries = model.Entries;
            totalPages = Mathf.CeilToInt(allEntries.Count / (float)pageSize);
            currentPage = Mathf.Clamp(currentPage, 0, Mathf.Max(0, totalPages - 1));

            return allEntries
                .Skip(currentPage * pageSize)
                .Take(pageSize)
                .Select(e => new AssetEntryViewModel
                {
                    Entry = e,
                    Exists = model.FileExists(e)
                })
                .ToList();
        }

        //--- Disposal ---//

        /// <summary>
        /// Unsubscribes from view events.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            UnsubscribeViewEvents();

            isDisposed = true;
        }

        private void UnsubscribeViewEvents()
        {
            view.OnLoadConfigRequested -= Refresh;
            view.OnLoadConfigFromJSONRequested -= HandleLoadConfigFromJSON;
            view.OnAddLocalRequested -= HandleAddLocal;
            view.OnAddGitRequested -= HandleAddGit;
            view.OnLocateRequested -= HandleLocate;
            view.OnRemoveRequested -= HandleRemove;
            view.OnImportAllRequested -= HandleImportAsync;
            view.OnRefreshRequested -= HandleRefreshRequested;
            view.OnPageChanged -= HandlePageChange;
            view.OnSelectConfigRequested -= HandleSelectConfig;
        }

        /// <summary>
        /// Helper to call Dispose() safely (from EditorWindow.OnDisable, etc.).
        /// </summary>
        public void DisposeIfNeeded() => Dispose();

        //--- Event Handlers ---//

        private void HandleAddLocal(string absolutePath)
        {
            model.HandleAddLocal(absolutePath);
            Refresh();
        }

        private void HandleAddGit(string gitURL)
        {
            model.HandleAddGit(gitURL);
            Refresh();
        }

        private void HandleLocate(int index)
        {
            model.HandleLocate(index);
            Refresh();
        }

        private void HandleRemove(int index)
        {
            var entry = model.Entries[index];
            model.RemoveEntry(entry);
            Refresh();
        }

        private async void HandleImportAsync()
        {
            await model.HandleImportAsync();
            Refresh();
        }

        private void HandlePageChange(int page)
        {
            currentPage = page;
            Refresh();
        }

        private void HandleSelectConfig(int newIndex)
        {
            if (newIndex < 0 || newIndex >= configs.Count)
                return;

            currentConfigIndex = newIndex;
            model.SelectConfig(configs[newIndex]);
            view.UpdateConfigPageInfo(currentConfigIndex, configs.Count);
            Refresh();
        }

        private void HandleLoadConfigFromJSON()
        {
            model.HandleLoadConfigFromJSON((path) =>
            {
                if (!string.IsNullOrEmpty(path))
                {
                    model.LoadConfigFromJSON(path);
                    RefreshAllConfigs();
                    Refresh();
                }
            });
        }

        private void HandleRefreshRequested()
        {
            RefreshAllConfigs();
            Refresh();
        }
    }
}