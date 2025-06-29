namespace VT.ReusableSystems.HealthSystem
{
    public interface IHealth
    {
        void TakeDamage(float amount);
        void Heal(float amount);

        float CurrentHP { get; }
        float MaxHP { get; }
        bool IsDead { get; }
    }
}
