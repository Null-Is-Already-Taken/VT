using System;

namespace VT.PackageManager
{ 
    [Serializable]
    public class UnityPackageRecord
    {
        public string aliasPath;
        public string importedAtUtc;
        public string sha256Hash; // optional
    }
}
