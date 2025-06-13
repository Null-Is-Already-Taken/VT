#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace VT.Tools.EssentialAssetsImporter
{
    /// <summary>
    /// Installs a Package Manager package from a Git URL that points to a folder containing package.json.
    /// </summary>
    public static class GitPackageInstaller
    {
        public static event Action<string>         OnInstallStarted;
        public static event Action<string, string> OnInstallSucceeded; // url, packageId
        public static event Action<string, string> OnInstallFailed;    // url, errorMessage

        private static AddRequest _currentRequest;
        private static string     _currentUrl;

        /// <summary>
        /// Begins installing the given UPM Git URL.  
        /// Example:
        ///   "https://github.com/Cysharp/ZLinq.git?path=/src/ZLinq.Unity/Assets/ZLinq.Unity#main"
        /// </summary>
        public static void Install(string url)
        {
            if (_currentRequest != null)
            {
                Debug.LogWarning($"A package install is already in progress: {_currentUrl}");
                return;
            }

            _currentUrl     = url;
            _currentRequest = Client.Add(url);
            OnInstallStarted?.Invoke(url);

            // Poll until done
            EditorApplication.update += Progress;
        }

        private static void Progress()
        {
            if (_currentRequest == null || !_currentRequest.IsCompleted)
                return;

            if (_currentRequest.Status == StatusCode.Success)
            {
                var installed = _currentRequest.Result.packageId; 
                Debug.Log($"✔ Installed package: {installed}");
                OnInstallSucceeded?.Invoke(_currentUrl, installed);
            }
            else
            {
                var error = _currentRequest.Error.message;
                Debug.LogError($"✘ Failed to install '{_currentUrl}': {error}");
                OnInstallFailed?.Invoke(_currentUrl, error);
            }

            // Clean up
            _currentRequest = null;
            _currentUrl     = null;
            EditorApplication.update -= Progress;
        }
    }
}
#endif
