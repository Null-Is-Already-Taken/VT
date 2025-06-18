#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VT.Editor.Utils;

namespace VT.Tools.Experiments
{
    public class UpmClientWrapperTester : EditorWindow
    {
        private string  _identifier  = "com.unity.textmeshpro";
        private string  _packageName = "com.unity.collections";
        private Vector2 _scrollPos;
        private string  _log = "";

        [MenuItem("Tools/UPM Wrapper Tester")]
        public static void ShowWindow()
        {
            var window = GetWindow<UpmClientWrapperTester>("UPM Tester");
            window.minSize = new Vector2(450, 420);
        }

        private void OnGUI()
        {
            GUILayout.Label("UPM Client Wrapper Tester", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _identifier  = EditorGUILayout.TextField("Identifier",  _identifier);
            _packageName = EditorGUILayout.TextField("Package Name", _packageName);

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("List Installed")) ListInstalled();
                if (GUILayout.Button("Search Remote")) SearchRemote();
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add Package")) AddPackage();
                if (GUILayout.Button("Remove Package")) RemovePackage();
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Embed Package")) EmbedPackage();
                if (GUILayout.Button("Get Dependencies")) GetDependencies();
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Check Package Is Installed")) CheckIsInstalled();
            }

            EditorGUILayout.Space();
            GUILayout.Label("Output Log:", EditorStyles.boldLabel);

            if (_scrollToBottom)
            {
                _scrollPos.y = float.MaxValue;
                _scrollToBottom = false;
            }
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(200));
            EditorGUILayout.TextArea(_log, GUILayout.ExpandHeight(true));
            
            EditorGUILayout.EndScrollView();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Clear Log"))
                {
                    _log = "";
                    
                }
            }
        }

        private bool _scrollToBottom = false;

        private void Append(string message)
        {
            _log += message + "\n";
            _scrollToBottom = true;
        }

        private async void ListInstalled()
        {
            Append("⏳ Listing installed packages...");
            try
            {
                var list = await UPMClientWrapper.ListInstalledAsync(includeDependencies: false);
                Append($"✔️ {list.Length} packages installed:");
                foreach (var p in list)
                    Append($"  • {p.name}@{p.version}");
                
            }
            catch (Exception e)
            {
                Append($"❌ Error: {e.Message}");
            }
        }

        private async void SearchRemote()
        {
            var pk = _packageName.Trim();

            Append($"⏳ Searching remote for '{pk}'...");
            try
            {
                var list = await UPMClientWrapper.SearchRemoteAsync(pk);
                Append($"✔️ {list.Length} matches:");
                foreach (var p in list)
                    Append($"  • {p.name}@{p.version}");
                
            }
            catch (Exception e)
            {
                Append($"❌ Error: {e.Message}");
            }
        }

        private async void AddPackage()
        {
            var id = _identifier.Trim();

            Append($"⏳ Adding '{id}'...");
            try
            {
                var pkgId = await UPMClientWrapper.AddPackageAsync(id);
                Append($"✔️ Added: {pkgId}");
                
            }
            catch (Exception e)
            {
                Append($"❌ Error: {e.Message}");
            }
        }

        private async void RemovePackage()
        {
            var pk = _packageName.Trim();

            Append($"⏳ Removing '{pk}'...");
            try
            {
                var name = await UPMClientWrapper.RemovePackageAsync(pk);
                Append($"✔️ Removed: {name}");
                
            }
            catch (Exception e)
            {
                Append($"❌ Error: {e.Message}");
            }
        }

        private async void EmbedPackage()
        {
            var pk = _packageName.Trim();

            Append($"⏳ Embedding '{pk}'...");
            try
            {
                var name = await UPMClientWrapper.EmbedPackageAsync(pk);
                Append($"✔️ Embedded: {name}");
                
            }
            catch (Exception e)
            {
                Append($"❌ Error: {e.Message}");
            }
        }

        private async void GetDependencies()
        {
            var pk = _packageName.Trim();

            Append($"⏳ Fetching dependencies of '{pk}'...");
            try
            {
                var deps = await UPMClientWrapper.GetDependenciesAsync(pk, recursive: true);
                Append($"✔️ {deps.Length} dependencies:");
                foreach (var d in deps)
                    Append($"  • {d.name}@{d.version}");
                
            }
            catch (Exception e)
            {
                Append($"❌ Error: {e.Message}");
            }
        }

        private async void CheckIsInstalled()
        {
            var id = _identifier.Trim();

            Append($"⏳ Checking if package '{id}' is installed...");
            try
            {
                var ins = await UPMClientWrapper.IsPackageInstalledAsync(id);
                Append($"✔️ Installed: {ins}");
                
            }
            catch (Exception e)
            {
                Append($"❌ Error: {e.Message}");
            }
        }
    }
}
#endif
