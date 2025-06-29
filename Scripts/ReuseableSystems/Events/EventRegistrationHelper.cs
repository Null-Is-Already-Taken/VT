using System;
using UnityEngine;

namespace VT.ReusableSystems.Events
{
    /// <summary>
    /// Utility class for simplifying event registration and management.
    /// Provides helper methods to register for specific events with automatic lifecycle management.
    /// </summary>
    public static class EventRegistrationHelper
    {
        /// <summary>
        /// Registers for a specific event type with automatic binding creation and lifecycle management.
        /// </summary>
        /// <typeparam name="T">The event type to register for</typeparam>
        /// <param name="owner">The MonoBehaviour that owns the registration</param>
        /// <param name="callback">The callback to invoke when the event is raised</param>
        /// <param name="binding">Reference to store the created binding (for deregistration)</param>
        public static void RegisterForEvent<T>(MonoBehaviour owner, Action<T> callback, out EventBinding<T> binding) where T : IEvent
        {
            binding = new EventBinding<T>(owner, callback);
            EventBus<T>.Register(binding);
        }

        /// <summary>
        /// Registers for a specific event type using the existing BindAndRegister pattern.
        /// </summary>
        /// <typeparam name="T">The event type to register for</typeparam>
        /// <param name="owner">The MonoBehaviour that owns the registration</param>
        /// <param name="callback">The callback to invoke when the event is raised</param>
        /// <param name="binding">Reference to store the created binding (for deregistration)</param>
        public static void BindAndRegisterEvent<T>(MonoBehaviour owner, Action<T> callback, out EventBinding<T> binding) where T : IEvent
        {
            binding = null;
            EventBus<T>.BindAndRegister(binding, callback, owner);
        }

        /// <summary>
        /// Deregisters from an event using the stored binding.
        /// </summary>
        /// <typeparam name="T">The event type to deregister from</typeparam>
        /// <param name="binding">The binding to deregister</param>
        public static void DeregisterFromEvent<T>(EventBinding<T> binding) where T : IEvent
        {
            if (binding != null)
            {
                EventBus<T>.Deregister(binding);
            }
        }

        /// <summary>
        /// Registers for multiple events at once using a single method call.
        /// </summary>
        /// <typeparam name="T1">First event type</typeparam>
        /// <typeparam name="T2">Second event type</typeparam>
        /// <param name="owner">The MonoBehaviour that owns the registrations</param>
        /// <param name="callback1">Callback for first event</param>
        /// <param name="callback2">Callback for second event</param>
        /// <param name="binding1">Binding for first event</param>
        /// <param name="binding2">Binding for second event</param>
        public static void RegisterForEvents<T1, T2>(
            MonoBehaviour owner,
            Action<T1> callback1, Action<T2> callback2,
            out EventBinding<T1> binding1, out EventBinding<T2> binding2)
            where T1 : IEvent where T2 : IEvent
        {
            RegisterForEvent(owner, callback1, out binding1);
            RegisterForEvent(owner, callback2, out binding2);
        }

        /// <summary>
        /// Registers for three events at once using a single method call.
        /// </summary>
        /// <typeparam name="T1">First event type</typeparam>
        /// <typeparam name="T2">Second event type</typeparam>
        /// <typeparam name="T3">Third event type</typeparam>
        /// <param name="owner">The MonoBehaviour that owns the registrations</param>
        /// <param name="callback1">Callback for first event</param>
        /// <param name="callback2">Callback for second event</param>
        /// <param name="callback3">Callback for third event</param>
        /// <param name="binding1">Binding for first event</param>
        /// <param name="binding2">Binding for second event</param>
        /// <param name="binding3">Binding for third event</param>
        public static void RegisterForEvents<T1, T2, T3>(
            MonoBehaviour owner,
            Action<T1> callback1, Action<T2> callback2, Action<T3> callback3,
            out EventBinding<T1> binding1, out EventBinding<T2> binding2, out EventBinding<T3> binding3)
            where T1 : IEvent where T2 : IEvent where T3 : IEvent
        {
            RegisterForEvent(owner, callback1, out binding1);
            RegisterForEvent(owner, callback2, out binding2);
            RegisterForEvent(owner, callback3, out binding3);
        }

        /// <summary>
        /// Deregisters from multiple events at once.
        /// </summary>
        /// <typeparam name="T1">First event type</typeparam>
        /// <typeparam name="T2">Second event type</typeparam>
        /// <param name="binding1">Binding for first event</param>
        /// <param name="binding2">Binding for second event</param>
        public static void DeregisterFromEvents<T1, T2>(EventBinding<T1> binding1, EventBinding<T2> binding2)
            where T1 : IEvent where T2 : IEvent
        {
            DeregisterFromEvent(binding1);
            DeregisterFromEvent(binding2);
        }

        /// <summary>
        /// Deregisters from three events at once.
        /// </summary>
        /// <typeparam name="T1">First event type</typeparam>
        /// <typeparam name="T2">Second event type</typeparam>
        /// <typeparam name="T3">Third event type</typeparam>
        /// <param name="binding1">Binding for first event</param>
        /// <param name="binding2">Binding for second event</param>
        /// <param name="binding3">Binding for third event</param>
        public static void DeregisterFromEvents<T1, T2, T3>(EventBinding<T1> binding1, EventBinding<T2> binding2, EventBinding<T3> binding3)
            where T1 : IEvent where T2 : IEvent where T3 : IEvent
        {
            DeregisterFromEvent(binding1);
            DeregisterFromEvent(binding2);
            DeregisterFromEvent(binding3);
        }
    }
} 