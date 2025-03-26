using System;
using System.Collections.Generic;

namespace _3_Scripts.Infrastructure
{
    public class ServiceLocator
    {
        private readonly Dictionary<Type, object> _services = new();

        public void RegisterService<T>(T service)
        {
            _services[typeof(T)] = service;
        }

        public T GetService<T>()
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }
            throw new Exception($"Service of type {typeof(T).Name} not registered.");
        }
        
        private static readonly Lazy<ServiceLocator> _instance = new(() => new ServiceLocator());
        public static ServiceLocator Instance => _instance.Value;
    }
}