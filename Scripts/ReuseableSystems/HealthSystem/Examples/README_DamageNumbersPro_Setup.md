# DamageNumbersPro Integration Setup Guide

This guide explains how to set up the health system event monitoring classes with DamageNumbersPro for displaying damage and heal numbers.

## Prerequisites

1. **DamageNumbersPro Asset**: Make sure you have DamageNumbersPro imported in your project
2. **Health System**: Ensure the VT health system is properly set up with events

## Setup Steps

### 1. Create DamageNumbersPro Prefabs

First, you need to create DamageNumbersPro prefabs for damage and healing:

#### For World Space (3D):
1. In the Project window, navigate to `DamageNumbersPro/Demo/Prefabs/3D/`
2. Find a damage number prefab (e.g., `Basic-Default.prefab`)
3. Duplicate it and rename it to something like `DamagePopup.prefab`
4. Optionally create a separate heal prefab (e.g., `Pixel Heal.prefab`)

#### For GUI Space (2D/UI):
1. In the Project window, navigate to `DamageNumbersPro/Demo/Prefabs/UI/`
2. Find a GUI damage number prefab (e.g., `Basic-Default.prefab`)
3. Duplicate it and rename it to something like `DamagePopupGUI.prefab`

### 2. Configure DamageNumberDisplay

1. **Add the Component**: Add `DamageNumberDisplay` to a GameObject in your scene
2. **Assign Prefabs**: 
   - Drag your damage popup prefab to the `Damage Popup Prefab` field
   - Optionally drag a heal popup prefab to the `Heal Popup Prefab` field
3. **Choose Display Mode**:
   - Check `Use World Space` for 3D world space display
   - Uncheck for GUI space display (requires a `GUI Target` RectTransform)
4. **Set GUI Target** (if using GUI space): Assign a RectTransform from your Canvas

### 3. Configure HealthEventMonitor

1. **Add the Component**: Add `HealthEventMonitor` to a GameObject in your scene
2. **Assign Prefabs**: Same as above
3. **Configure Settings**:
   - `Log Events`: Enable to see debug logs
   - `Show Damage Numbers`: Enable to display damage numbers
   - `Use World Space`: Choose display mode
   - `GUI Target`: Required for GUI space mode

### 4. Test the Setup

Both classes include test methods accessible via the Context Menu:

1. **Right-click** on the GameObject with the component
2. Select **"Test Damage Number"** or **"Test Heal Number"**
3. You should see damage/heal numbers appear

## Example Scene Setup

```
Scene Hierarchy:
├── HealthSystem
│   ├── HealthEventMonitor (with DamageNumberDisplay component)
│   └── DamageNumberDisplay (standalone component)
├── Player
│   └── Health (with Health component)
├── Enemy
│   └── Health (with Health component)
└── Canvas (for GUI mode)
    └── DamageNumberTarget (RectTransform)
```

## Configuration Options

### DamageNumberDisplay Settings

- **Enable Damage Numbers**: Toggle damage number display on/off
- **Damage Popup Prefab**: The DamageNumbersPro prefab for damage numbers
- **Heal Popup Prefab**: Optional separate prefab for heal numbers
- **Use World Space**: Choose between world space (3D) or GUI space (2D)
- **GUI Target**: Required RectTransform for GUI space mode

### HealthEventMonitor Settings

- **Log Events**: Enable debug logging for all health events
- **Show Damage Numbers**: Enable visual damage number display
- **Damage Numbers Pro Settings**: Same as DamageNumberDisplay

## Customization

### Color Coding
The classes automatically color-code damage numbers based on amount:
- **Small damage (≤20)**: Light orange
- **Medium damage (21-50)**: Orange  
- **Large damage (>50)**: Bright red
- **Heals**: Green

### Scaling
Damage numbers are automatically scaled based on amount:
- **Small damage**: Normal scale (1.0x)
- **Medium damage**: 1.2x scale
- **Large damage**: 1.5x scale
- **Heals**: 1.1x scale

### Custom Prefabs
You can create custom DamageNumbersPro prefabs with:
- Different fonts and materials
- Custom animations and effects
- Special visual styles
- Different lifetime settings

## Troubleshooting

### No Damage Numbers Appearing
1. Check that `Enable Damage Numbers` is checked
2. Verify that a `Damage Popup Prefab` is assigned
3. Ensure the Health component is properly raising events
4. Check the Console for any error messages

### GUI Mode Not Working
1. Make sure `Use World Space` is unchecked
2. Assign a `GUI Target` RectTransform
3. Ensure the GUI target is part of a Canvas
4. Verify the Camera.main is set up correctly

### Performance Issues
1. Enable pooling in your DamageNumbersPro prefabs
2. Set appropriate lifetime values
3. Use spam control settings to limit overlapping numbers
4. Consider using GUI mode for better performance in 2D games

## Advanced Usage

### Custom Event Handling
You can extend the classes to handle additional events:

```csharp
// Add to HealthEventMonitor
private void HandleCriticalHit(CriticalHitEvent criticalEvent)
{
    // Show special critical hit effect
    DamageNumber criticalPopup = damagePopupPrefab.Spawn(
        criticalEvent.Target.transform.position, 
        "CRITICAL!"
    );
    criticalPopup.SetColor(Color.red);
    criticalPopup.SetScale(2f);
}
```

### Integration with Other Systems
The event-based approach makes it easy to integrate with:
- Audio systems (play damage sounds)
- Particle systems (spawn hit effects)
- UI systems (update health bars)
- Achievement systems (track damage milestones) 