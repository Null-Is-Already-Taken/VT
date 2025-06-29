using UnityEngine;
using VT.ReusableSystems.Events;
using DamageNumbersPro;
using Sirenix.OdinInspector;

namespace VT.ReusableSystems.HealthSystem
{
    /// <summary>
    /// Example class that demonstrates how to register for health-related events.
    /// This class listens to health events from the entire system and can react accordingly.
    /// </summary>
    public class HealthEventMonitor : MonoBehaviour
    {
        [Header("Event Bindings")]
        [SerializeField] private bool logEvents = true;
        [SerializeField] private bool showDamageNumbers = true;

        [Header("Damage Numbers Pro Settings")]
        [SerializeField] private DamageNumber damagePopupPrefab;
        [SerializeField] private DamageNumber healPopupPrefab;
        [SerializeField] private bool useWorldSpace = true;
        [SerializeField] private RectTransform guiTarget;

        // Event bindings for health-related events
        private EventBinding<DamageTakenEvent> damageTakenBinding;
        private EventBinding<HealReceivedEvent> healReceivedBinding;
        private EventBinding<DeathEvent> deathBinding;

        private void OnEnable()
        {
            // Register for all health-related events using the existing pattern
            damageTakenBinding = new EventBinding<DamageTakenEvent>(this, HandleDamageTaken);
            healReceivedBinding = new EventBinding<HealReceivedEvent>(this, HandleHealReceived);
            deathBinding = new EventBinding<DeathEvent>(this, HandleDeath);

            EventBus<DamageTakenEvent>.Register(damageTakenBinding);
            EventBus<HealReceivedEvent>.Register(healReceivedBinding);
            EventBus<DeathEvent>.Register(deathBinding);

            Debug.Log($"[HealthEventMonitor] {gameObject.name} registered for health events");
        }

        private void OnDisable()
        {
            // Deregister from all health-related events
            EventBus<DamageTakenEvent>.Deregister(damageTakenBinding);
            EventBus<HealReceivedEvent>.Deregister(healReceivedBinding);
            EventBus<DeathEvent>.Deregister(deathBinding);

            Debug.Log($"[HealthEventMonitor] {gameObject.name} deregistered from health events");
        }

        // Event handling methods
        private void HandleDamageTaken(DamageTakenEvent damageEvent)
        {
            if (logEvents)
            {
                Debug.Log($"[HealthEventMonitor] {damageEvent.Target.name} took {damageEvent.Amount} damage");
            }

            if (showDamageNumbers && damageEvent.Target != null)
            {
                // Show damage numbers at the target's position using DamageNumbersPro
                ShowDamageNumber(damageEvent.Target.transform.position, damageEvent.Amount, false);
            }
        }

        private void HandleHealReceived(HealReceivedEvent healEvent)
        {
            if (logEvents)
            {
                Debug.Log($"[HealthEventMonitor] {healEvent.Target.name} received {healEvent.Amount} healing");
            }

            if (showDamageNumbers && healEvent.Target != null)
            {
                // Show heal numbers at the target's position using DamageNumbersPro
                ShowDamageNumber(healEvent.Target.transform.position, healEvent.Amount, true);
            }
        }

        private void HandleDeath(DeathEvent deathEvent)
        {
            if (logEvents)
            {
                Debug.Log($"[HealthEventMonitor] {deathEvent.Target.name} has died!");
            }

            // Example: Play death effects, update UI, etc.
            HandleEntityDeath(deathEvent.Target);
        }

