#if UNITY_EDITOR
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace VT.Editor.Utils
{
    public static class UPMClientWrapper
    {
        // Generic wrapper for Request<T>
        static Task<T> Wrap<T>(Request<T> req)
        {
            var tcs = new TaskCompletionSource<T>();
            void Poll()
            {
                if (!req.IsCompleted) return;
                EditorApplication.update -= Poll;

                if (req.Status == StatusCode.Success)
                    tcs.SetResult(req.Result);
                else if (req.Status >= StatusCode.Failure)
                    tcs.SetException(new Exception(req.Error.message));
            }

            EditorApplication.update += Poll;
            return tcs.Task;
        }

        // Non-generic wrapper for Request + extractor
        static Task<T> Wrap<T>(Request req, Func<T> extractor)
        {
            var tcs = new TaskCompletionSource<T>();
            void Poll()
            {
                if (!req.IsCompleted) return;
                EditorApplication.update -= Poll;

                if (req.Status == StatusCode.Success)
                    tcs.SetResult(extractor());
                else if (req.Status >= StatusCode.Failure)
                    tcs.SetException(new Exception(req.Error.message));
            }

            EditorApplication.update += Poll;
            return tcs.Task;
        }

        public static async Task<UnityEditor.PackageManager.PackageInfo[]> ListInstalledAsync(bool includeDependencies = false)
        {
            var col = await Wrap(Client.List(includeDependencies));
            return col.ToArray();
        }

        public static async Task<UnityEditor.PackageManager.PackageInfo[]> SearchRemoteAsync(string packageName, bool offlineMode = false)
        {
            var col = await Wrap(Client.Search(packageName, offlineMode));
            return col.ToArray();
        }

        public static async Task<string> AddPackageAsync(string identifier)
        {
            var info = await Wrap(Client.Add(identifier));
            return info.packageId;
        }

        public static Task<string> RemovePackageAsync(string packageName) => Wrap(Client.Remove(packageName), () => packageName);

        public static Task<string> EmbedPackageAsync(string packageName) => Wrap(Client.Embed(packageName), () => packageName);

        public static async Task<UnityEditor.PackageManager.PackageInfo[]> GetDependenciesAsync(string packageName, bool recursive = false)
        {
            var all = await ListInstalledAsync(includeDependencies: true);
            var map = all.ToDictionary(p => p.name);

            if (!map.TryGetValue(packageName, out var root))
                throw new Exception($"Package '{packageName}' is not installed.");

            var result = new System.Collections.Generic.List<UnityEditor.PackageManager.PackageInfo>();
            var seen = new System.Collections.Generic.HashSet<string>();

            void Traverse(UnityEditor.PackageManager.PackageInfo pkg)
            {
                foreach (var dep in pkg.dependencies)
                {
                    if (!seen.Add(dep.name)) continue;
                    if (map.TryGetValue(dep.name, out var info))
                    {
                        result.Add(info);
                        if (recursive) Traverse(info);
                    }
                }
            }

            Traverse(root);
            return result.ToArray();
        }

        public static async Task<bool> IsPackageInstalledAsync(string identifier)
        {
            var installed = await ListInstalledAsync();

            // Try splitting identifier to check for version
            string name = identifier;
            string version = null;

            int atIndex = identifier.IndexOf('@');
            if (atIndex >= 0)
            {
                name = identifier[..atIndex];
                version = identifier[(atIndex + 1)..];
            }

            return installed.Any(pkg =>
            {
                bool nameMatch = string.Equals(pkg.name, name, StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(pkg.packageId, identifier, StringComparison.OrdinalIgnoreCase) ||
                                 identifier.Contains(pkg.name, StringComparison.OrdinalIgnoreCase);

                if (!nameMatch) return false;

                // If version was specified, check it
                if (!string.IsNullOrEmpty(version))
                {
                    return string.Equals(pkg.version, version, StringComparison.OrdinalIgnoreCase);
                }

                return true; // Name matches and no version constraint
            });
        }
    }
}
#endif
