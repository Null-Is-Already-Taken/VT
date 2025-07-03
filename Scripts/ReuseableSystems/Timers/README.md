# VT Reusable Timers System

A flexible, fluent, and event-driven timer utility for Unity, supporting both finite and infinite durations, pausing/resuming, and automatic disposal. Includes a configurable logger for debugging and development.

## Folder Structure

- `Timer.cs` — Core timer logic (runtime)
- `Utils/TimerLogger.cs` — Timer logger for debugging (runtime and editor)
- `Test/TimerTest.cs` — Example/test MonoBehaviour for Unity
- `VT.ReusableSystems.Timers.asmdef` — Assembly definition for Unity package

## Features
- Countdown timers with event callbacks
- Infinite (indefinite) timers
- Pause, resume, restart, stop, and auto-dispose
- Fluent API for easy chaining
- Works with or without a MonoBehaviour runner
- Supports scaled and unscaled time
- Configurable logging (progress, elapsed time, custom names)
- Runtime and editor compatible logger

## Installation
Copy the files in this folder (and subfolders) to your Unity project. No additional dependencies are required.

## Basic Usage

### Creating and Starting a Timer
```csharp
using VT.Utils;

// Create a 5-second timer
Timer.Create(5f)
    .OnStart(() => Debug.Log("Timer started"))
    .OnUpdate(progress => Debug.Log($"Progress: {progress:P0}"))
    .OnComplete(() => Debug.Log("Timer completed"))
    .AutoDispose()
    .Start();
```

### Infinite Timer
```csharp
Timer infiniteTimer = Timer.Create()
    .RunIndefinitely(); // Infinite duration

VT.ReusableSystems.Timers.Utils.TimerLogger.Attach(infiniteTimer)
    .WithName("Infinite")
    .LogProgress()
    .LogTime();

infiniteTimer.Start();
```

### Using TimerLogger (Runtime or Editor)
```csharp
using VT.ReusableSystems.Timers.Utils;

Timer timer = Timer.Create(10f);
TimerLogger.Attach(timer)
    .WithName("My Timer")
    .LogProgress()    // Log progress every 5% (default)
    .LogTime(2f);     // Log elapsed time every 2 seconds
```

## Example: Pausing and Resuming
```csharp
Timer mainTimer = Timer.Create(20f).Start();
Timer pauseAndResumeTimer = Timer.Create(5f)
    .OnStart(() => mainTimer.Pause())
    .OnComplete(() => mainTimer.Resume())
    .AutoDispose()
    .Start();
```

## API Highlights

### Timer
- `Timer.Create(float seconds = -1, MonoBehaviour runner = null)`
- `SetDuration(float seconds)`
- `RunIndefinitely()`
- `OnStart(Action onStart)`
- `OnUpdate(Action<float> onUpdate)`
- `OnComplete(Action onComplete)`
- `WithRunner(MonoBehaviour runner)`
- `UseUnscaledTime(bool useUnscaledTime)`
- `AutoDispose()`
- `Start()`, `Pause()`, `Resume()`, `Restart()`, `Stop()`, `Dispose()`
- Properties: `Duration`, `Elapsed`, `Progress`, `IsRunning`

### TimerLogger
- `TimerLogger.Attach(Timer timer)`
- `WithName(string name)`
- `LogProgress(float threshold = 0.05f)`
- `LogTime(float threshold = 1f)`

## Example Test Methods (from TimerTest.cs)
```csharp
// Start a timer with an external runner
Timer basicTimer = Timer.Create().SetDuration(duration).WithRunner(this);
VT.ReusableSystems.Timers.Utils.TimerLogger.Attach(basicTimer).WithName("External Runner").LogProgress();
basicTimer.Start();

// Fluent API timer
Timer fluentTimer = Timer.Create().SetDuration(duration);
VT.ReusableSystems.Timers.Utils.TimerLogger.Attach(fluentTimer).WithName("Fluent").LogProgress();
fluentTimer.Start();

// Infinite timer
Timer infiniteTimer = Timer.Create().RunIndefinitely();
VT.ReusableSystems.Timers.Utils.TimerLogger.Attach(infiniteTimer).WithName("Infinite").LogProgress().LogTime();
infiniteTimer.Start();
```

## Notes
- `TimerLogger` is now available for both runtime and editor use (see `Utils/TimerLogger.cs`).
- The logger is designed for debugging and development, and uses `Debug.Log` for output.

## License
See repository for license details. 