using System;

namespace VT.ReusableSystems.Events
{
    public interface IEventBinding<T>
    {
        Action<T> OnEvent { get; set; }
        Action OnEventNoArgs { get; set; }
        UnityEngine.Object Owner { get; }
    }

    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        private Action<T> onEvent = _ => { };
        private Action onEventNoArgs = () => { };
        private UnityEngine.Object owner;

        Action<T> IEventBinding<T>.OnEvent
        {
            get => onEvent;
            set => onEvent = value;
        }

        Action IEventBinding<T>.OnEventNoArgs
        {
            get => onEventNoArgs;
            set => onEventNoArgs = value;
        }

        public EventBinding(UnityEngine.Object owner, Action<T> onEvent)
        {
            this.owner = owner;
            this.onEvent = onEvent;
        }

        public EventBinding(UnityEngine.Object owner, Action onEventNoArgs)
        {
            this.owner = owner;
            this.onEventNoArgs = onEventNoArgs;
        }

        public UnityEngine.Object Owner => owner;

        public void Add(Action onEvent) => onEventNoArgs += onEvent;
        public void Remove(Action onEvent) => onEventNoArgs -= onEvent;

        public void Add(Action<T> onEvent) => this.onEvent += onEvent;
        public void Remove(Action<T> onEvent) => this.onEvent -= onEvent;
    }
}
