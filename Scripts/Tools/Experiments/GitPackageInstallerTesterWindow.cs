#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VT.Tools.EssentialAssetsImporter
{
    /// <summary>
    /// Simple tester for the GitPackageInstaller
    /// </summary>
    public class GitPackageInstallerTesterWindow : EditorWindow
    {
        private string url = "https://github.com/Cysharp/ZLinq.git?path=/src/ZLinq.Unity/Assets/ZLinq.Unity#main";
        private string status = "Idle";

        [MenuItem("Tools/VT/Experiments/Git Package Installer Tester")]
        public static void ShowWindow()
        {
            GetWindow<GitPackageInstallerTesterWindow>("Package Installer Tester");
        }

        private void OnEnable()
        {
            GitPackageInstaller.OnInstallStarted += HandleInstallStarted;
            GitPackageInstaller.OnInstallSucceeded += HandleInstallSucceeded;
            GitPackageInstaller.OnInstallFailed += HandleInstallFailed;
        }

        private void OnDisable()
        {
            GitPackageInstaller.OnInstallStarted -= HandleInstallStarted;
            GitPackageInstaller.OnInstallSucceeded -= HandleInstallSucceeded;
            GitPackageInstaller.OnInstallFailed -= HandleInstallFailed;
        }

        private void HandleInstallStarted(string u)
        {
            status = $"Starting install: {u}";
        }

        private void HandleInstallSucceeded(string u, string packageId)
        {
            status = $"Installed: {packageId}";
        }

        private void HandleInstallFailed(string u, string error)
        {
            status = $"Failed: {error}";
        }

        private void OnGUI()
        {
            GUILayout.Label("Git Package Installer Tester", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            url = EditorGUILayout.TextField("Package URL", url);

            EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(url));
            if (GUILayout.Button("Install Package"))
            {
                status = "Installing...";
                GitPackageInstaller.Install(url);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Status:", status);
        }
    }
}
#endif
