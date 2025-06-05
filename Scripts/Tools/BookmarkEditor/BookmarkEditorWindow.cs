using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace VT.Tools.BookmarkEditor
{
    public class BookmarkEditorWindow : EditorWindow
    {
        #region Fields
        private const int GUI_BUTTON_HEIGHT = 20;
        private static BookmarkEditorWindow instance;
        public Dictionary<string, BookmarkCategory> BookmarkedObjects { get; } = new()
        {
            { "Default", new BookmarkCategory(true) }
        };
        #endregion

        #region Singleton
        public static BookmarkEditorWindow Instance
        {
            get
            {
                if (!instance)
                {
                    instance = GetWindow<BookmarkEditorWindow>();
                }
                return instance;
            }
        }
        #endregion

        #region Menu Items
        [MenuItem("GameObject/Bookmark #%t", false, 0)]
        private static void BookmarkGameObject()
        {
            if (Selection.activeGameObject != null)
            {
                CategorySelectionWindow.ShowWindow(Selection.activeGameObject);
            }
        }

        [MenuItem("Window/Bookmark Editor")]
        public static void ShowWindow()
        {
            GetWindow<BookmarkEditorWindow>("Bookmark Editor").Show();
        }
        #endregion

        #region GUI Methods
        private void OnGUI()
        {
            if (!HasBookmarkedObjects())
            {
                EditorGUILayout.HelpBox("No tracked objects yet. Right-click a GameObject in the hierarchy and select 'Track'.", MessageType.Info);
            }

            List<string> categories = new(BookmarkedObjects.Keys);

            foreach (var category in categories)
            {
                if (BookmarkedObjects[category].BookmarkedObjects.Count < 1)
                    continue;

                DrawCategoryHeader(category, out bool isCategoryModified);

                if (!isCategoryModified && BookmarkedObjects[category].IsExpanded)
                {
                    DrawCategoryContent(category);
                }

                EditorGUILayout.Space();
            }

            if (HasBookmarkedObjects())
            {
                if (GUILayout.Button("Clear All", GUILayout.Height(GUI_BUTTON_HEIGHT)))
                {
                    ClearAllBookmarks();
                }
            }
        }
        #endregion

        #region Category Handling
        private void DrawCategoryContent(string category)
        {
            var objects = BookmarkedObjects[category];

            for (int i = objects.BookmarkedObjects.Count - 1; i >= 0; i--)
            {
                GameObject obj = objects.BookmarkedObjects[i];

                if (obj == null)
                {
                    objects.BookmarkedObjects.RemoveAt(i);
                    continue;
                }

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button(obj.name, GUILayout.Height(GUI_BUTTON_HEIGHT)))
                {
                    PingAndSelect(obj);
                }

                if (GUILayout.Button("X", GUILayout.Width(GUI_BUTTON_HEIGHT), GUILayout.Height(GUI_BUTTON_HEIGHT)))
                {
                    objects.BookmarkedObjects.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawCategoryHeader(string category, out bool modifiedCategory)
        {
            modifiedCategory = false;

            EditorGUILayout.BeginHorizontal();
            ToggleCategoryFoldout(category);

            if (GUILayout.Button("Clear", GUILayout.Width(60)))
            {
                ClearCategoryObjects(category);
            }

            if (category != "Default" && GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                RemoveCategory(category);
                modifiedCategory = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ToggleCategoryFoldout(string category)
        {
            string content = $"{category} ({BookmarkedObjects[category].BookmarkedObjects.Count})";
            var entry = BookmarkedObjects[category];
            entry.SetExpanded(EditorGUILayout.Foldout(entry.IsExpanded, content, true, EditorStyles.foldoutHeader));
            BookmarkedObjects[category] = entry;
        }
        #endregion

        #region Utility Methods
        private bool HasBookmarkedObjects()
        {
            foreach (var obj in BookmarkedObjects)
            {
                if (obj.Value.HasBookmarks())
                {
                    return true;
                }
            }
            return false;
        }

        private void PingAndSelect(GameObject obj)
        {
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }

        private void ClearAllBookmarks()
        {
            foreach (var obj in BookmarkedObjects)
            {
                obj.Value.BookmarkedObjects.Clear();
            }
            Repaint();
        }
        #endregion

        #region Static Methods
        public static void AddTrackedObject(GameObject obj, string category)
        {
            if (!Instance.BookmarkedObjects.ContainsKey(category))
            {
                Instance.BookmarkedObjects[category] = new BookmarkCategory(true);
            }

            Instance.BookmarkedObjects[category].AddBookmark(obj);
            Instance.Show();
        }

        public static void AddCategory(string category)
        {
            if (!Instance.BookmarkedObjects.ContainsKey(category))
            {
                Instance.BookmarkedObjects[category] = new BookmarkCategory(true);
            }
        }

        public static void RemoveCategory(string category)
        {
            if (Instance.BookmarkedObjects.ContainsKey(category) && category != "Default")
            {
                Instance.BookmarkedObjects.Remove(category);
            }
            Instance.Repaint();
        }

        public static void ClearCategoryObjects(string category)
        {
            if (Instance.BookmarkedObjects.ContainsKey(category))
            {
                Instance.BookmarkedObjects[category].BookmarkedObjects.Clear();
            }
            Instance.Repaint();
        }
        #endregion
    }
}
#endif
