namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    public enum StatType
    {
        Strength,
        Dexterity,
        Intelligence,
        Vitality,
        Defense,
        Luck,
        Speed
    }
    
    public enum DamageType
    {
        Physical,
        Fire,
        Ice,
        Lightning,
        Poison,
        Arcane,
        Holy,
        Shadow,
        True // Bypasses resistance
    }
    
    public enum SkillCategory
    {
        Melee,
        Ranged,
        Spell,
        Trap,
        Summon,
        Aura,
        Utility
    }
}
