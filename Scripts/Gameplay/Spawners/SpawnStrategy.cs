namespace VT.Gameplay.Spawners
{
    public abstract class SpawnStrategy
    {
        public abstract void Initialize(ISpawnerContext context);
        public abstract void Update(float deltaTime);
        public abstract void Reset();
        public abstract bool ShouldSpawn();
        public abstract int GetMaxActive();
    }
} 