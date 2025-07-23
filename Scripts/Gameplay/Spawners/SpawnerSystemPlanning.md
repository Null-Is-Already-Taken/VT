# Generic GameObject Spawner System Planning

## Overview

This document outlines the initial planning for a flexible, generic GameObject spawner system in Unity. The system will utilize the Strategy Pattern to allow for customizable spawn behaviors and location providers.

## Core Concepts

- **Spawner**: The main component responsible for orchestrating the spawning process. It delegates all spawn rules to the strategy and only coordinates the process.
- **Prefab Provider**: Determines what prefab to spawn (e.g., random, sequential, rule-based selection from a list).
- **Location Provider**: Determines where objects are spawned (e.g., fixed location, random within bounds, etc.).
- **Spawn Strategy**: Encapsulates all logic about how and when to spawn (e.g., max active, interval, spawn until a limit, spawn indefinitely, spawn in waves, etc.).
- **Object Pooling (optional)**: For performance, consider integrating with an object pool.

## Initial Questions & Answers

1. **Spawned Object Type**
   - Should the spawner support any type of GameObject (prefab), or only specific types? **A: GameObject is fine.**
   - Should it support multiple prefab types at once? **A: List.**
2. **Spawn Strategy**
   - What strategies do you want to support? (e.g., spawn until N, spawn indefinitely, spawn in waves, spawn on event) **A: For now, let's spawn until N.**
   - Should strategies be switchable at runtime? **A: Yes.**
   - Should there be a delay or interval between spawns? **A: Yes. Also, please utilize the existing Timer in the ReusableSystems/Timers.**
3. **Location Provider**
   - What location strategies are needed? (e.g., fixed point, random in area, random on navmesh, from a list of points) **A: Make this generic, for now, I want random in a boundary (3D and 2D).**
   - Should location providers be composable (e.g., random within a set of fixed points)? **A: Can expand and create custom location provider. For now, we'll define random in 3D boundary and random in 2D boundary location provider.**
4. **Despawn/Recycle**
   - Should the system handle despawning or recycling objects? **A: Separated from spawner, make it simple and straight forward.**
   - Should there be a maximum number of active spawned objects? **A: Keep N active, no despawn.**
5. **Events & Callbacks**
   - Should the spawner provide events for spawn/despawn actions? **A: We will see after despawn/recycle has been figured out.**
   - Should it support custom logic on spawn (e.g., initialization)? **A: It should have OnSpawn event first. The rest, let's explore more.**
6. **Editor Integration**
   - Should the system be easily configurable in the Unity Inspector? **A: Yes.**
   - Should it support visual debugging (e.g., gizmos for spawn areas)? **A: Yes.**
7. **Performance**
   - Is object pooling required from the start, or can it be added later? **A: Object pooling is optional.**
   - Should the system support burst/batch spawning for performance? **A: Not now.**
8. **Extensibility**
   - Should users be able to define custom spawn strategies and location providers via scriptable objects or interfaces? **A: Yes.**
   - Should the system be usable both in runtime and in editor (for level design)? **A: We will come back to this later.**

## Design Direction (Updated)

- **Spawner**: Orchestrates the spawning process by querying:
  - **Prefab Provider** for what to spawn
  - **Location Provider** for where to spawn
  - **Spawn Strategy** for how/when to spawn (all spawn rules, such as max active, interval, etc., are encapsulated in the strategy)
- **Prefab Provider**: Handles prefab selection logic (random, sequential, rule-based, etc.).
- **Location Provider**: Handles spawn location logic (random in 2D/3D boundary, etc.).
- **Spawn Strategy**: Handles all timing and conditions for spawning (e.g., keep N active, interval, etc.).
- **Despawn**: Not managed by the spawner; spawner simply keeps N active.
- **Events**: OnSpawn event (reference to spawned object), subscribable by other components.
- **Extensibility**: Custom strategies, prefab providers, and location providers via interfaces or scriptable objects.
- **Editor**: Inspector configuration and gizmo support.

## Class Diagram

```md
classDiagram
    class ISpawnStrategy {
        <<interface>>
        +void Initialize(ISpawnerContext context)
        +void Update(float deltaTime)
        +void Reset()
        +bool ShouldSpawn()
        +int GetMaxActive()
        +float GetSpawnInterval()
    }
    
    class ILocationProvider {
        <<interface>>
        +Vector3 GetSpawnLocation()
    }
    
    class IPrefabProvider {
        <<interface>>
        +GameObject GetNextPrefab()
    }
    
    class Spawner {
        +ISpawnStrategy SpawnStrategy
        +ILocationProvider LocationProvider
        +IPrefabProvider PrefabProvider
        +event Action<GameObject> OnSpawn
        +void StartSpawning()
        +void StopSpawning()
        +void SwitchStrategy(ISpawnStrategy)
        +void SwitchLocationProvider(ILocationProvider)
        +void SwitchPrefabProvider(IPrefabProvider)
    }
    
    class SpawnUntilNStrategy {
        +int N
        +float Interval
        +void Initialize(ISpawnerContext context)
        +void Update(float deltaTime)
        +void Reset()
        +bool ShouldSpawn()
        +int GetMaxActive()
        +float GetSpawnInterval()
    }
    
    class RandomInBoundary3DProvider {
        +Boundary3D Boundary
        +Vector3 GetSpawnLocation()
    }
    
    class RandomInBoundary2DProvider {
        +Boundary2D Boundary
        +Vector3 GetSpawnLocation()
    }
    
    class SequentialPrefabProvider {
        +List<GameObject> Prefabs
        +int CurrentIndex
        +GameObject GetNextPrefab()
    }
    
    class RandomPrefabProvider {
        +List<GameObject> Prefabs
        +GameObject GetNextPrefab()
    }
    
    Spawner --> ISpawnStrategy
    Spawner --> ILocationProvider
    Spawner --> IPrefabProvider
    SpawnUntilNStrategy ..|> ISpawnStrategy
    RandomInBoundary3DProvider ..|> ILocationProvider
    RandomInBoundary2DProvider ..|> ILocationProvider
    SequentialPrefabProvider ..|> IPrefabProvider
    RandomPrefabProvider ..|> IPrefabProvider
```

## Next Steps

- The class diagram and architecture are finalized.
- Proceed to implement code stubs for the following:
  - ISpawnStrategy (interface)
  - ILocationProvider (interface)
  - IPrefabProvider (interface)
  - Spawner (base class)
  - SpawnUntilNStrategy (example strategy)
  - RandomInBoundary3DProvider (example location provider)
  - RandomInBoundary2DProvider (example location provider)
  - SequentialPrefabProvider (example prefab provider)
  - RandomPrefabProvider (example prefab provider)

---

**Proceeding to stub implementation...**
