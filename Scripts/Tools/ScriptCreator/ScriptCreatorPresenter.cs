using VT.IO;
using VT.Logger;
using static VT.Tools.ScriptCreator.ScriptCreatorIOService;

namespace VT.Tools.ScriptCreator
{
    public class ScriptCreatorPresenter
    {
        private readonly ScriptCreatorModel model;
        private readonly ScriptCreatorView view;

        private bool hasInit = false;
        private bool isDisposed = false;
        private bool silentOverwrite = false; // Flag to control silent overwrite behavior

        public ScriptCreatorPresenter(ScriptCreatorModel model, ScriptCreatorView view)
        {
            this.model = model;
            this.view = view;
        }
        
        public void Init()
        {
            if (!hasInit)
            {
                RegisterEvents();
                model.SetData(view.GetData());
                hasInit = true;
            }
        }
        
        public void RegisterEvents()
        {
            // Register any events here if needed
            view.OnScriptContentChangedEvent += OnScriptContentChanged;
            view.OnSaveScriptEvent += OnSaveScript;
        }

        private void OnSaveScript(string path, string scriptContent)
        {
            InternalLogger.Instance.LogDebug("[Presenter] Saving script...");
            var filePath = GetPath(model.Data, path);
            InternalLogger.Instance.LogDebug($"[Presenter] File path: {filePath}");

            if (HandlePathError(filePath) == false)
            {
                return;
            }

            model.Save(filePath, ScriptDataFactory.FromContent(scriptContent));
        }

        private bool HandlePathError(string path)
        {
            var error = ValidatePath(path);

            switch (error)
            {
                case PathError.InvalidPath:
                    InternalLogger.Instance.LogError("[Presenter] Invalid file path.");
                    return false;
                case PathError.FileAlreadyExists:
                    if (silentOverwrite)
                        return true; // If silent overwrite is enabled, skip confirmation
                    return view.AskForOverwriteConfirmation(model.Data.ClassName, IOManager.GetDirectoryName(path));
                case PathError.DirectoryNotFound:
                    InternalLogger.Instance.LogError("[Presenter] Directory not found.");
                    return false;
                case PathError.AccessDenied:
                    InternalLogger.Instance.LogError("[Presenter] Access denied to the specified path.");
                    return false;
                case PathError.None:
                    return true;
            }

            return false;
        }

        private void OnScriptContentChanged(string scriptContent)
        {
            var scriptData = ScriptDataFactory.ProcessScriptContent(scriptContent);
            model.SetData(scriptData);
            view.SetData(scriptData);
        }

        public void UnregisterEvents()
        {
            // Unregister any events here if needed
            view.OnScriptContentChangedEvent -= OnScriptContentChanged;
            view.OnSaveScriptEvent -= OnSaveScript;
        }
        
        public void Dispose()
        {
            if (!isDisposed)
            {
                UnregisterEvents();
                isDisposed = true;
            }
        }
    }
}
