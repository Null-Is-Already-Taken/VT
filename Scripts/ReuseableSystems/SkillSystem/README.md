# VT Skill System

A comprehensive, data-oriented skill system with clear separation of concerns, modular design, and a fluent API for rapid prototyping. This system allows you to create complex skills by composing small, reusable building blocks.

## 🏗️ Architecture Overview

The skill system is built around several key principles:

- **Data-Oriented Design**: Skills are defined using ScriptableObjects for easy configuration and iteration
- **Modular Blocks**: Skills are composed of small, reusable blocks that can be mixed and matched
- **Fluent API**: Clean, readable syntax for building complex skills
- **Clear Separation of Concerns**: Each component has a single responsibility
- **Scalable**: Easy to extend with new block types and functionality

## 📁 System Structure

```
SkillSystem/
├── Core/                    # Core interfaces and base classes
│   ├── ISkillBlock.cs      # Interface for skill blocks
│   ├── SkillExecutionContext.cs  # Execution context
│   └── SkillInstance.cs    # Runtime skill instance
├── Data/                   # Data structures
│   └── SkillDefinition.cs  # ScriptableObject skill definitions
├── Blocks/                 # Concrete skill blocks
│   ├── DamageBlock.cs      # Damage dealing block
│   └── HealBlock.cs        # Healing block
├── Assembly/               # Fluent API for skill building
│   └── SkillBuilder.cs     # Skill builder with fluent API
├── Execution/              # Runtime execution engine
│   └── SkillExecutor.cs    # Skill execution manager
└── Examples/               # Usage examples
    └── SkillSystemExample.cs  # Comprehensive example
```

## 🚀 Quick Start

### 1. Basic Skill Creation

```csharp
// Create a simple damage skill
var basicAttack = SkillBuilder.CreateDamageSkill("basic_attack", "Basic Attack", 15f, 0.5f);

// Create a skill instance
var skillInstance = new SkillInstance(basicAttack, player, 1);

// Execute the skill
skillExecutor.ExecuteSkill(skillInstance, target);
```

### 2. Complex Skill with Fluent API

```csharp
var fireball = new SkillBuilder("fireball", "Fireball")
    .WithDescription("Launches a fireball that deals fire damage")
    .WithType(SkillType.Active)
    .WithTargetType(TargetType.Direction)
    .WithCooldown(2f)
    .WithRange(15f)
    .WithManaCost(25f)
    .AddDamage(30f, 5f, DamageType.Fire)
    .AddVisualEffect(fireballPrefab)
    .AddAudioEffect(fireballSound)
    .Build();
```

### 3. Setting Up the System

```csharp
// Add SkillExecutor to a GameObject
var skillExecutor = gameObject.AddComponent<SkillExecutor>();

// Create and execute skills
var skill = SkillBuilder.CreateDamageSkill("test", "Test Skill", 10f);
var instance = new SkillInstance(skill, player, 1);
skillExecutor.ExecuteSkill(instance, target);
```

## 🧱 Skill Blocks

Skill blocks are the fundamental building blocks of the system. Each block represents a single, atomic piece of functionality.

### Built-in Blocks

#### DamageBlock
Deals damage to targets with support for critical hits and damage types.

```csharp
var damageBlock = new DamageBlock(20f, 3f, DamageType.Fire)
    .ConfigureCrit(true, 0.15f, 2.5f)
    .SetIgnoreArmor(false);
```

#### HealBlock
Heals targets with support for critical heals and overheal.

```csharp
var healBlock = new HealBlock(25f, 2f)
    .ConfigureOverheal(true, 0.2f)
    .ConfigureCrit(true, 0.1f, 1.5f);
```

### Creating Custom Blocks

To create a custom skill block, implement the `ISkillBlock` interface:

```csharp
public class CustomBlock : ISkillBlock
{
    public string BlockId => "custom_block";
    public string DisplayName => "Custom Block";
    public string Description => "A custom skill block";

    public bool CanExecute(SkillExecutionContext context)
    {
        // Check if the block can execute
        return true;
    }

    public SkillBlockResult Execute(SkillExecutionContext context)
    {
        // Execute the block logic
        return SkillBlockResult.Success;
    }

    public ValidationResult Validate()
    {
        // Validate the block configuration
        return ValidationResult.Success();
    }
}
```

## 🔧 Fluent API Reference

The `SkillBuilder` class provides a fluent API for creating skills:

### Basic Configuration

```csharp
.WithDescription(string description)     // Set skill description
.WithIcon(Sprite icon)                   // Set skill icon
.WithType(SkillType skillType)           // Set skill type
.WithTargetType(TargetType targetType)   // Set targeting type
.WithCooldown(float cooldown)            // Set cooldown time
.WithCastTime(float castTime)            // Set cast time
.WithRange(float range)                  // Set skill range
.WithManaCost(float manaCost)            // Set mana cost
.RequiresTarget(bool requiresTarget)     // Set if target is required
.CanBeInterrupted(bool canBeInterrupted) // Set if skill can be interrupted
.WithRequiredLevel(int requiredLevel)    // Set required level
```

