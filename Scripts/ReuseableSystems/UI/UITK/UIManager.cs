using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VT.ReuseableSystems.UI.UITK
{
    public abstract class UIManager<T> where T : UIManager<T>
    {
        protected Dictionary<string, UIPage> pages = new();
        protected UIPage currentPage;
        protected Stack<UIPage> backStack = new();
        protected Stack<UIPage> forwardStack = new();

        public event Action<UIPage, UIPage> OnPageChanged;

        public virtual void RegisterPage(string name, VisualElement root, UITransitionType type)
        {
            pages[name] = new UIPage(name, root, type, ResolveEffect);
        }

        public virtual UIPage ShowPage(string name, bool save = true)
        {
            if (!pages.TryGetValue(name, out var nextPage) || nextPage == currentPage)
                return currentPage;

            if (save && currentPage != null)
                backStack.Push(currentPage);

            currentPage?.Hide();
            var previous = currentPage;
            currentPage = nextPage;
            currentPage.Show();

            forwardStack.Clear();
            OnPageChanged?.Invoke(previous, currentPage);

            return currentPage;
        }

        public virtual void HidePage(string name)
        {
            if (pages.TryGetValue(name, out var page))
                page.Hide();
        }

        public UIPage ShowPreviousPage()
        {
            if (backStack.Count == 0) return currentPage;

            if (currentPage != null)
                forwardStack.Push(currentPage);

            var previousPage = backStack.Pop();
            return ShowPage(previousPage.Name, save: false);
        }

        public UIPage RedoNextPage()
        {
            if (forwardStack.Count == 0) return currentPage;

            if (currentPage != null)
                backStack.Push(currentPage);

            var nextPage = forwardStack.Pop();
            return ShowPage(nextPage.Name, save: false);
        }

        protected virtual IUITransitionEffect ResolveEffect(UITransitionType type)
        {
            return type switch
            {
                UITransitionType.SlideLeftRight => new SlideTransitionEffect(from: new(-1000f, 0f), to: Vector2.zero, duration: 3f),
                UITransitionType.SlideRightLeft => new SlideTransitionEffect(from: new(1000f, 0f), to: Vector2.zero, duration: 3f),
                UITransitionType.SlideUpDown => new SlideTransitionEffect(from: new(0f, 1000f), to: Vector2.zero, duration: 3f),
                UITransitionType.SlideDownUp => new SlideTransitionEffect(from: new(0f, -1000f), to: Vector2.zero, duration: 3f),
                UITransitionType.FadeInOut => new FadeTransitionEffect(duration: 3f),
                _ => null
            };
        }
    }
}
