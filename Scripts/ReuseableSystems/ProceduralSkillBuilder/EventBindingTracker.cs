using System.Collections.Generic;
using UnityEngine;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    public class EventBindingTracker : MonoBehaviour
    {
        public HashSet<string> keys = new();
        public List<string> Keys = new();

        public bool TryRegisterKey(string key)
        {
            bool isNew = keys.Add(key);
            if (isNew)
                Keys.Add(key);
            return isNew; // returns false if already added
        }
    }
}