# VT.ReusableSystems.ProceduralEffectBuilder

A **modular, designer-friendly procedural skill system** for Unity.  
Inspired by how natural language composes meaning, this framework allows designers to build skills by combining modular grammar nodes (`ScriptableObjects`) into fully functional runtime effects.

---

## ✨ Features

- ✅ Modular effect system using `ScriptableObject`-based grammar
- ✅ Async runtime pipeline with effect chaining and conditionals
- ✅ Feedback decorators for VFX/SFX wrapping
- ✅ Timing modifiers (e.g., delayed, one-shot)
- ✅ Dynamic magnitude injection (e.g., damage scaling)
- ✅ Clean separation between runtime logic and designer data
- ✅ Skill composition via chaining or conditional branching
- ✅ Extendable interface contracts (`IHealth`, `IMagnitudeAware`, etc.)

---

## 📦 Folder Structure

```plaintext
VT.ReusableSystems.ProceduralEffectBuilder/
├── Runtime/
│   ├── Core/              # Base effect logic and compiler
│   ├── Interfaces/        # Contracts like IHealth
│   ├── Definitions/       # ScriptableObject grammar nodes
│   ├── Effects/           # Concrete effect implementations
│   ├── Compositions/      # Multi-effect chain and branching
│   └── Components/        # Runtime MonoBehaviours (e.g., Health)
├── Editor/                # Custom inspectors and validators
└── Tests/                 # Unit and integration tests
```

---

## 🧠 How It Works

A `SkillTemplate` is a ScriptableObject that defines:

| Part             | Role                                                       |
|------------------|------------------------------------------------------------|
| `SourceType`     | Who causes the effect (e.g., self, owner, spawner)         |
| `TargetType`     | Who receives the effect (e.g., single enemy, AOE)          |
| `EffectDefinition` | What the effect does (e.g., damage, heal, buff)         |
| `Magnitude`      | How much (e.g., 25 base damage, 50% of target HP)          |
| `Condition`      | Whether to execute the effect                              |
| `TimingModifier` | When to execute (e.g., delay, duration)                    |
| `Feedbacks`      | VFX/SFX decorators for visual polish                       |
| `Composition`    | Optional multi-effect logic (e.g., chains, conditionals)   |

Each part compiles into a runtime `IEffect`, which is executed via `SkillCompiler`.

---

## 🧪 Example Flow

```csharp
var effect = SkillCompiler.Compile(skillTemplate, caster);
await effect.ExecuteAsync();
```

Internally:

- Targets are resolved  
- Effects are composed and chained  
- Visuals, conditions, and timing are applied  
- Effect executes on runtime objects (via interfaces like `IHealth`)

---

## 🔌 Extending the System

To add a new type of effect:

- Create a `MyEffectDefinition : EffectDefinition`
- Create a runtime `MyEffect : IEffect`
- (Optional) Implement `IMagnitudeAware` or use `FeedbackDecorator`
- Expose your ScriptableObject to designers via `[CreateAssetMenu]`

To enforce grammar constraints (e.g., no "Area" + "SingleTarget"):

- Create validators under `Editor/Validators/`
- Hook them into a custom inspector or build-time rule check

---

## 🚧 Roadmap

- [ ] Editor UI for drag-and-drop composition  
- [ ] Rule validator for design-time errors  
- [ ] Additional runtime interfaces: `IStunnable`, `IBuffable`, etc.  
- [ ] Playmode test scenes for rapid iteration  
- [ ] Integration with chatbot and narrative AI support (future)