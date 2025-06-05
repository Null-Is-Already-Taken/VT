using System;
using UnityEngine.UIElements;
using VT.Patterns.SingletonPattern;

namespace VT.ReuseableSystems.UI.UITK
{
    public abstract class UITKManager : Singleton<UITKManager>
    {
        // Could hold global layout roots, active documents, etc.

        // Generic access to any constants + Q
        protected TElement Get<TConstants, TElement>(string name)
            where TElement : VisualElement
            where TConstants : class
        {
            var root = GetRootFor<TConstants>();
            return root.Q<TElement>(name);
        }

        protected virtual VisualElement GetRootFor<TConstants>() where TConstants : class
        {
            // Look up which UIDocument (or VisualElement) corresponds to this constants class
            // Could be from a dictionary you manage:
            // e.g., constantsType => UIDocument.rootVisualElement
            throw new NotImplementedException("You need to register or map constants → root.");
        }
    }
}
