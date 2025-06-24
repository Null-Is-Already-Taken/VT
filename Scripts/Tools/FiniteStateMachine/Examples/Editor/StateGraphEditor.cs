using VT.Logger;
using XNodeEditor;

namespace VT.FiniteStateMachine.Examples.Editor {
	[CustomNodeGraphEditor(typeof(StateGraph))]
	public class StateGraphEditor : NodeGraphEditor {

		/// <summary> 
		/// Overriding GetNodeMenuName lets you control if and how nodes are categorized.
		/// In this example we are sorting out all node types that are not in the XNode.Examples namespace.
		/// </summary>
		public override string GetNodeMenuName(System.Type type)
		{
			if (type.Namespace == "VT.FiniteStateMachine.Examples.Nodes") {
                return base.GetNodeMenuName(type).Replace("VT/Finite State Machine/Examples/Nodes/", "");
			}
			else return null;
		}
	}
}