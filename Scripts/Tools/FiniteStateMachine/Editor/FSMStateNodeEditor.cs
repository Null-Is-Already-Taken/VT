//using UnityEditor;
//using UnityEngine;
//using XNodeEditor;

//[CustomNodeEditor(typeof(FSMStateNode))]
//public class FSMStateNodeEditor : NodeEditor
//{
//    public override void OnBodyGUI()
//    {
//        serializedObject.Update();

//        var node = target as FSMStateNode;

//        // Draw inputs/outputs normally
//        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("previous"), true);
//        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("transitions"), true);

//        // Loop over all serialized properties to handle resizable text areas
//        SerializedProperty prop = serializedObject.GetIterator();
//        bool enterChildren = true;

//        while (prop.NextVisible(enterChildren))
//        {
//            enterChildren = false;

//            var field = node.GetType().GetField(prop.name);
//            if (field != null)
//            {
//                var resizable = System.Attribute.GetCustomAttribute(field, typeof(ResizableTextAreaAttribute)) as ResizableTextAreaAttribute;
//                if (resizable != null)
//                {
//                    EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(prop.name));
//                    prop.stringValue = EditorGUILayout.TextArea(
//                        prop.stringValue,
//                        GUILayout.MinHeight(resizable.minHeight),
//                        GUILayout.MaxHeight(resizable.maxHeight)
//                    );
//                    continue;
//                }
//            }

//            // Default field drawer
//            NodeEditorGUILayout.PropertyField(prop, true);
//        }

//        serializedObject.ApplyModifiedProperties();
//    }
//}
