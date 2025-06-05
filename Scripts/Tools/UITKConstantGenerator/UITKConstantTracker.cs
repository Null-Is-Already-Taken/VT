#if UNITY_EDITOR

using UnityEditor;
using System.IO;
using System.Collections.Generic;
using VT.IO;
using VT.Utils.Logger;

namespace VT.Tools.UITKConstantGenerator
{
    public class UITKConstantTracker : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var trackingMap = UITKConstantTrackerHelper.LoadTrackingData();
            Dictionary<string, List<string>> ussToUxmlMap = UITKConstantTrackerHelper.BuildUSSToUXMLMap(trackingMap);

            HashSet<string> dirtyUxmls = new();

            foreach (string path in importedAssets)
            {
                if (path.EndsWith(".uxml"))
                {
                    string normalized = IOManager.NormalizePathSeparators(path);
                    if (trackingMap.ContainsKey(normalized))
                    {
                        dirtyUxmls.Add(normalized);
                    }
                }
            }

            foreach (string path in importedAssets)
            {
                if (path.EndsWith(".uss"))
                {
                    string ussPath = IOManager.NormalizePathSeparators(path);
                    if (ussToUxmlMap.TryGetValue(ussPath, out var linkedUxmls))
                    {
                        foreach (var uxml in linkedUxmls)
                        {
                            if (trackingMap.ContainsKey(uxml))
                            {
                                dirtyUxmls.Add(uxml);
                            }
                        }
                    }
                }
            }

            foreach (string uxmlPath in dirtyUxmls)
            {
                GenerateAndWriteConstants(uxmlPath);
            }

            if (dirtyUxmls.Count > 0)
                AssetDatabase.Refresh();
        }

        private static void GenerateAndWriteConstants(string uxmlPath)
        {
            if (!File.Exists(uxmlPath)) return;

            string className = UITKConstantGeneratorAPI.SanitizeClassName(Path.GetFileNameWithoutExtension(uxmlPath));
            string content = UITKConstantGeneratorAPI.GenerateClassContent(uxmlPath, className);
            string outputPath = Path.Combine(Path.GetDirectoryName(uxmlPath), className + ".cs");
            outputPath = IOManager.NormalizePathSeparators(outputPath);

            File.WriteAllText(outputPath, content);
            InternalLogger.Instance.LogDebug($"[UIConstantTracker] Synced: {outputPath}");
        }
    }
}

#endif
