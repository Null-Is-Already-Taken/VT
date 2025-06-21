#if UNITY_EDITOR

using System;
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
            if (string.IsNullOrWhiteSpace(path))
            {
                InternalLogger.Instance.LogError("[Model] File path cannot be empty.");
                return;
            }

            try
            {
                ScriptCreatorIOService.Save(path, data);
                InternalLogger.Instance.LogDebug($"[Model] Script {data.ClassName} saved at: {path}");
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"[Model] Error saving script: {ex.Message}");
                return;
            }
        }

        #endregion
    }
}

#endif
