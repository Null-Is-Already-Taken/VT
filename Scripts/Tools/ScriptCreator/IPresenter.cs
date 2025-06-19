using System;

public interface IEditorPresenter<IEditorModel, IEditorView> : IDisposable
{
    void Init();
    void RegisterEvents();
    void UnregisterEvents();
    void RefreshView();
    void AttachView();
}

public interface IEditorView
{
    void OnGUI();
    void OnEnable();
    void OnDisable();
}

public interface IEditorModel<TData>
{
    TData Data { get; }
    TData Load();
    void Save(string path, TData data, bool silentOverwrite);
    void SetData(TData data);
}