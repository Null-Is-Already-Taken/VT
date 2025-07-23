using UnityEngine;
using VT.Extensions;
using VT.Generic;

namespace VT.Gameplay.Spawners
{
    [System.Serializable]
    public class RandomInBoundary2DProvider : LocationProvider
    {
        public Boundary2D Boundary;

        public override Vector3 GetSpawnLocation()
        {
            Vector2 pt = Boundary.GetRandomPoint();
            return new(pt.x, pt.y, 0f);
        }
    }
} 