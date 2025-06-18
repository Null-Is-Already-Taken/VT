#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using VT.Logger;

namespace VT.Tools.EssentialAssetsImporter
{
    [CreateAssetMenu(fileName = "New Assets Config", menuName = "Essential Assets Importer/Assets Config")]
    public class AssetsConfig : ScriptableObject
    {
        [SerializeField]
        private AssetEntryList entries;

        public AssetEntryList Entries => entries;

        public void Init()
        {
            if (entries == null)
            {
                entries = new AssetEntryList();
            }
            else
            {
                entries.Clear();
            }
        }

        public void AssignEntries(AssetEntryList entries)
        {
            if (entries == null || entries.Count == 0)
            {
                InternalLogger.Instance.LogWarning("[AssetsConfig] No entries provided to assign.");
                return;
            }

            InternalLogger.Instance.LogDebug($"[AssetsConfig] Assigning {entries.Count} entries to AssetsConfig.");

            this.entries.Clear();
            this.entries = entries;
        }

        internal bool Exists(AssetEntry entry)
        {
            return entries.list.Any(e => e.Equals(entry));
        }
    }
}

#endif // UNITY_EDITOR
