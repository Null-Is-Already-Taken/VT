using UnityEngine;
using XNode;

namespace VT.FiniteStateMachine.Examples {
	[CreateAssetMenu(fileName = "New State Graph", menuName = "xNode Examples/State Graph")]
	public class StateGraph : NodeGraph {

		// The current "active" node
		public Nodes.StateNode current;

		public void Continue() {
			current.MoveNext();
		}
	}
}