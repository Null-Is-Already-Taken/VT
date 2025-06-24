using UnityEngine;

namespace VT.ReusableSystems.Events
{
    public struct TestEvent : IEvent
    {
    }

    public struct PlayerEvent : IEvent
    {
        public int health;
        public int mana;
    }

    public struct EnemyEvent : IEvent
    {
        public int damage;
        public string enemyType;
    }

    public struct GameEvent : IEvent
    {
        public string eventName;
        public float eventTime;
    }

    public struct UIEvent : IEvent
    {
        public string uiElement;
        public string action;
    }

    public struct SystemEvent : IEvent
    {
        public string systemName;
        public string action;
        public Sprite data; // Can be any type of data relevant to the system event
    }
}
