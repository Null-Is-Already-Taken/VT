using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FSMTransition))]
public class FSMTransitionDrawer : PropertyDrawer
{
    private static string[] conditionOptions;
    private static System.Type[] conditionTypes;

    private void LoadConditionTypes()
    {
        if (conditionTypes != null) return;

        var baseType = typeof(FSMCondition);
        var allTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract);

        conditionTypes = allTypes.ToArray();
        conditionOptions = conditionTypes.Select(t => t.Name).ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        LoadConditionTypes();

        var nameProp = property.FindPropertyRelative("name");
        var condProp = property.FindPropertyRelative("condition");

        Rect nameRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(nameRect, nameProp);

        Rect condRect = new(position.x, position.y + 20, position.width * 0.7f, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(condRect, condProp);

        Rect buttonRect = new(position.x + position.width * 0.72f, position.y + 20, position.width * 0.25f, EditorGUIUtility.singleLineHeight);

        if (GUI.Button(buttonRect, "New"))
        {
            //int selectedIndex = 0;

            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < conditionTypes.Length; i++)
            {
                int localIndex = i;
                menu.AddItem(new GUIContent(conditionOptions[i]), false, () =>
                {
                    var instance = ScriptableObject.CreateInstance(conditionTypes[localIndex]);
                    AssetDatabase.AddObjectToAsset(instance, property.serializedObject.targetObject);
                    AssetDatabase.SaveAssets();

                    condProp.objectReferenceValue = instance;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2.4f;
    }
}
