using UnityEngine;
using VT.Extensions;
using VT.Generic;

namespace VT.Gameplay.Spawners
{
    [System.Serializable]
    public class RandomInBoundary3DLocationProvider : LocationProvider
    {
        public Boundary3D Boundary;

        public override Vector3 GetSpawnLocation()
        {
            return Boundary.GetRandomPoint();
        }
    }
} 