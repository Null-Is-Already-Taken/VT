using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Condition/Bool Flag")]
public class BoolFlagCondition : FSMCondition
{
    public bool flag;

    public override bool IsMet() => flag;
}
