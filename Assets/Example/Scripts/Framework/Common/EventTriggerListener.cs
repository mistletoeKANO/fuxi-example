using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;


/// <summary>
/// 
/// @Author:      XuXiaoLiang
/// @CreateTime:  2018/7/17  17:40
/// @Desc:         
/// </summary>
public class EventTriggerListener : EventTrigger
{
    public Action<GameObject, PointerEventData> onEnter;
    public Action<GameObject, PointerEventData> onExit;
    public Action<GameObject, PointerEventData> onDown;
    public Action<GameObject, PointerEventData> onUp;
    public Action<GameObject, PointerEventData> onClick;
    public Action<GameObject, BaseEventData> onSelect;
    public Action<GameObject, BaseEventData> onUpdateSelect;
    public Action<GameObject, PointerEventData> onInitializePotentialDrag;
    public Action<GameObject, PointerEventData> onBeginDrag;
    public Action<GameObject, PointerEventData> onEndDrag;
    public Action<GameObject, PointerEventData> onDrag;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter(gameObject, eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null) onExit(gameObject, eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null) onDown(gameObject, eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null) onUp(gameObject, eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null) onClick(gameObject, eventData);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(gameObject, eventData);
    }

    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null) onUpdateSelect(gameObject, eventData);
    }

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (onInitializePotentialDrag != null) onInitializePotentialDrag(gameObject, eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null) onBeginDrag(gameObject, eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null) onEndDrag(gameObject, eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null) onDrag(gameObject, eventData);
    }

    public float timeClickGlobal = .2f;

    public void BindClick(Action<GameObject, PointerEventData> onCustomClick, params Transform[] items)
    {
        float timeDown = 0f;
        float timeUp = 0f;

        Action<GameObject, PointerEventData> tOnDowm = (o, data) => { timeDown = Time.time; };
        Action<GameObject, PointerEventData> tOnUp = (o, data) => { timeUp = Time.time; };
        Action<GameObject, PointerEventData> tOnClick = (o, data) =>
        {
            var t = timeUp - timeDown;
            if (t <= timeClickGlobal)
            {
                onCustomClick(o, data);
            }
        };
        onDown += tOnDowm;
        onUp += tOnUp;
        onClick += tOnClick;

        for (var i = 0; i < items.Length; i++)
        {
            var item = items[i];
            if (null != item)
            {
                item.Listener().onDown += tOnDowm;
                item.Listener().onUp += tOnUp;
                item.Listener().onClick += tOnClick;
            }
        }
    }

    /// <summary>
    /// 和ScrollRect绑定一起移动。
    /// </summary>
    /// <param name="scrollRect">绑定的滚动组件</param>
    public void BindToScroll(ScrollRect scrollRect)
    {
        if (null != scrollRect)
        {
            onInitializePotentialDrag += (o, data) => { scrollRect.OnInitializePotentialDrag(data); };
            onBeginDrag += (o, data) => { scrollRect.OnBeginDrag(data); };
            onEndDrag += (o, data) => { scrollRect.OnEndDrag(data); };
            onDrag += (o, data) => { scrollRect.OnDrag(data); };
        }
        else
        {
            Debug.LogError(string.Format("bind {0} with ScrollRect(null)", name));
        }
    }

    public UnityEvent onLongPress = new UnityEvent();
    public UnityEvent onDoubleClick = new UnityEvent();
    private const float DOUBLE_CLICK_TIME = 0.2f;
    private const float PRESS_TIME = 0.5f;

    private bool isDown = false; //按钮是否按下
    private bool isPress = false; //按钮是否按住不放
    private int clickCount = 0; //按钮双击计数
    private float pointDownTimer = 0f; //按钮计时器
    
    public void SetPressEvent(Action func)
    {
        Action<GameObject, PointerEventData> tOnDowm = (o, data) =>
        {
            isDown = true;
            isPress = false;
            pointDownTimer = 0f;
        };
        Action<GameObject, PointerEventData> tOnUp = (o, data) =>
        {
            isDown = false;
        };
        onDown += tOnDowm;
        onUp += tOnUp;
        onLongPress.AddListener(delegate { func(); });
    }

    public void SetPressEvent(Action func, Action upAct)
    {
        Action<GameObject, PointerEventData> tOnDowm = (o, data) =>
        {
            isDown = true;
            isPress = false;
            pointDownTimer = 0f;
        };
        Action<GameObject, PointerEventData> tOnUp = (o, data) =>
        {
            isDown = false;
            upAct?.Invoke();
        };
        onDown += tOnDowm;
        onUp += tOnUp;
        onLongPress.AddListener(delegate { func(); });
    }

    void Update()
    {
        if (isDown)
        {
            pointDownTimer += Time.unscaledDeltaTime;
            if (pointDownTimer >= PRESS_TIME)
            {
                isPress = true;
                isDown = false;
                pointDownTimer = 0f;
                onLongPress?.Invoke();
            }
        }
        //
        // if (clickCount > 0)
        // {
        //     pointDownTimer += Time.unscaledDeltaTime;
        //     if (pointDownTimer >= DOUBLE_CLICK_TIME)
        //     {
        //         if (clickCount < 2)
        //         {
        //             onDown();
        //             onClick();
        //             //m_OnUpEventData = null;
        //         }
        //
        //         clickCount = 0;
        //     }
        //
        //     if (clickCount >= 2)
        //     {
        //         onDoubleClick?.Invoke();
        //         //m_OnUpEventData = null;
        //         clickCount = 0;
        //     }
        // }
    }
}

public static class EventTriggerListenerExtensions
{
    public static EventTriggerListener Listener(this MaskableGraphic target)
    {
        var listener = target.GetComponent<EventTriggerListener>();
        if (listener == null) listener = target.gameObject.AddComponent<EventTriggerListener>();
        return listener;
    }
    
    public static EventTriggerListener Listener(this Transform target)
    {
        var listener = target.GetComponent<EventTriggerListener>();
        if (listener == null) listener = target.gameObject.AddComponent<EventTriggerListener>();
        return listener;
    }

    public static void InvokeExcute(this Toggle toggle, bool isOn = true)
    {
        if (toggle.isOn == isOn)
        {
            toggle.isOn = !isOn;
            toggle.isOn = isOn;
        }
        else
        {
            toggle.isOn = isOn;
        }
    }

    public static void InvokeExcute(this ToggleGroup toggleGroup, Toggle toggle)
    {
        toggleGroup.NotifyToggleOn(toggle);
    }

    /// <summary>
    /// RemoveAll 并且 AddListener
    /// </summary>
    /// <param name="e"></param>
    /// <param name="call"></param>
    public static void AddNewListener(this Button.ButtonClickedEvent e, UnityAction call)
    {
        e.RemoveAllListeners();
        e.AddListener(call);
    }

    /// <summary>
    /// RemoveAll 并且 AddListener
    /// </summary>
    /// <param name="e"></param>
    /// <param name="call"></param>
    public static void Interactable(this Button btn, bool interactable, Action<bool> onInteractableChanged)
    {
        btn.interactable = interactable;
        if (null != onInteractableChanged)
        {
            onInteractableChanged(interactable);
        }
    }
}