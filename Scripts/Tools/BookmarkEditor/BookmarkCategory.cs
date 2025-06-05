using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

namespace VT.Tools.BookmarkEditor
{
    public struct BookmarkCategory
    {
        #region Fields
        public List<GameObject> BookmarkedObjects;
        public bool IsExpanded;
        #endregion

        #region Constructor
        public BookmarkCategory(bool isExpanded)
        {
            BookmarkedObjects = new();
            IsExpanded = isExpanded;
        }
        #endregion

        #region Methods
        public void AddBookmark(GameObject obj)
        {
            if (BookmarkedObjects == null)
            {
                BookmarkedObjects = new();
            }

            if (!BookmarkedObjects.Contains(obj))
            {
                BookmarkedObjects.Add(obj);
            }
        }

        public void SetExpanded(bool expanded)
        {
            IsExpanded = expanded;
        }

        public readonly bool HasBookmarks()
        {
            return !BookmarkedObjects.IsNullOrEmpty();
        }
        #endregion
    }
}
