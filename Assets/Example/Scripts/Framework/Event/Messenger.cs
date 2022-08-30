using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
public interface IEvent { }
public interface IAsyncEvent : IEvent { }

public class Messenger : MonoBehaviour
{
    private readonly Dictionary<Type, Delegate> eventDic = new Dictionary<Type, Delegate>();
    private readonly Queue<InvokeAsyncEvent> invokeQueue = new Queue<InvokeAsyncEvent>();
    private float timeSlice = 0.05f;
    private float lastSampleTime = 0.0f;

    private bool isBusy
    {
        get
        {
            var res = this.lastSampleTime - Time.realtimeSinceStartup >= timeSlice;
            if (res) this.lastSampleTime = Time.realtimeSinceStartup;
            return res;
        }
    }

    public void AddListener<T>(Action action, bool repeatable = false) where T : IEvent
    {
        if (!this.eventDic.TryGetValue(typeof(T), out var holder))
        {
            holder = action;
            this.eventDic.Add(typeof(T), holder);
        }
        else
        {
            if (!repeatable && this.DelegateContainsAction<T>(holder, action)) return;
            holder = Delegate.Combine(holder, action);
        }
        this.eventDic[typeof(T)] = holder;
    }
    
    public void AddListener<T>(Action<T> action, bool repeatable = false) where T : IEvent
    {
        if (!this.eventDic.TryGetValue(typeof(T), out var holder))
        {
            holder = action;
            this.eventDic.Add(typeof(T), holder);
        }
        else
        {
            if (!repeatable && this.DelegateContainsAction(holder, action)) return;
            holder = Delegate.Combine(holder, action);
        }
        this.eventDic[typeof(T)] = holder;
    }
    
    public void RemoveListener<T>(Action action) where T : IEvent
    {
        if (!this.eventDic.TryGetValue(typeof(T), out var holder)) return;
        if (!this.DelegateContainsAction<T>(holder, action)) return;
        holder = Delegate.Remove(holder, action);
        if (holder == null)
            this.eventDic.Remove(typeof(T));
        else
            this.eventDic[typeof(T)] = holder;
    }

    public void RemoveListener<T>(Action<T> action) where T : IEvent
    {
        if (!this.eventDic.TryGetValue(typeof(T), out var holder)) return;
        if (!this.DelegateContainsAction(holder, action)) return;
        holder = Delegate.Remove(holder, action);
        if (holder == null)
            this.eventDic.Remove(typeof(T));
        else
            this.eventDic[typeof(T)] = holder;
    }

    public void RemoveListeners<T>() where T : IEvent
    {
        if (!this.eventDic.ContainsKey(typeof(T))) return;
        this.eventDic.Remove(typeof(T));
    }
    
    public void Invoke<T>() where T : IEvent
    {
        if (!this.eventDic.TryGetValue(typeof(T), out var dDelegate))
        {
            Debugger.ColorWarning(Debugger.ColorStyle.Orange, "Event {0} not register.", typeof(T));
            return;
        }
        dDelegate.DynamicInvoke();
    }

    public void Invoke<T>(T v) where T : IEvent
    {
        if (!this.eventDic.TryGetValue(typeof(T), out var dDelegate))
        {
            Debugger.ColorWarning(Debugger.ColorStyle.Orange, "Event {0} not register.", typeof(T));
            return;
        }
        dDelegate.DynamicInvoke(v);
    }

    public void InvokeAsync<T>(T v) where T : IAsyncEvent
    {
        if (!this.eventDic.TryGetValue(typeof(T), out var dDelegate))
        {
            Debugger.ColorWarning(Debugger.ColorStyle.Orange, "Event {0} not register.", typeof(T));
            return;
        }
        var invokeList = dDelegate.GetInvocationList();
        foreach (var @delegate in invokeList)
        {
            InvokeAsyncEvent ivk = null;
            var contains = this.invokeQueue.Any(invokeEvent =>
            {
                if (invokeEvent.action == @delegate)
                {
                    ivk = invokeEvent;
                    return true;
                }
                return false;
            });
            if (!contains)
                this.invokeQueue.Enqueue(new InvokeAsyncEvent {action = @delegate, pars = v});
            else 
                ivk.pars = v;
        }
    }

    public bool ContainsEvent(Type type)
    {
        if (!this.eventDic.TryGetValue(type, out var holder)) return false;
        if (holder == null) return false;
        return holder.GetInvocationList().Length > 0;
    }

    private void Update()
    {
        if (this.invokeQueue.Count == 0) return;
        while (this.invokeQueue.Count > 0)
        {
            var invokeInfo = this.invokeQueue.Dequeue();
            invokeInfo.action?.DynamicInvoke(invokeInfo.pars);
            if (this.isBusy) break;
        }
    }
    
    private bool DelegateContainsAction<T>(Delegate @delegate, Action<T> action)
    {
        var invokeList = @delegate.GetInvocationList();
        return invokeList.Contains(action);
    }
    
    private bool DelegateContainsAction<T>(Delegate @delegate, Action action)
    {
        var invokeList = @delegate.GetInvocationList();
        return invokeList.Contains(action);
    }

    private class InvokeAsyncEvent
    {
        internal Delegate action;
        internal IEvent pars;
    }
}