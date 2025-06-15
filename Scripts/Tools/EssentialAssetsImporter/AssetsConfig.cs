#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using System.Collections.Generic;
using UnityEngine;

namespace VT.Tools.EssentialAssetsImporter
{
    [CreateAssetMenu(fileName = "New Assets Config", menuName = "Essential Assets Importer/Assets Config")]
    public class AssetsConfig : ScriptableObject
    {
#if ODIN_INSPECTOR
        [BoxGroup("Entries")]
        [TableList]
        [HideLabel]
#endif
        public List<AssetEntry> assetsEntries = new();
    }
}
