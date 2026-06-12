using System;
using System.Collections.Generic;

public class ServiceLocator
{
    private Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public Dictionary<Type, object> Services => _services;

    public void Register(object service)
    {
        var type = service.GetType();
        if (_services.ContainsKey(type))
        {
            _services[type] = service;
        }
        else
        {
            _services.Add(type, service);
        }

        if (service is IInitializable initializable)
        {
            initializable.Initialize();
        }
    }

    public T Get<T>()
    {
        var type = typeof(T);
        if (_services.TryGetValue(type, out object service))
        {
            return (T)service;
        }

        throw new Exception($"Service {type} is not registered.");
    }

    public bool TryGet<T>(out T service)
    {
        var type = typeof(T);
        if (_services.TryGetValue(type, out object obj))
        {
            service = (T)obj;
            return true;
        }

        service = default;
        return false;
    }

    public void Unregister<T>()
    {
        var type = typeof(T);
        if (_services.TryGetValue(type, out object obj))
        {
            if (obj is IDisposable disposable)
            {
                disposable.Dispose();
            }
            _services.Remove(type);
        }
    }

    public void Clear()
    {
        foreach (var service in _services.Values)
        {
            if (service is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        _services.Clear();
    }
}
