using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VT.IO;

namespace VT.Tools.UITKConstantGenerator
{
    public static class UITKConstantTrackerHelper
    {
        private const string TrackingFilePath = "Assets/UITKConstantGenerator/Generated/UITKConstantTracking.json";

        public static Dictionary<string, string> LoadTrackingData()
        {
            if (!File.Exists(TrackingFilePath))
                return new Dictionary<string, string>();

            string json = File.ReadAllText(TrackingFilePath);
            return JsonUtilityWrapper.FromJson<SerializableDict>(json)?.ToDictionary() ?? new Dictionary<string, string>();
        }

        public static void SaveTrackingData(Dictionary<string, string> data)
        {
            string json = JsonUtilityWrapper.ToJson(new SerializableDict(data), true);
            Directory.CreateDirectory(Path.GetDirectoryName(TrackingFilePath)!);
            File.WriteAllText(TrackingFilePath, json);
        }

        public static Dictionary<string, List<string>> BuildUSSToUXMLMap(Dictionary<string, string> tracking)
        {
            var map = new Dictionary<string, List<string>>();

            foreach (var uxmlPath in tracking.Keys)
            {
                if (!File.Exists(uxmlPath)) continue;

                var lines = File.ReadAllLines(uxmlPath);
                foreach (var line in lines)
                {
                    if (!line.Contains(".uss")) continue;

                    var ussRef = NormalizeUSSPath(line);
                    if (string.IsNullOrEmpty(ussRef)) continue;

                    if (!map.ContainsKey(ussRef))
                        map[ussRef] = new List<string>();

                    if (!map[ussRef].Contains(uxmlPath))
                        map[ussRef].Add(uxmlPath);
                }
            }

            return map;
        }

        //public static string ExtractUSSPath(string line)
        //{
        //    var prefix = "project://database/";
        //    var start = line.IndexOf(prefix);
        //    if (start == -1) return null;

        //    int end = line.IndexOf(".uss", start);
        //    if (end == -1) return null;

        //    end += ".uss".Length;
        //    string path = line.Substring(start + prefix.Length, end - start - prefix.Length);
        //    return IOManager.NormalizePathSeparators("Assets/" + path.TrimStart('/'));
        //}

        public static string NormalizeUSSPath(string rawSrc)
        {
            string path = rawSrc;

            // Decode HTML entities like &amp;
            path = System.Net.WebUtility.HtmlDecode(path);

            // Convert Unity's virtual path to actual asset path
            if (path.StartsWith("project://database/Assets"))
            {
                path = path.Replace("project://database/Assets", "Assets/");
            }

            // Remove query strings (e.g., ?fileID=... or #...)
            int queryIndex = path.IndexOfAny(new char[] { '?', '#' });
            if (queryIndex >= 0)
            {
                path = path[..queryIndex];
            }

            return IOManager.NormalizePathSeparators(path);
        }

        [System.Serializable]
        private class SerializableDict
        {
            public List<string> Keys = new();
            public List<string> Values = new();

            public SerializableDict() { }

            public SerializableDict(Dictionary<string, string> dict)
            {
                foreach (var kv in dict)
                {
                    Keys.Add(kv.Key);
                    Values.Add(kv.Value);
                }
            }

            public Dictionary<string, string> ToDictionary()
            {
                var dict = new Dictionary<string, string>();
                for (int i = 0; i < Keys.Count && i < Values.Count; i++)
                    dict[Keys[i]] = Values[i];
                return dict;
            }
        }

        private static class JsonUtilityWrapper
        {
            public static T FromJson<T>(string json) => JsonUtility.FromJson<T>(json);
            public static string ToJson<T>(T obj, bool prettyPrint = false) => JsonUtility.ToJson(obj, prettyPrint);
        }
    }
}
