using UnityEngine;
using VT.ReusableSystems.Events;
using DamageNumbersPro;
using Sirenix.OdinInspector;

namespace VT.ReusableSystems.HealthSystem.Examples
{
    /// <summary>
    /// Simple example class that demonstrates registering for damage events only.
    /// This class shows how to use DamageNumbersPro for displaying damage numbers.
    /// </summary>
    public class DamageNumberDisplay : MonoBehaviour
    {
        [Header("Damage Display Settings")]
        [SerializeField] private bool enableDamageNumbers = true;
        [SerializeField] private DamageNumber damagePopupPrefab; // Reference to DamageNumbersPro prefab
        [SerializeField] private DamageNumber healPopupPrefab; // Reference to heal prefab (optional)
        [SerializeField] private bool useWorldSpace = true; // Use world space or GUI space
        [SerializeField] private RectTransform guiTarget; // Required for GUI space

        // Single event binding for damage events
        private EventBinding<DamageTakenEvent> damageBinding;

        private void OnEnable()
        {
            if (enableDamageNumbers)
            {
                // Register for damage events using the existing pattern
                damageBinding = new EventBinding<DamageTakenEvent>(this, HandleDamage);
                EventBus<DamageTakenEvent>.Register(damageBinding);
                Debug.Log($"[DamageNumberDisplay] {gameObject.name} registered for damage events");
            }
        }

        private void OnDisable()
        {
            if (damageBinding != null)
            {
                // Deregister from damage events
                EventBus<DamageTakenEvent>.Deregister(damageBinding);
                Debug.Log($"[DamageNumberDisplay] {gameObject.name} deregistered from damage events");
            }
        }

        private void HandleDamage(DamageTakenEvent damageEvent)
        {
            if (!enableDamageNumbers || damageEvent.Target == null) return;

            // Show damage number at the target's position
            ShowDamageNumber(damageEvent.Target.transform.position, damageEvent.Amount, false);
        }

        private void ShowDamageNumber(Vector3 position, float amount, bool isHeal)
        {
            if (damagePopupPrefab == null)
            {
                Debug.LogWarning("[DamageNumberDisplay] No damage popup prefab assigned!");
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
                    Debug.LogWarning("[DamageNumberDisplay] GUI target is required for GUI space mode!");
                    return;
                }

                // Convert world position to screen position for GUI
                Vector2 screenPos = Camera.main.WorldToScreenPoint(position);
                newPopup = popupPrefab.SpawnGUI(guiTarget, screenPos, amount);
            }

            // Customize the popup based on the amount
            if (amount > 50f)
            {
                // Large damage - make it bigger and redder
                newPopup.SetScale(1.5f);
                newPopup.SetColor(new Color(1f, 0.2f, 0.2f)); // Bright red
            }
            else if (amount > 20f)
            {
                // Medium damage - orange
                newPopup.SetScale(1.2f);
                newPopup.SetColor(new Color(1f, 0.5f, 0.2f)); // Orange
            }
            else
            {
                // Small damage - default settings
                newPopup.SetScale(1f);
                newPopup.SetColor(new Color(1f, 0.7f, 0.5f)); // Light orange
            }

            // For heals, use green color
            if (isHeal)
            {
                newPopup.SetColor(new Color(0.2f, 1f, 0.2f)); // Green
            }

            Debug.Log($"[DamageNumberDisplay] Showing {(isHeal ? "heal" : "damage")} number: {amount} at {position}");
        }

        // Public method to toggle damage number display
        public void ToggleDamageNumbers(bool enabled)
        {
            if (enabled && !enableDamageNumbers)
            {
                // Enable damage numbers
                enableDamageNumbers = true;
                if (damageBinding == null)
                {
                    damageBinding = new EventBinding<DamageTakenEvent>(this, HandleDamage);
                    EventBus<DamageTakenEvent>.Register(damageBinding);
                }
            }
            else if (!enabled && enableDamageNumbers)
            {
                // Disable damage numbers
                enableDamageNumbers = false;
                if (damageBinding != null)
                {
                    EventBus<DamageTakenEvent>.Deregister(damageBinding);
                    damageBinding = null;
                }
            }
        }

        // Public method to manually spawn a damage number (for testing)
        [Button]
        public void TestDamageNumber()
        {
            if (damagePopupPrefab != null)
            {
                float testAmount = Random.Range(10f, 100f);
                Vector3 testPosition = transform.position + Vector3.up * 2f;
                ShowDamageNumber(testPosition, testAmount, false);
            }
        }

        // Public method to manually spawn a heal number (for testing)
        [Button]
        public void TestHealNumber()
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