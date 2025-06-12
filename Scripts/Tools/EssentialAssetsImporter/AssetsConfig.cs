using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace VT.Tools.EssentialAssetsImporter
{
    [CreateAssetMenu(fileName = "New Assets Config", menuName = "Essential Assets Importer/Assets Config")]
    public class AssetsConfig : ScriptableObject
    {
        [BoxGroup("Entries")]
        [TableList]
        [HideLabel]
        public List<AssetEntry> assetsEntries = new();
    }
}
