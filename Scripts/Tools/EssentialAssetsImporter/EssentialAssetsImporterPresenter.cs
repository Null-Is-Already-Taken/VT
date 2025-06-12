using System.Linq;
using UnityEditor;
using UnityEngine;
using VT.IO;
using VT.Logger;

namespace VT.Tools.EssentialAssetsImporter
{
    // Presenters/EssentialAssetsImporterPresenter.cs
    public class EssentialAssetsImporterPresenter
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
            view.OnImportAllRequested += HandleImportAll;
            view.OnRefreshRequested += Refresh;
            view.OnPageChanged += HandlePageChange;

            Refresh();
        }

        private readonly IEssentialAssetsImporterView view;
        private readonly IAssetsConfigModel model;
        private int currentPage = 0;
        private int totalPages = 0;
        private const int pageSize = 3;

        private void Refresh()
        {
            InternalLogger.Instance.LogDebug("[Presenter] ▶ Refresh() called");

            // 1) Load config
            InternalLogger.Instance.LogDebug("[Presenter] └─ Loading config...");
            model.LoadConfig();

            // tell the view which ScriptableObject was loaded
            view.UpdateConfigAsset(model.Config);

            // 2) Grab all entries
            var all = model.Entries;
            InternalLogger.Instance.LogDebug($"[Presenter] └─ Total entries loaded: {all.Count}");

            // 3) Recompute pagination
            totalPages = Mathf.CeilToInt(all.Count / (float)pageSize);
            currentPage = Mathf.Clamp(currentPage, 0, Mathf.Max(0, totalPages - 1));
            InternalLogger.Instance.LogDebug($"[Presenter] └─ Pagination → pageSize={pageSize}, totalPages={totalPages}, currentPage={currentPage}");

            // 4) Build view‐models for the current page, logging each existence check
            var vms = all
                .Skip(currentPage * pageSize)
                .Take(pageSize)
                .Select(e =>
                {
                    bool exists = model.FileExists(e);
                    InternalLogger.Instance.LogDebug($"[Presenter]    • Entry '{e}' exists on disk? {exists}");
                    return new AssetEntryViewModel
                    {
                        Entry = e,
                        Exists = exists
                    };
                })
                .ToList();

            // 5) Push to the view
            InternalLogger.Instance.LogDebug($"[Presenter] └─ Displaying {vms.Count} entries in view");
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
                Debug.LogError($"[Presenter] Invalid locate index: {index}");
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
                Debug.LogError("[Presenter] Failed to compute relative path");
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
                Debug.LogWarning("[Presenter] This asset is already in the config.");
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

        private void HandleImportAll()
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
    }
}