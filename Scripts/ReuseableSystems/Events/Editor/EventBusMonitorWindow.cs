#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using VT.Editor.GUI;
using VT.Logger;

namespace VT.ReusableSystems.Events.Editor
{
    public class EventBusMonitorWindow : EditorWindow
    {
        private Vector2 scroll;

        private class CallbackSectionKey { }
        private class BindingSectionKey { }

        [MenuItem("Tools/EventBus/Monitor")]
        public static void Open()
        {
            GetWindow<EventBusMonitorWindow>("Event Bus Monitor");
        }

        private void OnEnable()
        {
            InternalLogger.Instance.LogDebug("EventBusMonitorWindow enabled. Initializing EventBusUtil.");
            EventBusUtil.Initialize(); // Ensure initialization is done before we start monitoring
        }

        private void OnGUI()
        {
            if (EventBusUtil.EventBusTypes == null || EventBusUtil.EventBusTypes.Count == 0)
            {
                EditorGUILayout.HelpBox("No EventBus types registered. Make sure EventBusUtil.Initialize() has run.", MessageType.Warning);
                return;
            }

            using var sv = new EditorGUILayout.ScrollViewScope(scroll);
            scroll = sv.scrollPosition;

            foreach (var busType in EventBusUtil.EventBusTypes)
            {
                DrawEventBusEntry(busType);
            }
        }

        // Store per-type temp event data
        readonly Dictionary<Type, object> eventInstances = new();

        // remembers toggle state + fade
        readonly Dictionary<string, AnimBool> busFoldouts = new();
        readonly Dictionary<string, AnimBool> callbackFoldouts = new();
        readonly Dictionary<string, AnimBool> registeredBindingsFoldouts = new();

        private void DrawEventBusEntry(Type busType)
        {
            Type eventType = busType.GetGenericArguments()[0];
            string typeName = eventType.Name;

            var bindingsField = busType.GetField("bindings", BindingFlags.NonPublic | BindingFlags.Static);
            var bindings = bindingsField?.GetValue(null) as IEnumerable<object>;
            if (bindings != null && !bindings.Any())
            {
                //EditorGUILayout.HelpBox($"No bindings registered for {typeName}.", MessageType.Info);
                return;
            }

            // Foldout body for each bound event bus t
            Foldout.Draw(
                title: typeName,
                tooltip: $"Event Bus for {typeName}",
                key: busType.ToString(),
                foldoutStateCache: busFoldouts,
                repaintCallback: Repaint,
                subScriptGetter: () => $"Subscriber: {bindings.Count()}",
                drawContentCallback: () =>
                {
                    // Nested: Callback
                    DrawCallbackSection(busType, bindings, eventType, typeName);

                    // Nested: Registered Bindings
                    DrawRegisteredBindingsSection(busType, bindings);
                }
            );
        }

        private void DrawCallbackSection(Type busType, IEnumerable<object> bindings, Type eventType, string typeName)
        {
            // cache / create an instance of the event so we can edit its fields
            if (!eventInstances.TryGetValue(eventType, out var evt))
            {
                evt = Activator.CreateInstance(eventType);
                eventInstances[eventType] = evt;
            }

            Foldout.Draw(
                title: "Callback",
                tooltip: $"Edit and raise {typeName} event",
                key: $"{busType}.Callback",
                foldoutStateCache: callbackFoldouts,
                repaintCallback: Repaint,
                subScriptGetter: null,
                drawContentCallback: () =>
                {
                    foreach (var field in eventType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                    {
                        object current = field.GetValue(evt);
                        object updated = GenericFields.Draw(field.FieldType, field.Name, current);
                        field.SetValue(evt, updated);
                    }

                    Button.Draw(
                        label: $"Raise {typeName}",
                        backgroundColor: Color.white,
                        onClick: () =>
                        {
                            RaiseEvent(busType, bindings, eventType, evt);
                        }
                    );
                },
                defaultFoldoutState: false
            );
        }

        private void DrawRegisteredBindingsSection(Type busType, IEnumerable<object> bindings)
        {
            Foldout.Draw(
                title: "Registered Bindings",
                tooltip: $"List of registered bindings for {busType.Name}",
                key: $"{busType}.RegisteredBindings",
                foldoutStateCache: registeredBindingsFoldouts,
                repaintCallback: Repaint,
                subScriptGetter: null,
                drawContentCallback: () =>
                {
                    foreach (var binding in bindings)
                    {
                        var ownerObj = GetBindingOwner(binding) as UnityEngine.Object;
                        string label = ownerObj ? $"{ownerObj.GetType().Name} ({ownerObj.name})" : "Unknown or null";

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            Button.Draw(
                                content: new GUIContent(label),
                                backgroundColor: Color.white,
                                onClick: () => {
                                    EditorGUIUtility.PingObject(ownerObj);
                                },
                                style: ButtonStyles.MiniButtonMid
                            );
                        }
                    }
                }
            );
        }

        private void RaiseEvent(Type busType, IEnumerable<object> bindings, Type eventType, object evtInstance)
        {
            try
            {
                var raiseMethod = busType.GetMethod("Raise", BindingFlags.Static | BindingFlags.Public);
                raiseMethod?.Invoke(null, new object[] { evtInstance });

                Debug.Log($"Raised {eventType.Name}");

                if (bindings == null) return;

                foreach (var binding in bindings)
                {
                    var owner = GetBindingOwner(binding);
                    Debug.Log($"Event received by: {owner ?? "Unknown"}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to raise event: {ex.Message}");
            }
        }

        private object GetBindingOwner(object binding)
        {
            return binding.GetType().GetProperty("Owner", BindingFlags.Public | BindingFlags.Instance)?.GetValue(binding);
        }
    }
}

#endif
