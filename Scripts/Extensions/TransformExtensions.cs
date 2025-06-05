using UnityEngine;

namespace VT.Extensions
{
    public static class TransformExtensions
    {
        public static bool TryFind(this Transform self, string targetName, out Transform target)
        {
            target = self.Find(targetName);

            return target != null;
        }
    }
}
