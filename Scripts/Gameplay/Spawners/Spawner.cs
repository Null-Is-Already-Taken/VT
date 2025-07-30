using System.Collections;
using UnityEngine;

namespace VT.Gameplay.Spawners
{
    public class Spawner : MonoBehaviour, ISpawnerContext
    {
        [Header("Providers / Strategy")]
        [SerializeReference] private SpawnStrategy spawnStrategy;
        [SerializeReference] private LocationProvider locationProvider;
        [SerializeReference] private PrefabProvider prefabProvider;

        private int activeCount = 0;
        public int ActiveCount => activeCount;

        private Coroutine spawnRoutine;
        private bool isSpawning = false;

        private readonly SpawnRegistrar spawnRegistrar = new();
        private readonly DespawnRegistrar despawnRegistrar = new();

        private void OnEnable()
        {
            SwitchStrategy(spawnStrategy);
            SwitchPrefabProvider(prefabProvider);
            StartSpawning();
        }

        private void DespawnEventHandler(GameObject item)
        {
            activeCount = Mathf.Max(0, ActiveCount - 1);
            Debug.Log($"Spawner: Despawned {item.name}. Active count: {ActiveCount}");
        }

        private void SpawnEventHandler(GameObject item)
        {
            activeCount += 1;
            Debug.Log($"Spawner: Spawned {item.name}. Active count: {ActiveCount}");
        }

        private void OnDisable()
        {
            StopSpawning();
            spawnStrategy?.Reset();
            spawnRegistrar.Detach(prefabProvider, SpawnEventHandler);
            despawnRegistrar.Detach(prefabProvider, DespawnEventHandler);
        }

        public void StartSpawning()
        {
            if (isSpawning) return;

            isSpawning = true;
            spawnRoutine = StartCoroutine(SpawnLoop());
        }

        public void StopSpawning()
        {
            if (!isSpawning) return;

            isSpawning = false;
            if (spawnRoutine != null) StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                if (!isSpawning) yield break;

                if (spawnStrategy == null || locationProvider == null || prefabProvider == null)
                {
                    yield return null;
                    continue;
                }

                float dt = Time.deltaTime;
                spawnStrategy.Update(dt);

                if (spawnStrategy.ShouldSpawn())
                {
                    Vector3 position = locationProvider.GetSpawnLocation();
                    GameObject obj = prefabProvider.GetPrefab();
                    if (obj != null)
                    {
                        obj.transform.SetPositionAndRotation(position, Quaternion.identity);
                    }
                }

                yield return null;
            }
        }

        public void SwitchStrategy(SpawnStrategy strategy)
        {
            spawnStrategy = strategy;
            spawnStrategy.Initialize(this);
        }

        public void SwitchLocationProvider(LocationProvider provider)
        {
            locationProvider = provider;
        }

        public void SwitchPrefabProvider(PrefabProvider provider)
        {
            if (provider == null) return;

            spawnRegistrar.Detach(prefabProvider, SpawnEventHandler);
            despawnRegistrar.Detach(prefabProvider, DespawnEventHandler);

            prefabProvider = provider;
            prefabProvider.Initialize();
            
            spawnRegistrar.Attach(prefabProvider, SpawnEventHandler);
            despawnRegistrar.Attach(prefabProvider, DespawnEventHandler);
        }
    }
}
