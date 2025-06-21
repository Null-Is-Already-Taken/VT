using System.Collections.Generic;
using UnityEngine;
using XNode;

public class FSMStateNode : Node
{
    [Input] public FSMStateNode previous;
    [Output(dynamicPortList = true)] public List<FSMTransition> transitions = new();

    [TextArea] public string stateName;

    public override object GetValue(NodePort port)
    {
        return null;
    }
}
