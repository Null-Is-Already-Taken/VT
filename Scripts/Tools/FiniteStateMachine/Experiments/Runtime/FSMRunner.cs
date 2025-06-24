using UnityEngine;

public class FSMRunner : MonoBehaviour
{
    public FSMGraph graph;
    private FSMStateNode currentState;

    void Start()
    {
        currentState = graph.nodes[0] as FSMStateNode;
    }

    void Update()
    {
        if (currentState == null) return;

        for (int i = 0; i < currentState.transitions.Count; i++)
        {
            var transition = currentState.transitions[i];
            if (transition.condition != null && transition.condition.IsMet())
            {
                var port = currentState.GetOutputPort("transitions " + i);
                if (port.IsConnected)
                {
                    currentState = port.Connection.node as FSMStateNode;
                    break;
                }
            }
        }
    }
}
