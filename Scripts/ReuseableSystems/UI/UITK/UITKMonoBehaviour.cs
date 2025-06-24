using UnityEngine;
using UnityEngine.UIElements;

namespace VT.ReusableSystems.UI.UITK
{
    public abstract class UITKMonoBehaviour<TConstants> : MonoBehaviour where TConstants : class, new()
    {
        [SerializeField] protected UIDocument uiDocument;

        private void Awake()
        {
            uiDocument ??= GetComponent<UIDocument>();
        }

        protected VisualElement Root => uiDocument.rootVisualElement;

        protected T Get<T>(string name) where T : VisualElement
        {
            return Root.Q<T>(name);
        }

        protected T Get<T>(string name, string className) where T : VisualElement
        {
            return Root.Q<T>(name, className);
        }

        protected TConstants constants;
        protected TConstants Constants
        {
            get
            {
                if (constants == null)
                {
                    constants = new();
                }

                return constants;
            }
        }
    }
}