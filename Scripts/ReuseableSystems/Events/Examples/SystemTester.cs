using Sirenix.OdinInspector;
using UnityEngine;

namespace VT.ReusableSystems.Events.Examples
{
    public class SystemTester : MonoBehaviour
    {
        EventBinding<SystemEvent> sysBinding;

        public Sprite sampleIcon;

        private void OnEnable()
        {
            EventBus<SystemEvent>.BindAndRegister(sysBinding, HandleSystemEvent, this);
        }

        private void OnDisable()
        {
            EventBus<SystemEvent>.Deregister(sysBinding);
        }

        [Button]
        private void RaiseSystemEvent()
        {
            EventBus<SystemEvent>.Raise(new SystemEvent
            {
                systemName = "SaveSystem",
                action = "SaveCompleted",
                data = sampleIcon
            });
        }

        private void HandleSystemEvent(SystemEvent e)
        {
            Debug.Log($"[SystemTester] SystemEvent — System: {e.systemName}, Action: {e.action}, Sprite: {(e.data ? e.data.name : "null")}");
        }
    }
}
