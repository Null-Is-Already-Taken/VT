//Assets /
//└── VT.ReusableSystems.ProceduralEffectBuilder /
//    ├── Runtime /
//    │   ├── Core /
//    │   │   ├── IEffect.cs
//    │   │   ├── IMagnitudeAware.cs
//    │   │   ├── NoOpEffect.cs
//    │   │   ├── ChainedEffect.cs
//    │   │   ├── ConditionalEffect.cs
//    │   │   ├── DelayedEffect.cs
//    │   │   ├── FeedbackEffectWrapper.cs
//    │   │   └── SkillCompiler.cs
//    │   ├── Interfaces /
//    │   │   └── IHealth.cs
//    │   ├── Definitions /
//    │   │   ├── SkillTemplate.cs
//    │   │   ├── EffectDefinition.cs
//    │   │   ├── TargetType.cs
//    │   │   ├── SourceType.cs
//    │   │   ├── MagnitudeDefinition.cs
//    │   │   ├── TimingModifier.cs
//    │   │   ├── ConditionDefinition.cs
//    │   │   ├── FeedbackDecorator.cs
//    │   │   └── EffectComposition.cs
//    │   ├── Effects /
//    │   │   ├── DamageEffectDefinition.cs
//    │   │   └── DamageEffect.cs
//    │   ├── Compositions /
//    │   │   ├── EffectSequence.cs
//    │   │   └── ConditionalComposition.cs
//    │   └── Components /
//    │       └── Health.cs                   # Example MonoBehaviour implementing IHealth
//    ├── Editor /
//    │   ├── SkillTemplateEditor.cs          # (optional) Custom Inspector/UI helpers
//    │   └── Validators /                     # Runtime/editor checks (e.g. conflict rules)
//    └── Tests /
//        ├── RuntimeTests /
//        │   └── SkillCompilerTests.cs
//        └── IntegrationTests /
//            └── EffectExecutionTest.cs
