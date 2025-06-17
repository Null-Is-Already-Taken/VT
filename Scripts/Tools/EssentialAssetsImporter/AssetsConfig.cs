#if UNITY_EDITOR

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using System;

#endif

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VT.Editor.Utils;
using VT.IO;
using VT.Logger;

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

        public static AssetsConfig LoadFromJson(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("Config not found: " + path);
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<AssetsConfig>(json);
        }

        public void LoadConfigFromJSON(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    AssetsConfig newConfig = LoadFromJson(path);

                    assetsEntries.Clear();
                    //foreach (var entry in newConfig.assetsEntries)
                    //{
                    //    entry.RelativePath = PathUtils.FromAlias(entry.RelativePath); // resolve alias to absolute
                    //    assetsEntries.Add(entry);
                    //}
                }
                catch (Exception ex)
                {
                    InternalLogger.Instance.LogError("Failed to load config: " + ex.Message);
                }
            }
        }

        public void SaveConfigToJSON(string path)
        {
            try
            {
                // Clone and alias the paths
                AssetsConfig configToSave = CreateInstance<AssetsConfig>();
                configToSave.assetsEntries = assetsEntries
                        .Select(entry => new AssetEntry(
                            sourceType: entry.sourceType,
                            absolutePath: PathUtils.FromAlias(entry.AbsolutePath)
                        ))
                        .ToList();

                string json = JsonUtility.ToJson(configToSave, true);

                IOManager.WriteAllText(path, json);

                Debug.Log("Config saved to: " + path);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to save config: " + ex.Message);
            }
        }
    }
}

#endif // UNITY_EDITOR
