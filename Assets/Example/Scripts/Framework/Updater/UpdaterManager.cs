using System;
using Cysharp.Threading.Tasks;

public class UpdaterManager : MonoSingleton<UpdaterManager>
{
    private Messenger m_UpdateHandle;

    public void Init()
    {
        this.m_UpdateHandle = this.gameObject.AddComponent<Messenger>();
    }
    public void AddUpdate(Action action)
    {
        this.m_UpdateHandle.AddListener<UpdateEvent>(action);
    }
    public void RemoveUpdate(Action action)
    {
        this.m_UpdateHandle.RemoveListener<UpdateEvent>(action);
    }
    public void AddFixedUpdate(Action action)
    {
        this.m_UpdateHandle.AddListener<FixedUpdateEvent>(action);
    }
    public void RemoveFixedUpdate(Action action)
    {
        this.m_UpdateHandle.RemoveListener<FixedUpdateEvent>(action);
    }
    public void AddLateUpdate(Action action)
    {
        this.m_UpdateHandle.AddListener<LateUpdateEvent>(action);
    }
    public void RemoveLateUpdate(Action action)
    {
        this.m_UpdateHandle.RemoveListener<LateUpdateEvent>(action);
    }

    #region UnityEvent
    private void Update()
    {
        if (this.m_UpdateHandle == null || !this.m_UpdateHandle.ContainsEvent(typeof(UpdateEvent))) return;
        this.m_UpdateHandle.Invoke<UpdateEvent>();
    }
    private void LateUpdate()
    {
        if (this.m_UpdateHandle == null || !this.m_UpdateHandle.ContainsEvent(typeof(LateUpdateEvent))) return;
        this.m_UpdateHandle.Invoke<LateUpdateEvent>();
    }
    private void FixedUpdate()
    {
        if (this.m_UpdateHandle == null || !this.m_UpdateHandle.ContainsEvent(typeof(FixedUpdateEvent))) return;
        this.m_UpdateHandle.Invoke<FixedUpdateEvent>();
    }
    #endregion
    
    private class UpdateEvent : IEvent { }
    private class FixedUpdateEvent : IEvent { }
    private class LateUpdateEvent : IEvent { }
}