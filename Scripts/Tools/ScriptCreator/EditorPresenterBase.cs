//using UnityEngine;

//namespace VT.Tools.ScriptCreator
//{
//    public abstract class EditorPresenterBase : IEditorPresenter
//    {
//        public IEditorModel<TData> Model { get; private set; }
//        public IEditorView View { get; private set; }
        
//        public EditorPresenterBase(IEditorModel<TData> model, IEditorView view)
//        {
//            Model = model;
//            View = view;
//        }

//        public void Init()
//        {
//            AttachView();
//            RegisterEvents();
//        }
        
//        public void RegisterEvents()
//        {
//            // Register any events here
//        }
        
//        public void UnregisterEvents()
//        {
//            // Unregister any events here
//        }
        
//        public void RefreshView()
//        {
//        }
        
//        public void AttachView()
//        {
//        }
        
//        public void Dispose()
//        {
//            UnregisterEvents();
//        }
//    }
//}
