using UnityEngine;

namespace VT.Tools.ScriptCreator
{
    public class ScriptCreatorPresenter : IEditorPresenter<ScriptCreatorModel, ScriptCreatorView>
    {
        private readonly ScriptCreatorModel _model;
        private readonly ScriptCreatorView _view;
        
        public ScriptCreatorPresenter(ScriptCreatorModel model, ScriptCreatorView view)
        {
            _model = model;
            _view = view;
        }
        
        public void Init()
        {
            AttachView();
            RegisterEvents();
        }
        
        public void RegisterEvents()
        {
            // Register any events here if needed
        }
        
        public void UnregisterEvents()
        {
            // Unregister any events here if needed
        }
        
        public void RefreshView()
        {
        }
        
        public void AttachView()
        {
        }
        
        public void Dispose()
        {
            UnregisterEvents();
        }
    }
}
