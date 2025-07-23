using UnityEngine;

namespace VT.Gameplay.Spawners
{
    public abstract class LocationProvider
    {
        public abstract Vector3 GetSpawnLocation();
    }
} 