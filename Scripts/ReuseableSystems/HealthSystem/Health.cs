using Sirenix.OdinInspector;
using UnityEngine;
using VT.ReusableSystems.Events;

namespace VT.ReusableSystems.HealthSystem
{
    public class Health : MonoBehaviour, IHealth
    {
        [SerializeField] private float maxHP = 100f;
        [SerializeField] private float currentHP;

        public float CurrentHP => currentHP;
        public float MaxHP => maxHP;
        public bool IsDead => currentHP <= 0;

        private void Awake()
        {
            currentHP = maxHP;
        }

        [Button]
        public void TakeDamage(float amount)
        {
            if (IsDead) return;

            currentHP -= amount;
            EventBus<DamageTakenEvent>.Raise(new DamageTakenEvent { Target = gameObject, Amount = amount });

            if (currentHP <= 0)
            {
                currentHP = 0;
                EventBus<DeathEvent>.Raise(new DeathEvent { Target = gameObject });
            }
        }

        [Button]
        public void Heal(float amount)
        {
            if (IsDead) return;

            currentHP = Mathf.Min(currentHP + amount, maxHP);
            EventBus<HealReceivedEvent>.Raise(new HealReceivedEvent { Target = gameObject, Amount = amount });
        }

        public void SetMaxHP(float amount)
        {
            maxHP = amount;
        }
    }
}
