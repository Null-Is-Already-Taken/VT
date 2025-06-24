using Sirenix.OdinInspector;
using UnityEngine;

namespace VT.ReusableSystems.Events.Examples
{
    public class EventBusTester : MonoBehaviour
    {
        EventBinding<PlayerEvent> playerEventBinding;
        EventBinding<EnemyEvent> enemyEventBinding;
        EventBinding<SystemEvent> systemEventBinding;
        EventBinding<GameEvent> gameEventBinding;

        private void OnEnable()
        {
            EventBus<PlayerEvent>.BindAndRegister(playerEventBinding, HandlePlayerEvent, this);
            EventBus<EnemyEvent>.BindAndRegister(enemyEventBinding, HandleEnemyEvent, this);
            EventBus<SystemEvent>.BindAndRegister(systemEventBinding, HandleSystemEvent, this);
        }

        [Button]
        private void TestAddBindings()
        {
            EventBus<GameEvent>.BindAndRegister(gameEventBinding, e => Debug.Log($"[Tester] GameEvent raised — {e}"), this);
        }

        private void OnDisable()
        {
            // Deregister when disabled
            EventBus<PlayerEvent>.Deregister(playerEventBinding);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                EventBus<PlayerEvent>.Raise(new PlayerEvent
                {
                    health = UnityEngine.Random.Range(0, 100),
                    mana = UnityEngine.Random.Range(0, 100)
                });
                EventBus<EnemyEvent>.Raise(new EnemyEvent
                {
                    damage = UnityEngine.Random.Range(0, 100),
                    enemyType = UnityEngine.Random.Range(0, 100).ToString()
                });
                EventBus<SystemEvent>.Raise(new SystemEvent
                {
                    systemName = "Huh",
                    action = "Nope",
                    data = null // Replace with actual data if needed
                });
                EventBus<GameEvent>.Raise(new GameEvent
                {
                    eventName = "TestEvent",
                    eventTime = Time.time
                });
            }
        }

        private void HandlePlayerEvent(PlayerEvent e)
        {
            Debug.Log($"[Tester] PlayerEvent raised — Health: {e.health}, Mana: {e.mana}");
        }

        private void HandleEnemyEvent(EnemyEvent e)
        {
            Debug.Log($"[Tester] EnemyEvent raised — Damage: {e.damage}, EnemyType: {e.enemyType}");
        }

        private void HandleSystemEvent(SystemEvent e)
        {
            Debug.Log($"[Tester] EnemyEvent raised — System Name: {e.systemName}, Action: {e.action}, Data: {e.data}");
        }
    }
}
