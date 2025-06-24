#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using VT.Editor.GUI;
using VT.Editor.Utils;
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

        private void OnDisable()
        {
        }

        private void OnGUI()
        {
            Label.Draw("Event Bus Monitor", EditorStyles.boldLabel);

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

        readonly Dictionary<Type, object> eventInstances = new(); // Store per-type temp event data

        readonly Dictionary<string, AnimBool> busFoldouts = new();   // remembers toggle state + fade
        readonly Dictionary<string, AnimBool> callbackFoldouts = new();   // remembers toggle state + fade
        readonly Dictionary<string, AnimBool> registeredBindingsFoldouts = new();   // remembers toggle state + fade

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
                key: busType.ToString(),
                foldoutStateCache: busFoldouts,
                repaintCallback: Repaint,
                getItemCountFunc: () => bindings.Count(),
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
                key: $"{busType}.Callback",
                foldoutStateCache: callbackFoldouts,
                repaintCallback: Repaint,
                getItemCountFunc: () => 0,
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
                key: $"{busType}.RegisteredBindings",
                foldoutStateCache: registeredBindingsFoldouts,
                repaintCallback: Repaint,
                getItemCountFunc: () => 0,
                drawContentCallback: () =>
                {
                    foreach (var binding in bindings)
                    {
                        var ownerObj = GetBindingOwner(binding) as UnityEngine.Object;
                        string label = ownerObj ? $"{ownerObj.GetType().Name} ({ownerObj.name})" : "Unknown or null";

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            Label.DrawAutoSized(label, LabelStyles.Label);
                            GUILayout.FlexibleSpace();
                            Button.Draw(
                                content: new GUIContent(EmbeddedIcons.Bell_Unicode),
                                backgroundColor: Color.white,
                                onClick: () => EditorGUIUtility.PingObject(ownerObj),
                                style: ButtonStyles.Inline
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
