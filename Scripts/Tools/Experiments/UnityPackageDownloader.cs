#if UNITY_EDITOR
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;
using VT.IO;
using VT.Logger;

namespace VT.Tools.Experiments
{
    public static class UnityPackageDownloader
    {
        public static event Action<string> OnDownloadStarted;
        public static event Action<string, float> OnDownloadProgress;
        public static event Action<string, string> OnDownloadCompleted;
        public static event Action<string, string> OnDownloadFailed;

        private static readonly string PackageCacheRoot = IOManager.CombinePaths
        (
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EssentialAssetImporter", "Cache"
        );

        /// <summary>
        /// Downloads a .unitypackage, streaming it to disk.
        /// </summary>
        /// <param name="url">HTTP(S) URL ending in ".unitypackage"</param>
        /// <param name="cancellationToken">Optional token to cancel/abort</param>
        /// <returns>The local file path, or null on failure.</returns>
        public static async Task<string> DownloadUnityPackage(string url, CancellationToken cancellationToken = default)
        {
            // 1) Validate URL
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                || !uri.AbsolutePath.EndsWith(".unitypackage", StringComparison.OrdinalIgnoreCase))
            {
                OnDownloadFailed?.Invoke(url, "Invalid UnityPackage URL");
                return null;
            }

            // 2) Compute paths
            string fileName = IOManager.GetURIFileName(uri);
            string version = IOManager.GetURIVersion(uri);
            string packageName = IOManager.GetURIPackageName(uri);
            string targetFolder = IOManager.CombinePaths(PackageCacheRoot, packageName, version);
            string targetPath = IOManager.CombinePaths(targetFolder, fileName);

            // 3) Already cached?
            if (IOManager.FileExists(targetPath))
            {
                OnDownloadCompleted?.Invoke(url, targetPath);
                return targetPath;
            }

            // 4) Prepare folder
            try
            {
                IOManager.CreateDirectory(targetFolder);
            }
            catch (Exception ex)
            {
                OnDownloadFailed?.Invoke(url, $"Failed to create cache folder: {ex.Message}");
                return null;
            }

            OnDownloadStarted?.Invoke(url);

            // 5) Start request with stream handler
            using (var request = UnityWebRequest.Get(url))
            {
                // DownloadHandlerFile will write directly to disk
                request.downloadHandler = new DownloadHandlerFile(targetPath);

                // Abort if cancellation is requested
                using (cancellationToken.Register(() => request.Abort()))
                {
                    var op = request.SendWebRequest();
                    while (!op.isDone)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        OnDownloadProgress?.Invoke(url, request.downloadProgress);
                        await Task.Yield();
                    }
                }

                // 6) Check result
                if (request.result != UnityWebRequest.Result.Success)
                {
                    var error = cancellationToken.IsCancellationRequested
                        ? "Download canceled"
                        : request.error;

                    InternalLogger.Instance.LogError($"[UnityPackageDownloader] Failed: {error}");
                    OnDownloadFailed?.Invoke(url, error);

                    // remove partial file
                    try { if (File.Exists(targetPath)) File.Delete(targetPath); } catch { }

                    return null;
                }

                InternalLogger.Instance.LogDebug($"[UnityPackageDownloader] Completed: {targetPath}");
                OnDownloadCompleted?.Invoke(url, targetPath);
            }

            return targetPath;
        }
    }
}
#endif
