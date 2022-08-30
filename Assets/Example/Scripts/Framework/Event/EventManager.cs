using System;

// ReSharper disable once CheckNamespace
public class EventManager : MonoSingleton<EventManager>
{
    private Messenger messenger;
    public void Init()
    {
        this.messenger = this.gameObject.AddComponent<Messenger>();
    }

    public void AddListener<T>(Action<T> action, bool repeatable = false) where T : IEvent
    {
        this.messenger.AddListener<T>(action, repeatable);
    }

    public void RemoveListener<T>(Action<T> action) where T : IEvent
    {
        this.messenger.RemoveListener<T>(action);
    }

    public void RemoveListeners<T>() where T : IEvent
    {
        this.messenger.RemoveListeners<T>();
    }

    public void Invoke<T>(T v = default) where T : IEvent
    {
        this.messenger.Invoke<T>(v);
    }
    
    public void InvokeAsync<T>(T v = default) where T : IAsyncEvent
    {
        this.messenger.InvokeAsync<T>(v);
    }
}