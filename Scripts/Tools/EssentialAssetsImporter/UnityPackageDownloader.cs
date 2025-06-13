#if UNITY_EDITOR

using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace VT.Tools.EssentialAssetsImporter
{
    public static class UnityPackageDownloader
    {
        public static event Action<string> OnDownloadStarted;
        public static event Action<string, float> OnDownloadProgress;
        public static event Action<string, string> OnDownloadCompleted;
        public static event Action<string, string> OnDownloadFailed;

        private static readonly string PackageCacheRoot = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EssentialAssetImporter", "Cache");

        public static async Task<string> DownloadUnityPackage(string url)
        {
            if (!url.EndsWith(".unitypackage"))
            {
                OnDownloadFailed?.Invoke(url, "URL must end with .unitypackage");
                return null;
            }

            string fileName = Path.GetFileName(url);
            string version = ExtractVersion(url);
            string packageName = ExtractPackageName(fileName, version);
            string targetFolder = Path.Combine(PackageCacheRoot, packageName, version);
            string targetPath = Path.Combine(targetFolder, fileName);

            if (File.Exists(targetPath))
            {
                Debug.Log($"Package already exists at: {targetPath}");
                OnDownloadCompleted?.Invoke(url, targetPath);
                return targetPath;
            }

            Directory.CreateDirectory(targetFolder);
            OnDownloadStarted?.Invoke(url);

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                UnityWebRequestAsyncOperation operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    OnDownloadProgress?.Invoke(url, request.downloadProgress);
                    await Task.Yield();
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to download package: {request.error}");
                    OnDownloadFailed?.Invoke(url, request.error);
                    return null;
                }

                File.WriteAllBytes(targetPath, request.downloadHandler.data);
                Debug.Log($"Downloaded to: {targetPath}");
                OnDownloadCompleted?.Invoke(url, targetPath);
            }

            return targetPath;
        }

        private static string ExtractVersion(string url)
        {
            var segments = url.Split('/');
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] == "download" && i + 1 < segments.Length)
                    return segments[i + 1];
            }
            return "unknown";
        }

        private static string ExtractPackageName(string fileName, string version)
        {
            if (fileName.Contains(version))
                return fileName.Replace(version, "").Replace(".unitypackage", "").TrimEnd('.');
            return Path.GetFileNameWithoutExtension(fileName);
        }
    }
}

#endif
