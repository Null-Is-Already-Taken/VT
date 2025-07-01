# VT Modular Skill System

A lightweight, grammar-based skill system that uses building blocks to create skills. Each skill is composed of blocks that follow grammatical rules, ensuring valid skill combinations.

## Core Concepts

### Grammar Rules
- **Effect Block**: The core action (damage, healing, etc.) - can start a skill
- **Target Block**: Defines targeting behavior - can only follow Effect blocks
- **Modifier Blocks**: Decorative blocks that modify properties - can follow Effect or Target blocks

### Building Blocks

#### Effect Block
- **Purpose**: Defines the core action of the skill
- **Types**: Damage, Healing, Buff, Debuff, Movement, Custom
- **Grammar**: Can start a skill, can be followed by Target or Modifier blocks

#### Target Block
- **Purpose**: Defines how the skill targets entities
- **Types**: Self, Single, Multiple, Area, Directional
- **Grammar**: Can only follow Effect blocks

#### Modifier Blocks
- **Magnitude Modifier**: Modifies the strength of the effect
- **Range Modifier**: Modifies the range of the skill
- **Duration Modifier**: Modifies the duration of the effect
- **Grammar**: Can follow Effect, Target, or other Modifier blocks

## Usage

### Creating Skills

1. **Create Skill Asset**:
   - Right-click in Project → Create → VT/Skill System/Skill
   - Name your skill

2. **Add Blocks**:
   - Create block assets (Effect, Target, Modifiers)
   - Add them to the skill in the correct order
   - The system will validate grammar rules automatically

### Example Skill Combinations

#### Basic Attack
```
Effect (Damage) → Target (Single)
```

#### Area Heal
```
Effect (Healing) → Target (Area) → Magnitude Modifier (×1.5)
```

#### Buffed Fireball
```
Effect (Damage) → Target (Directional) → Magnitude Modifier (×2.0) → Range Modifier (×1.5)
```

### Using Skills in Code

```csharp
// Get a skill executor component
var executor = GetComponent<SkillExecutor>();

// Execute the assigned skill
executor.ExecuteSkill();

// Execute a specific skill
executor.ExecuteSkill(mySkill, target);
```

### Creating Custom Blocks

1. Inherit from `SkillBlock`
2. Override `Execute(SkillContext context)`
3. Set grammar rules in `OnEnable()`
4. Use `CreateAssetMenu` attribute for easy creation

```csharp
[CreateAssetMenu(fileName = "New Custom Block", menuName = "VT/Skill System/Blocks/Custom")]
public class CustomBlock : SkillBlock
{
    private void OnEnable()
    {
        // Set grammar rules
        canStartSkill = false;
        validPreviousBlocks.Add(typeof(EffectBlock));
    }
    
    public override void Execute(SkillContext context)
    {
        // Your custom logic here
    }
}
```

## Validation

The system automatically validates skills:
- First block must be able to start a skill
- Each block must be able to follow the previous block
- Invalid combinations are logged as errors

## Context System

The `SkillContext` class carries data between blocks:
- `Caster`: The entity casting the skill
- `Target`: The target entity
- `Magnitude`: Effect strength
- `Duration`: Effect duration
- `Range`: Skill range
- Custom data storage for block communication

## Performance

- Lightweight execution with minimal overhead
- ScriptableObject-based for easy asset management
- Grammar validation only occurs when needed
- Efficient context sharing between blocks

## Extensibility

The system is designed to be easily extended:
- Add new block types by inheriting from `SkillBlock`
- Create custom grammar rules
- Extend the context with additional data
- Add new validation rules 