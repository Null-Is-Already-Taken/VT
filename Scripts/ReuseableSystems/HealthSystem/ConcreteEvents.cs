using UnityEngine;
using VT.ReusableSystems.Events;

namespace VT.ReusableSystems.HealthSystem
{
    public struct DamageTakenEvent : IEvent
    {
        public GameObject Target;
        public float Amount;
    }

    public struct HealReceivedEvent : IEvent
    {
        public GameObject Target;
        public float Amount;
    }

    public struct DeathEvent : IEvent
    {
        public GameObject Target;
    }
}
