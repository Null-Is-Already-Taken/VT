#if UNITY_EDITOR && ODIN_INSPECTOR

using System;
using UnityEditor;
using UnityEngine;
using VT.Editor.GUI;
using VT.Editor.Utils;

namespace VT.Tools.ScriptCreator
{
    public class ScriptCreatorView : EditorWindow, IEditorView
    {
        public event Action OnScriptContentChangedEvent;
        public event Action<string, string> OnSaveScriptEvent;

        public void OnEnable()
        {
            var scriptCreatorModel = new ScriptCreatorModel();
        }

        public void OnDisable()
        {
            // Cleanup if needed
        }

        public void OnGUI()
        {
            Label.Draw(
                content: new("Script Creator"),
                style: LabelStyles.BoldLabel,
                options: GUILayout.ExpandWidth(true)
            );

            EditorGUILayout.Space(spacing);

            using (new EditorGUILayout.HorizontalScope())
            {
                Label.Draw(
                    content: new("Script Name"),
                    style: LabelStyles.Label,
                    options: GUILayout.Width(80)
                );

                TextField.Draw(
                    label: string.Empty,
                    value: "",
                    style: EditorStyles.textField,
                    options: GUILayout.ExpandWidth(true)
                );
            }

            //using (new EditorGUILayout.HorizontalScope())
            //{
            //    Label.Draw(
            //        content: new("Path"),
            //        style: LabelStyles.Label,
            //        options: GUILayout.Width(80)
            //    );

            //    TextFields.Draw(
            //        label: string.Empty,
            //        value: "",
            //        style: EditorStyles.textField,
            //        options: GUILayout.ExpandWidth(true)
            //    );

            //    Button.Draw(
            //        content: new (EmbeddedIcons.OpenFolder_Unicode),
            //        backgroundColor: Color.white,
            //        onClick: () => {
            //            string absolutePath = EditorUtility.OpenFolderPanel(
            //                title: "Select Save Path",
            //                folder: PathUtils.GetProjectPath(),
            //                defaultName: ""
            //            );
            //        },
            //        style: ButtonStyles.Inline
            //    );
            //}

            PathPicker.Draw(
                label: "Save Path",
                currentPath: ref path,
                filter: PathType.Folder
            );
        }

        //private void SaveScript(string content)
        //{
        //    if (string.IsNullOrWhiteSpace(scriptName) || string.IsNullOrWhiteSpace(folderPath) || string.IsNullOrWhiteSpace(scriptContent))
        //    {
        //        EditorUtility.DisplayDialog
        //        (
        //            title: "Error",
        //            message: "Please fill in all fields before saving.",
        //            ok: "OK"
        //        );

        //        return;
        //    }

        //    // Invoke the save event
        //    OnSaveScriptEvent?.Invoke(folderPath, scriptContent);
        //}

        //[Button(ButtonSizes.Large)]
        //[GUIColor(0.3f, 0.8f, 0.3f)]
        //public void SaveScript()
        //{
        //    OnSaveScriptEvent?.Invoke(folderPath, scriptContent);
        //}

        [MenuItem("Tools/Script Creator")]
        private static void OpenWindow()
        {
            GetWindow<ScriptCreatorView>().Show();
        }

        private const int spacing = 8;
        private string path = string.Empty;
    }
}

#endif
