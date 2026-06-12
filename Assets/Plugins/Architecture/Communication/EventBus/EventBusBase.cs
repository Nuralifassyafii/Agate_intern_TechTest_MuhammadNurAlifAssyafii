using System;
using System.Collections.Generic;

public class EventBusBase
{
    protected readonly Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();

    public void Subscribe<T>(Action<T> action)
    {
        var type = typeof(T);

        if (_events.TryGetValue(type, out var existing))
        {
            _events[type] = Delegate.Combine(existing, action);
        }
        else
        {
            _events[type] = action;
        }
    }

    public void Unsubscribe<T>(Action<T> action)
    {
        var type = typeof(T);
        if (!_events.TryGetValue(type, out var existing))
        {
            return;
        }
        var updated = Delegate.Remove(existing, action);
        if (updated == null)
        {
            _events.Remove(type);
        }
        else
        {
            _events[type] = updated;
        }
    }

    public void Publish<TMessage>(TMessage message)
    {
        var type = typeof(TMessage);
        if (_events.TryGetValue(type, out Delegate evt))
        {
            ((Action<TMessage>)evt)?.Invoke(message);
        }
    }

    public void Clear()
    {
        _events.Clear();
    }
}
