#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using VT.Editor.GUI;
using VT.Logger;

namespace VT.Tools.ScriptCreator
{
    public class ScriptCreatorView : EditorWindow
    {
        public event Action<string> OnScriptContentChangedEvent;
        public event Action<string, string> OnSaveScriptEvent;
        public event Action OnRefreshEvent;

        public ScriptData Data => ScriptDataFactory.FromContent(scriptContent);

        private ScriptCreatorModel scriptCreatorModel;
        private ScriptCreatorPresenter scriptCreatorPresenter;

        public void OnEnable()
        {
            scriptCreatorModel ??= new ScriptCreatorModel();
            scriptCreatorPresenter ??= new ScriptCreatorPresenter(scriptCreatorModel, this);
            scriptCreatorPresenter.Init();
        }

        public void OnDisable()
        {
            // Cleanup if needed
            scriptCreatorPresenter?.Dispose();
        }

        private const float MinLayoutWidth = 300f;

        public void OnGUI()
        {
            float currentWidth = position.width;

            if (currentWidth < MinLayoutWidth)
            {
                EditorGUILayout.HelpBox($"Minimum recommended width is {MinLayoutWidth}px. Expand the window for full layout.", MessageType.Warning);
                GUI.enabled = false;
            }

            using var sv = new EditorGUILayout.ScrollViewScope(editorWindowScroll);
            editorWindowScroll = sv.scrollPosition;

            Label.Draw(
                content: new("Script Creator"),
                style: LabelStyles.BoldLabel,
                options: GUILayout.ExpandWidth(true)
            );

            using (new EditorGUILayout.VerticalScope("helpBox"))
            {
                EditorGUILayout.Space(spacing);

                using (new EditorGUILayout.HorizontalScope())
                {
                    Label.Draw(
                        content: new("Script Name"),
                        style: LabelStyles.Label,
                        options: GUILayout.Width(80)
                    );

                    Label.Draw(
                        content: new(className),
                        style: LabelStyles.Label,
                        options: GUILayout.ExpandWidth(true)
                    );
                }

                path = PathPicker.Draw(
                    label: new("Save Path"),
                    currentPath: path,
                    filter: PathType.Folder
                );

                EditorGUILayout.Space(spacing);
            }

            scriptContent = TextArea.Draw(
                label: null,
                value: scriptContent,
                scroll: ref textAreaScroll,
                onValueChanged: content =>
                {
                    InternalLogger.Instance.LogDebug("Script content changed.");
                    // Invoke the content changed event
                    OnScriptContentChangedEvent?.Invoke(content);
                },
                style: EditorStyles.textArea,
                height: EditorGUIUtility.singleLineHeight * scriptContentLines
            );

            if (string.IsNullOrWhiteSpace(scriptContent))
            {
                EditorGUILayout.HelpBox("Please enter the script content.", MessageType.Info);
            }
            else
            {
                Button.Draw(
                    content: new("Save Script"),
                    backgroundColor: Color.green,
                    onClick: () =>
                    {
                        // Invoke the save event
                        OnSaveScriptEvent?.Invoke(path, scriptContent);
                    },
                    style: ButtonStyles.BigButton
                );
            }

            Button.Draw(
                content: new("Test"),
                backgroundColor: Color.white,
                onClick: () =>
                {
                    //var instanceID = Selection.activeInstanceID;
                    //InternalLogger.Instance.LogDebug($"Selected asset GUIDs: {instanceID}");
                    //string assetPath = AssetDatabase.GetAssetPath(instanceID);
                    //InternalLogger.Instance.LogDebug($"Selected asset path: {assetPath}");

                    var guids = Selection.assetGUIDs;

                    if (guids.Length == 0)
                    {
                        InternalLogger.Instance.LogWarning("No assets selected for testing.");
                        return;
                    }

                    for (int i = 0; i < guids.Length; i++)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                        InternalLogger.Instance.LogDebug($"Selected asset {i + 1}: {assetPath}");
                    }
                },
                style: ButtonStyles.BigButton
            );

            if (currentWidth < MinLayoutWidth)
                GUI.enabled = true;
        }

        public ScriptData GetData() => ScriptDataFactory.FromContent(scriptContent);

        public void SetData(ScriptData scriptData)
        {
            className = scriptData.ClassName;
            scriptContent = scriptData.Content;
        }

        public bool AskForOverwriteConfirmation(string scriptName, string folderPath)
        {
            return EditorUtility.DisplayDialog(
                title: "Overwrite Script?",
                message: $"“{scriptName}.cs” already exists at {folderPath}. Overwrite?",
                ok: "Yes",
                cancel: "No"
            );
        }

        [MenuItem("Tools/Script Creator")]
        private static void OpenWindow()
        {
           var window = GetWindow<ScriptCreatorView>();
            window.minSize = new Vector2(400, 300); // Minimum width and height
            window.Show();
        }

        private const int spacing = 4;
        private string path = string.Empty;
        private string className = "NewScript";
        private string scriptContent = string.Empty;
        private const int scriptContentLines = 15;

        private Vector2 editorWindowScroll;
        private Vector2 textAreaScroll;
    }
}

#endif
