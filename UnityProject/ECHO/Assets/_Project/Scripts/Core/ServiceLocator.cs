using System;
using System.Collections.Generic;

namespace EchoZero.Core
{
    /// <summary>
    /// Simple explicit service registry.
    /// All services are registered once in Bootstrap before any gameplay begins.
    /// No reflection, no auto-wiring, no magic.
    ///
    /// Usage:
    ///   ServiceLocator.Register<ISaveService>(new SaveService());
    ///   var svc = ServiceLocator.Get<ISaveService>();
    ///
    /// Tests: call ServiceLocator.Clear() in [TearDown] to prevent test pollution.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        /// <summary>
        /// Registers a service implementation for interface type T.
        /// If T is already registered, the new implementation replaces the old one.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when service is null.</exception>
        public static void Register<T>(T service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service),
                    $"[ServiceLocator] Cannot register null for type {typeof(T).Name}.");
            _services[typeof(T)] = service;
        }

        /// <summary>
        /// Returns the registered implementation of type T.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if T has not been registered. Check Bootstrap for missing registration.
        /// </exception>
        public static T Get<T>()
        {
            if (_services.TryGetValue(typeof(T), out var svc))
                return (T)svc;

            throw new InvalidOperationException(
                $"[ServiceLocator] Service of type '{typeof(T).Name}' is not registered. " +
                $"Register it in Bootstrap.Awake() before any MonoBehaviour uses it.");
        }

        /// <summary>
        /// Attempts to retrieve the registered implementation of T without throwing.
        /// Returns false if T is not registered.
        /// </summary>
        public static bool TryGet<T>(out T service)
        {
            if (_services.TryGetValue(typeof(T), out var svc))
            {
                service = (T)svc;
                return true;
            }
            service = default;
            return false;
        }

        /// <summary>
        /// Removes all registered services.
        /// Call in [TearDown] in test classes to prevent test pollution.
        /// NOT for use in production code.
        /// </summary>
        public static void Clear() => _services.Clear();

        /// <summary>
        /// Removes a specific registered service of type T.
        /// Call in OnDestroy() of MonoBehaviour services to clean up gracefully.
        /// </summary>
        public static void Unregister<T>()
        {
            _services.Remove(typeof(T));
        }
    }
}
