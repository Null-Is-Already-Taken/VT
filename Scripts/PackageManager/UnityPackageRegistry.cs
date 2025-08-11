#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using VT.IO;

namespace VT.PackageManager
{
    [Serializable]
    public class UnityPackageRegistry
    {
        public List<UnityPackageRecord> records = new();

        public bool IsImported(string aliasPath)
        {
            return records.Any(r => r.aliasPath == aliasPath);
        }

        public void AddOrUpdate(string aliasPath, string hash = null)
        {
            var existing = records.FirstOrDefault(r => r.aliasPath == aliasPath);
            if (existing != null)
            {
                existing.importedAtUtc = DateTime.UtcNow.ToString("o");
                existing.sha256Hash = hash;
            }
            else
            {
                records.Add(new UnityPackageRecord
                {
                    aliasPath = aliasPath,
                    importedAtUtc = DateTime.UtcNow.ToString("o"),
                    sha256Hash = hash
                });
            }
        }

        public void Save(string path)
        {
            IOManager.SaveJson(path, this);
        }

        public static UnityPackageRegistry Load(string path)
        {
            return IOManager.LoadJson<UnityPackageRegistry>(path) ?? new UnityPackageRegistry();
        }
    }
}
#endif