        // Example methods for handling the events using DamageNumbersPro
        private void ShowDamageNumber(Vector3 position, float amount, bool isHeal)
        {
            if (damagePopupPrefab == null)
            {
                Debug.LogWarning("[HealthEventMonitor] No damage popup prefab assigned!");
                return;
            }

            DamageNumber popupPrefab = isHeal && healPopupPrefab != null ? healPopupPrefab : damagePopupPrefab;
            DamageNumber newPopup;

            if (useWorldSpace)
            {
                // Spawn in world space
                newPopup = popupPrefab.Spawn(position + Vector3.up, amount);
            }
            else
            {
                // Spawn in GUI space
                if (guiTarget == null)
                {
                    Debug.LogWarning("[HealthEventMonitor] GUI target is required for GUI space mode!");
                    return;
                }

                // Convert world position to screen position for GUI
                Vector2 screenPos = Camera.main.WorldToScreenPoint(position);
                newPopup = popupPrefab.SpawnGUI(guiTarget, screenPos, amount);
            }

            // Customize the popup based on the amount and type
            if (isHeal)
            {
                // Heal numbers - green and slightly larger
                newPopup.SetColor(new Color(0.2f, 1f, 0.2f)); // Green
                newPopup.SetScale(1.1f);
            }
            else
            {
                // Damage numbers - color based on amount
                if (amount > 50f)
                {
                    // Large damage - bright red and bigger
                    newPopup.SetScale(1.5f);
                    newPopup.SetColor(new Color(1f, 0.2f, 0.2f));
                }
                else if (amount > 20f)
                {
                    // Medium damage - orange
                    newPopup.SetScale(1.2f);
                    newPopup.SetColor(new Color(1f, 0.5f, 0.2f));
                }
                else
                {
                    // Small damage - light orange
                    newPopup.SetScale(1f);
                    newPopup.SetColor(new Color(1f, 0.7f, 0.5f));
                }
            }

            Debug.Log($"[HealthEventMonitor] Showing {(isHeal ? "heal" : "damage")} number: {amount} at {position}");
        }

        private void HandleEntityDeath(GameObject deadEntity)
        {
            // Example: Handle entity death (play effects, update score, etc.)
            Debug.Log($"[HealthEventMonitor] Handling death of {deadEntity.name}");
            
            // You could:
            // - Play death animation
            // - Spawn death particles
            // - Update game state
            // - Award experience/points
            // - Show a special death popup using DamageNumbersPro
            if (damagePopupPrefab != null && useWorldSpace)
            {
                // Show a special "DEAD" popup
                DamageNumber deathPopup = damagePopupPrefab.Spawn(deadEntity.transform.position + Vector3.up * 3f, "DEAD");
                deathPopup.SetColor(new Color(0.5f, 0.5f, 0.5f)); // Gray
                deathPopup.SetScale(1.5f);
            }
        }

        // Public methods for manual event registration (if needed)
        public void RegisterForDamageEvents()
        {
            damageTakenBinding = new EventBinding<DamageTakenEvent>(this, HandleDamageTaken);
            EventBus<DamageTakenEvent>.Register(damageTakenBinding);
        }

        public void RegisterForHealEvents()
        {
            healReceivedBinding = new EventBinding<HealReceivedEvent>(this, HandleHealReceived);
            EventBus<HealReceivedEvent>.Register(healReceivedBinding);
        }

        public void RegisterForDeathEvents()
        {
            deathBinding = new EventBinding<DeathEvent>(this, HandleDeath);
            EventBus<DeathEvent>.Register(deathBinding);
        }

        public void DeregisterFromAllEvents()
        {
            EventBus<DamageTakenEvent>.Deregister(damageTakenBinding);
            EventBus<HealReceivedEvent>.Deregister(healReceivedBinding);
            EventBus<DeathEvent>.Deregister(deathBinding);
        }

        // Test methods for debugging
        [Button]
        public void TestDamageEvent()
        {
            if (damagePopupPrefab != null)
            {
                float testAmount = Random.Range(10f, 100f);
                Vector3 testPosition = transform.position + Vector3.up * 2f;
                ShowDamageNumber(testPosition, testAmount, false);
            }
        }

        [Button]
        public void TestHealEvent()
        {
            if (healPopupPrefab != null)
            {
                float testAmount = Random.Range(10f, 50f);
                Vector3 testPosition = transform.position + Vector3.up * 2f;
                ShowDamageNumber(testPosition, testAmount, true);
            }
        }
    }
} 