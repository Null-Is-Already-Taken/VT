using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Condition/Random Chance")]
public class RandomChanceCondition : FSMCondition
{
    [Range(0, 1)]
    public float chance = 0.5f;

    private bool evaluated = false;
    private bool result;

    public override bool IsMet()
    {
        if (!evaluated)
        {
            result = UnityEngine.Random.value < chance;
            evaluated = true;
        }

        return result;
    }

    public void ResetChance()
    {
        evaluated = false;
    }
}
