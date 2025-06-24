using UnityEngine;

public enum ComparisonType { Equal, NotEqual, Greater, Less, GreaterOrEqual, LessOrEqual }

[CreateAssetMenu(menuName = "FSM/Condition/Compare Float")]
public class CompareFloatCondition : FSMCondition
{
    public float lhs;
    public float rhs;
    public ComparisonType comparison;

    public override bool IsMet()
    {
        return comparison switch
        {
            ComparisonType.Equal => lhs == rhs,
            ComparisonType.NotEqual => lhs != rhs,
            ComparisonType.Greater => lhs > rhs,
            ComparisonType.Less => lhs < rhs,
            ComparisonType.GreaterOrEqual => lhs >= rhs,
            ComparisonType.LessOrEqual => lhs <= rhs,
            _ => false
        };
    }
}
