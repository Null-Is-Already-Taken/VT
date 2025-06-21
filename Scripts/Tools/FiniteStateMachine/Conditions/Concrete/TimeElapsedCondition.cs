using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Condition/Time Elapsed")]
public class TimeElapsedCondition : FSMCondition
{
    public float duration = 1f;
    private float startTime;
    private bool initialized = false;

    public override bool IsMet()
    {
        if (!initialized)
        {
            startTime = Time.time;
            initialized = true;
        }

        return Time.time - startTime >= duration;
    }

    public void ResetTimer()
    {
        initialized = false;
    }
}
