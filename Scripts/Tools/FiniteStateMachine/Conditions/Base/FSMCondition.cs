using UnityEngine;

public abstract class FSMCondition : ScriptableObject
{
    public abstract bool IsMet();
}
