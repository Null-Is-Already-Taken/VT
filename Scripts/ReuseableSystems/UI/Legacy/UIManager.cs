using UnityEngine;
using System.Collections.Generic;
using VT.Patterns.SingletonPattern;
using System;

namespace VT.ReuseableSystems.UI.Legacy
{
    public abstract class UIManager<TManager> : Singleton<TManager>
        where TManager : UIManager<TManager>
    {

        [Header("Pages")]
        [SerializeField] private List<UIPage> uiPages = new();
        [SerializeField] private UIPage defaultPage;

        [Header("Settings")]
        [SerializeField] private float transitionDuration = 0.5f;

        protected UIPage currentPage;
        private Stack<UIPage> backStack = new();
        private Stack<UIPage> forwardStack = new();

        public bool CanGoBack() => backStack.Count > 0;
        public bool CanRedo() => forwardStack.Count > 0;

        public event Action<UIPage, UIPage> OnPageChanged;

        protected virtual void Start()
        {
            foreach (var entry in uiPages)
            {
                entry.Initialize
                (
                    page => ShowPage(page),
                    type => EffectResolver(type)
                );
            }

            if (defaultPage != null)
                ShowPage(defaultPage);
        }

        private IUITransitionEffect EffectResolver(UITransitionType type)
            => TransitionEffects.Effects.TryGetValue(type, out var effect) ? effect : null;

        public void Reset()
        {
            currentPage?.HidePage();
            currentPage = null;
            backStack.Clear();
            forwardStack.Clear();
            if (defaultPage != null)
            {
                OnPageChanged?.Invoke(currentPage, defaultPage);
                ShowPage(defaultPage);
            }
        }

        public virtual UIPage ShowPage(UIPage page, bool savePage = true)
        {
            if (currentPage == page || page == null) return currentPage;

            if (savePage && currentPage != null)
            {
                PushStack(backStack, currentPage);
                forwardStack.Clear();
            }

            if (currentPage != null)
            {
                currentPage.HidePage();
            }

            var previousPage = currentPage;
            currentPage = page;

            if (currentPage != null)
            {
                currentPage.ShowPage();
            }

            OnPageChanged?.Invoke(previousPage, currentPage);

            return currentPage;
        }

        public virtual void HidePage(UIPage page)
        {
            if (page == null) return;

            page.HidePage();
            if (currentPage == page)
                currentPage = null;
        }

        public UIPage ShowPreviousPage()
        {
            if (!CanGoBack()) return currentPage;

            if (currentPage != null)
                PushStack(forwardStack, currentPage);

            UIPage previousPage = backStack.Pop();
            return ShowPage(previousPage, savePage: false);
        }

        public UIPage RedoNextPage()
        {
            if (!CanRedo()) return currentPage;

            if (currentPage != null)
                backStack.Push(currentPage);

            UIPage nextPage = forwardStack.Pop();
            return ShowPage(nextPage, savePage: false);
        }

        public UIPage GetPageByName(string pageName)
        {
            var page = uiPages.Find(entry => entry.Name == pageName);
#if UNITY_EDITOR
            if (page == null)
                Debug.LogWarning($"[UIManager] Page with name '{pageName}' not found.");
#endif
            return page;
        }

        private readonly int maxHistory = 20;

        private void TrimStack(Stack<UIPage> stack)
        {
            while (stack.Count > maxHistory)
            {
                var temp = new Stack<UIPage>(stack);
                temp.Pop(); // Drop the oldest
                stack.Clear();
                foreach (var p in temp) stack.Push(p);
            }
        }

        private void PushStack(Stack<UIPage> stack, UIPage page)
        {
            backStack.Push(page);
            TrimStack(backStack);
        }
    }
}
