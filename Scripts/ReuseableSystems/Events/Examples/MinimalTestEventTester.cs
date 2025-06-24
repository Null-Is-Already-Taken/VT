using Sirenix.OdinInspector;
using UnityEngine;

namespace VT.ReusableSystems.Events.Examples
{
    public class MinimalTestEventTester : MonoBehaviour
    {
        EventBinding<TestEvent> testBinding;

        private void OnEnable()
        {
            EventBus<TestEvent>.BindAndRegister(testBinding, HandleTestEvent, this);
        }

        private void OnDisable()
        {
            EventBus<TestEvent>.Deregister(testBinding);
        }

        [Button]
        private void RaiseTestEvent()
        {
            EventBus<TestEvent>.Raise(new TestEvent());
        }

        private void HandleTestEvent(TestEvent e)
        {
            Debug.Log("[MinimalTestEventTester] TestEvent received");
        }
    }
}
