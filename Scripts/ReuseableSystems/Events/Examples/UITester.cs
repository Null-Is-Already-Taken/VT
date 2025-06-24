using Sirenix.OdinInspector;
using UnityEngine;

namespace VT.ReusableSystems.Events.Examples
{
    public class UITester : MonoBehaviour
    {
        EventBinding<UIEvent> uiBinding;

        private void OnEnable()
        {
            EventBus<UIEvent>.BindAndRegister(uiBinding, HandleUIEvent, this);
        }

        private void OnDisable()
        {
            EventBus<UIEvent>.Deregister(uiBinding);
        }

        [Button]
        private void RaiseUIEvent()
        {
            EventBus<UIEvent>.Raise(new UIEvent
            {
                uiElement = "StartButton",
                action = "Clicked"
            });
        }

        private void HandleUIEvent(UIEvent e)
        {
            Debug.Log($"[UITester] UIEvent — UI Element: {e.uiElement}, Action: {e.action}");
        }
    }
}
