# EventRegistrationHelper Utility

The `EventRegistrationHelper` utility class provides simplified methods for registering and deregistering from events in the VT event system. This utility makes it easier to manage event bindings with proper lifecycle management.

## Features

- **Simplified Registration**: One-line registration for single or multiple events
- **Automatic Lifecycle Management**: Proper registration/deregistration patterns
- **Type Safety**: Generic methods ensure type safety
- **Memory Leak Prevention**: Built-in null checks and proper cleanup

## Basic Usage

### Single Event Registration

```csharp
public class MyComponent : MonoBehaviour
{
    private EventBinding<DamageTakenEvent> damageBinding;

    private void OnEnable()
    {
        // Register for damage events
        EventRegistrationHelper.RegisterForEvent(this, HandleDamage, out damageBinding);
    }

    private void OnDisable()
    {
        // Deregister from damage events
        EventRegistrationHelper.DeregisterFromEvent(damageBinding);
    }

    private void HandleDamage(DamageTakenEvent damageEvent)
    {
        Debug.Log($"Damage taken: {damageEvent.Amount}");
    }
}
```

### Multiple Event Registration

```csharp
public class HealthMonitor : MonoBehaviour
{
    private EventBinding<DamageTakenEvent> damageBinding;
    private EventBinding<HealReceivedEvent> healBinding;
    private EventBinding<DeathEvent> deathBinding;

    private void OnEnable()
    {
        // Register for multiple events at once
        EventRegistrationHelper.RegisterForEvents(
            owner: this,
            callback1: HandleDamage,
            callback2: HandleHeal,
            callback3: HandleDeath,
            binding1: out damageBinding,
            binding2: out healBinding,
            binding3: out deathBinding
        );
    }

    private void OnDisable()
    {
        // Deregister from all events at once
        EventRegistrationHelper.DeregisterFromEvents(damageBinding, healBinding, deathBinding);
    }

    private void HandleDamage(DamageTakenEvent damageEvent) { /* ... */ }
    private void HandleHeal(HealReceivedEvent healEvent) { /* ... */ }
    private void HandleDeath(DeathEvent deathEvent) { /* ... */ }
}
```

### Two Event Registration

```csharp
public class SimpleMonitor : MonoBehaviour
{
    private EventBinding<DamageTakenEvent> damageBinding;
    private EventBinding<HealReceivedEvent> healBinding;

    private void OnEnable()
    {
        // Register for two events
        EventRegistrationHelper.RegisterForEvents(
            owner: this,
            callback1: HandleDamage,
            callback2: HandleHeal,
            binding1: out damageBinding,
            binding2: out healBinding
        );
    }

    private void OnDisable()
    {
        // Deregister from two events
        EventRegistrationHelper.DeregisterFromEvents(damageBinding, healBinding);
    }
}
```

## Available Methods

### Registration Methods

- `RegisterForEvent<T>(MonoBehaviour owner, Action<T> callback, out EventBinding<T> binding)`
- `BindAndRegisterEvent<T>(MonoBehaviour owner, Action<T> callback, out EventBinding<T> binding)`
- `RegisterForEvents<T1, T2>(MonoBehaviour owner, Action<T1> callback1, Action<T2> callback2, out EventBinding<T1> binding1, out EventBinding<T2> binding2)`
- `RegisterForEvents<T1, T2, T3>(MonoBehaviour owner, Action<T1> callback1, Action<T2> callback2, Action<T3> callback3, out EventBinding<T1> binding1, out EventBinding<T2> binding2, out EventBinding<T3> binding3)`

### Deregistration Methods

- `DeregisterFromEvent<T>(EventBinding<T> binding)`
- `DeregisterFromEvents<T1, T2>(EventBinding<T1> binding1, EventBinding<T2> binding2)`
- `DeregisterFromEvents<T1, T2, T3>(EventBinding<T1> binding1, EventBinding<T2> binding2, EventBinding<T3> binding3)`

## Best Practices

1. **Always deregister in OnDisable()**: This prevents memory leaks and ensures proper cleanup
2. **Store bindings as private fields**: This allows for proper deregistration
3. **Use the out parameter pattern**: This ensures the binding is properly initialized
4. **Check for null before deregistering**: The utility handles this automatically, but it's good practice
5. **Use descriptive callback names**: Makes debugging easier

## Examples

See the following example classes for complete implementations:

- `HealthEventMonitor.cs` - Full health system event monitoring
- `DamageNumberDisplay.cs` - Simple damage number display system

## Integration with Existing Code

The utility is designed to work seamlessly with the existing VT event system. You can gradually migrate existing event registration code to use this utility for cleaner, more maintainable code.

## Performance Considerations

- The utility methods are lightweight and have minimal overhead
- Registration/deregistration is O(1) for single events
- Multiple event registration is O(n) where n is the number of events
- Memory usage is the same as manual registration 