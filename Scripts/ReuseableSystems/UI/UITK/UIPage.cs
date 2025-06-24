using System;
using UnityEngine.UIElements;

namespace VT.ReusableSystems.UI.UITK
{
    public class UIPage
    {
        public string Name { get; }
        public VisualElement Root { get; }
        public UITransitionType TransitionType { get; }

        private Func<UITransitionType, IUITransitionEffect> effectResolver;

        public UIPage(string name, VisualElement root, UITransitionType type,
                    Func<UITransitionType, IUITransitionEffect> resolver)
        {
            Name = name;
            Root = root;
            TransitionType = type;
            effectResolver = resolver;
        }

        public void Show()
        {
            effectResolver(TransitionType)?.Play(Root);
        }

        public void Hide()
        {
            effectResolver(TransitionType)?.Rewind(Root);
        }
    }
}