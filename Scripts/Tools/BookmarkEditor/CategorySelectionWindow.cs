using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
namespace VT.Tools.BookmarkEditor
{
    public class CategorySelectionWindow : EditorWindow
    {
        #region Enums
        private enum SelectionProcess
        {
            Default,
            AddCategory,
            RemoveCategory
        }
        #endregion

        #region Fields
        private static GameObject selectedObject;
        private static BookmarkEditorWindow bookmarkEditor;
        private static string categoryInputName = "";
        private static int selectedCategoryIndex = 0;
        private static SelectionProcess selectionProcess;
        #endregion

        #region Window Handling
        public static void ShowWindow(GameObject obj)
        {
            selectedObject = obj;
            bookmarkEditor = GetWindow<BookmarkEditorWindow>();
            CategorySelectionWindow window = GetWindow<CategorySelectionWindow>("Select Category");
            window.ShowUtility();
        }
        #endregion

        #region GUI Methods
        private void OnGUI()
        {
            var categories = bookmarkEditor.BookmarkedObjects.Keys;

            switch (selectionProcess)
            {
                case SelectionProcess.Default:
                    DrawDefaultLayout(categories);
                    break;
                case SelectionProcess.AddCategory:
                    DrawAddCategoryLayout(categories);
                    break;
                case SelectionProcess.RemoveCategory:
                    DrawRemoveCategoryLayout(categories);
                    break;
            }
        }
        #endregion

        #region Layout Methods
        private void DrawDefaultLayout(IEnumerable<string> categories)
        {
            EditorGUILayout.LabelField($"Selected: {selectedObject.name}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Choose a category:", EditorStyles.boldLabel);

            var selectedIndex = selectedCategoryIndex < categories.Count() ? selectedCategoryIndex : 0;
            selectedCategoryIndex = EditorGUILayout.Popup("Category", selectedIndex, categories.ToArray());

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add New Category"))
            {
                selectionProcess = SelectionProcess.AddCategory;
            }

            if (categories.Count() > 1 && GUILayout.Button("Remove Category"))
            {
                selectionProcess = SelectionProcess.RemoveCategory;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("OK"))
            {
                BookmarkEditorWindow.AddTrackedObject(selectedObject, categories.ElementAt(selectedCategoryIndex));
                Close();
            }
        }

        private void DrawAddCategoryLayout(IEnumerable<string> categories)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("New Category Name:");
            categoryInputName = EditorGUILayout.TextField(categoryInputName);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                if (!string.IsNullOrEmpty(categoryInputName))
                {
                    BookmarkEditorWindow.AddCategory(categoryInputName);
                    BookmarkEditorWindow.AddTrackedObject(selectedObject, categoryInputName);
                    selectedCategoryIndex = categories.Count() - 1;
                    selectionProcess = SelectionProcess.Default;
                    Close();
                }
            }

            if (GUILayout.Button("Cancel"))
            {
                selectionProcess = SelectionProcess.Default;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawRemoveCategoryLayout(IEnumerable<string> categories)
        {
            List<string> filteredCategories = categories.Skip(1).ToList();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Select Category to Remove:");
            var selectedIndex = selectedCategoryIndex >= 1 ? selectedCategoryIndex - 1 : 0;
            selectedCategoryIndex = EditorGUILayout.Popup(selectedIndex, filteredCategories.ToArray()) + 1;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove"))
            {
                if (categories.ElementAt(selectedCategoryIndex) != "Default")
                {
                    BookmarkEditorWindow.RemoveCategory(categories.ElementAt(selectedCategoryIndex));
                    selectionProcess = SelectionProcess.Default;
                    selectedCategoryIndex = 0;
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "The 'Default' category cannot be removed.", "OK");
                }
            }

            if (GUILayout.Button("Cancel"))
            {
                selectionProcess = SelectionProcess.Default;
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion
    }
}
#endif