### Adding Blocks

```csharp
.AddDamage(float baseDamage, float scaling, DamageType type)  // Add damage block
.AddHeal(float baseHeal, float scaling)                       // Add heal block
.AddBlock(ISkillBlock block)                                  // Add custom block
.AddVisualEffect(GameObject prefab)                           // Add visual effect
.AddAudioEffect(AudioClip clip)                               // Add audio effect
.AddMovement(float distance, float duration)                  // Add movement
.AddKnockback(float force, float upwardForce)                 // Add knockback
.AddStun(float duration)                                      // Add stun
.AddBuff(string buffId, float duration)                       // Add buff
.AddDebuff(string debuffId, float duration)                   // Add debuff
.AddTeleport(float maxDistance)                               // Add teleport
.AddProjectile(GameObject prefab, float speed)                // Add projectile
.AddAreaOfEffect(float radius, LayerMask layers)              // Add AoE
```

### Advanced Block Types

```csharp
.AddConditionalBlock(block, condition)    // Conditional execution
.AddDelayedBlock(block, delay)            // Delayed execution
.AddRepeatedBlock(block, count, interval) // Repeated execution
.AddChainBlock(block)                     // Chain execution
```

## 🎮 Skill Types

The system supports different types of skills:

- **Active**: Player-activated skills (most common)
- **Passive**: Always active skills
- **Toggle**: Can be turned on/off
- **Ultimate**: Special powerful skills

## 🎯 Targeting Types

Different targeting mechanisms are supported:

- **None**: No target needed
- **Self**: Target is the caster
- **Single**: Single target
- **Multiple**: Multiple targets
- **Area**: Area of effect
- **Direction**: Directional skill

## 🔄 Skill Execution Flow

1. **Skill Request**: Player or AI requests skill execution
2. **Validation**: Check if skill can be cast (cooldown, mana, etc.)
3. **Casting**: If cast time > 0, enter casting phase
4. **Block Execution**: Execute skill blocks in order
5. **Completion**: Mark skill as completed and start cooldown

## 📊 Skill Scaling

Skills automatically scale with level:

```csharp
// Get scaled values
float currentDamage = skillInstance.GetScaledValue(baseDamage, damageScaling);
float currentRange = skillInstance.GetCurrentRange();
float currentManaCost = skillInstance.GetCurrentManaCost();
```

## 🎨 Integration with DamageNumbersPro

The system integrates with DamageNumbersPro for visual feedback:

```csharp
// In a custom block or event handler
var damageNumber = damagePopupPrefab.Spawn(targetPosition, damageAmount);
```

## 🔌 Event System Integration

The skill system integrates with the existing event system:

```csharp
// Subscribe to skill events
skillExecutor.OnSkillStarted += OnSkillStarted;
skillExecutor.OnSkillCompleted += OnSkillCompleted;
skillExecutor.OnSkillFailed += OnSkillFailed;
skillExecutor.OnSkillInterrupted += OnSkillInterrupted;
```

## 📝 Best Practices

### 1. Skill Design
- Keep blocks small and focused
- Use meaningful block names and descriptions
- Validate skill configurations
- Test skills thoroughly

### 2. Performance
- Avoid expensive operations in block execution
- Use object pooling for effects
- Cache frequently accessed data
- Limit the number of active skills

### 3. Extensibility
- Create reusable block types
- Use the fluent API for consistency
- Follow the established patterns
- Document custom blocks

### 4. Debugging
- Enable debug logging in SkillExecutor
- Use the validation system
- Test skills in isolation
- Monitor skill execution flow

## 🧪 Testing

The system includes comprehensive testing capabilities:

```csharp
// Test skill execution
[Button("Test Skill")]
private void TestSkill()
{
    skillExecutor.ExecuteSkill(skillInstance, target);
}

// Validate skill configuration
var validation = skillDefinition.Validate();
if (!validation.IsValid)
{
    Debug.LogError(validation.ErrorMessage);
}
```

## 🔮 Future Enhancements

Planned features for future versions:

- **Skill Trees**: Hierarchical skill progression
- **Skill Combos**: Chain multiple skills together
- **Skill Modifiers**: Runtime skill modification
- **Skill Templates**: Pre-built skill configurations
- **Visual Skill Builder**: Editor tool for skill creation
- **Skill Analytics**: Performance tracking and optimization
- **Network Support**: Multiplayer skill synchronization

## 📚 Examples

See `SkillSystemExample.cs` for comprehensive usage examples including:

- Basic skill creation and execution
- Complex skill composition
- Input handling
- Event management
- Skill leveling
- Testing utilities

## 🤝 Contributing

When adding new features to the skill system:

1. Follow the existing architecture patterns
2. Add comprehensive documentation
3. Include validation and error handling
4. Create example usage
5. Update this README

## 📄 License

This skill system is part of the VT framework and follows the same licensing terms. 