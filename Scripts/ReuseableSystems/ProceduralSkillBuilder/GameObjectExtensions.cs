// --- GameObjectExtensions.cs ---
using System;
using UnityEngine;
using VT.ReusableSystems.Events;
using VT.ReusableSystems.HealthSystem;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    public static class GameObjectExtensions
    {
        public static void ApplyStatus(this GameObject go, string status, float duration)
        {
            if (go.TryGetComponent<IStatusReceiver>(out var receiver))
            {
                receiver.ApplyStatus(status, duration);
            }
        }

        public static void OnDeath(
            this GameObject go,
            Action callback,
            MonoBehaviour owner,
            bool noDuplicate = false,
            ExecutionContext context = null)
        {
            if (noDuplicate)
            {
                string skillKey = context?.Get<string>(ContextKeys.SkillName, "default-skill") ?? "default-skill";
                string key = $"{skillKey}.{callback.Method.Name}";
                var tracker = go.GetComponent<EventBindingTracker>() ?? go.AddComponent<EventBindingTracker>();
                if (!tracker.TryRegisterKey(key))
                    return;
            }

            EventBus<DeathEvent>.BindAndRegister(null, evt =>
            {
                if (evt.Target == go)
                    callback?.Invoke();
            }, owner);
        }
    }
}
