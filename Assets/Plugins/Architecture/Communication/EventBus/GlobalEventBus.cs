using System;

public static class GlobalEventBus
{
    private static readonly EventBusImplementation impl = new();

    public static void Subscribe<T>(Action<T> listener)
        => impl.Subscribe(listener);

    public static void Unsubscribe<T>(Action<T> listener)
        => impl.Unsubscribe(listener);

    public static void Publish<T>(T evt)
        => impl.Publish(evt);

    public sealed class EventBusImplementation : EventBusBase { }
}

