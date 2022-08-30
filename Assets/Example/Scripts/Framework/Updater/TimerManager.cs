
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 技能系统的计时器
/// </summary>
public class TimerManager : MonoSingleton<TimerManager>
{
    private List<Timer> timers; 
    
    public void Init()
    {
        timers = new List<Timer>();
    }

    private void Awake()
    {
        Init();
    }

    public  void Clear()
    {
    }

    private void Update()
    {
        var dt = Time.deltaTime;
        for (var i = 0; i < timers.Count; i++)
        {
            var timer = timers[i];
            var isAllInvoked = timer.Update(dt);
            if (isAllInvoked)
            {
                timers.RemoveAt(i);
                timer.Clear();
                i--;
            }
        }
    }

    public Timer NewTimer(string id = "")
    {
        var timer = new Timer(id);
        timers.Add(timer);
        return timer;
    }

    public Timer AddTimer(float time, Action callback, string id = "")
    {
        var timer = NewTimer(id);
        timer.AddEvent(time, callback);
        return timer;
    }
}

public class Timer
{
    private float dt = 0f;

    private List<TimerEvent> timerEvents = new List<TimerEvent>();

    /// <summary>
    /// 是否有效
    /// Timer只能一次性设置好所有事件然后再触发。
    /// 不能延时添加Timer，例如在timer的事件中再嵌套Timer.否则可能后续添加的会被清掉。
    /// </summary>
    private bool isValid = true;

    private string id;

    public Timer(string id)
    {
        this.id = id;
    }

    public void Clear()
    {
        isValid = false;
        timerEvents.Clear();
    }

    public void AddEvent(float time, Action callback)
    {
        if (!isValid)
        {
            Debug.LogError("Timer:{id} is not valid.");
            return;
        }
        var timerEvt = TimerEvent.Get(time, callback);
        timerEvents.Add(timerEvt);
    }

    public bool Update(float deltaTime)
    {
        dt += deltaTime;
        var invokedNum = 0;
        for (var i = 0; i < timerEvents.Count; i++)
        {
            var timerEvent = timerEvents[i];
            if (timerEvent == null)
            {
                continue;
            }
            
            if (dt >= timerEvent.Time)
            {
                timerEvent.Invoke();
            }
            
            if (timerEvent.IsInvoked)
            {
                invokedNum++;
            }
        }

        var result = invokedNum == timerEvents.Count;
        return result;
    }

    public class TimerEvent
    {
        private float time;
        public float Time
        {
            get { return time; }
        }
        private Action callback;

        public bool IsInvoked
        {
            get { return null == callback; }
        }

        private TimerEvent(float time, Action callback)
        {
            this.time = time;
            this.callback = callback;
        }

        public void Invoke()
        {
            callback?.Invoke();
            callback = null;
        }

        public static TimerEvent Get(float time, Action callback)
        {
            return new TimerEvent(time, callback);
        }
    }
}