using Sirenix.OdinInspector;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace VT.Tools.EssentialAssetsImporter
{
    [CreateAssetMenu(fileName = "New Assets Config", menuName = "Essential Assets Importer/Assets Config")]
    public class AssetsConfig : ScriptableObject
    {
        [BoxGroup("Entries")]
        [TableList]
        [ReadOnly]
        [HideLabel]
        public List<AssetEntry> assetsPaths = new();

        [Serializable]
        public class AssetEntry
        {
            [ReadOnly]
            public string relativePath;
        }
    }
}
