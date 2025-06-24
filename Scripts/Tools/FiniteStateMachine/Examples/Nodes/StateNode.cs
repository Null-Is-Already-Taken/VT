using System;
using UnityEngine;
using XNode;

namespace VT.FiniteStateMachine.Examples.Nodes {
	public class StateNode : Node {

		[Input] public Empty enter;
		[Output] public Empty exit;

		public void MoveNext() {
			StateGraph fmGraph = graph as StateGraph;

			if (fmGraph.current != this) {
				Debug.LogWarning("Node isn't active");
				return;
			}

			NodePort exitPort = GetOutputPort("exit");

			if (!exitPort.IsConnected) {
				Debug.LogWarning("Node isn't connected");
				return;
			}

			StateNode node = exitPort.Connection.node as StateNode;
			node.OnEnter();
		}

		public void OnEnter() {
			StateGraph fmGraph = graph as StateGraph;
			fmGraph.current = this;
        }

        public override object GetValue(NodePort port)
        {
            return exit;
        }

        [Serializable]
		public class Empty { }
	}
}