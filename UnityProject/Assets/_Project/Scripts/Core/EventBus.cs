using System;

namespace EchoZero.Core.Events
{
    /// <summary>
    /// Typed, static publish-subscribe event bus.
    ///
    /// T must be a struct to guarantee allocation-free publishing on the hot path.
    /// All event structs are defined in the EchoZero.Core.Events namespace.
    ///
    /// Usage:
    ///   EventBus&lt;RecallCompletedEvent&gt;.Subscribe(OnRecallCompleted);   // in OnEnable
    ///   EventBus&lt;RecallCompletedEvent&gt;.Unsubscribe(OnRecallCompleted); // in OnDisable
    ///   EventBus&lt;RecallCompletedEvent&gt;.Publish(new RecallCompletedEvent { ... });
    ///
    /// Tests: call EventBus&lt;T&gt;.Clear() in [TearDown] to prevent subscriber leakage.
    /// </summary>
    public static class EventBus<T> where T : struct
    {
        private static Action<T> _onEvent;

        /// <summary>Subscribes a handler to events of type T.</summary>
        public static void Subscribe(Action<T> handler)
        {
            if (handler == null) return;
            _onEvent += handler;
        }

        /// <summary>Unsubscribes a handler from events of type T.</summary>
        public static void Unsubscribe(Action<T> handler)
        {
            if (handler == null) return;
            _onEvent -= handler;
        }

        /// <summary>
        /// Publishes an event to all current subscribers.
        /// No-op if there are no subscribers — does not throw.
        /// </summary>
        public static void Publish(T evt) => _onEvent?.Invoke(evt);

        /// <summary>
        /// Removes all subscribers.
        /// Call in [TearDown] in test classes. NOT for use in production code.
        /// </summary>
        public static void Clear() => _onEvent = null;
    }
}
