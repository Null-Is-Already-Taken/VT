#if UNITY_EDITOR && ODIN_INSPECTOR

using System;
using UnityEditor;
using VT.IO;
using VT.Logger;

namespace VT.Tools.ScriptCreator
{
    public class ScriptCreatorModel
    {
        #region Constants


        private ScriptData data;
        public ScriptData Data => data;

        #endregion

        #region Public Entry Point

        public void SetData(ScriptData data)
        {
            this.data = data;
        }

        public void Save(string path, ScriptData data)
        {
            path = IOManager.NormalizePath(path);

            if (string.IsNullOrWhiteSpace(path))
            {
                InternalLogger.Instance.LogError("[Model] File path cannot be empty.");
                return;
            }

            try
            {
                IOManager.WriteAllText(path, data.Content);
                InternalLogger.Instance.LogDebug($"[Model] Script {data.ClassName} saved at: {path}");
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"[Model] Error saving script: {ex.Message}");
            }
        }

        #endregion
    }
}

#endif
