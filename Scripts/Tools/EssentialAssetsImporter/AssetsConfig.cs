using Sirenix.OdinInspector;
using UnityEngine;
using System;
using System.Collections.Generic;
using VT.IO;

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

            /// <summary>
            /// Returns the author and package name from the relative path (e.g., "Sirenix/Odin Inspector").
            /// </summary>
            public string AuthorAndPackage
            {
                get
                {
                    if (string.IsNullOrEmpty(relativePath))
                        return string.Empty;

                    var parts = relativePath.Split('/');
                    if (parts.Length < 2)
                        return relativePath;

                    string author = parts[0];

                    // Get last part without ".unitypackage"
                    string packageFile = parts[^1];
                    string packageName = IOManager.GetFileNameWithoutExtension(packageFile);

                    return $"{author}/{packageName}";
                }
            }
        }

    }
}
