using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 窗口开启状态
/// </summary>
public enum OpenState : byte
{
    Opened  = 0,
    Opening = 1,
    Closed  = 2,
}

/// <summary>
/// MVP中的P，持有view。
/// 负责维护数据和表现之间的关系。
/// </summary>
public abstract class BasePresenter
{
    protected BaseView view;
    internal UILayer ownLayer = UILayer.Normal;

    private OpenState openState;
    private bool isUnRender;

    public bool IsDontDestroyOnLoad { get; protected set; } = false;
    
    internal OpenState OpenState => openState;
    internal bool IsUnRender => isUnRender;
    internal readonly System.Threading.Tasks.TaskCompletionSource<BasePresenter> tcs 
        = new System.Threading.Tasks.TaskCompletionSource<BasePresenter>();

    internal UIFormOperation FormOperation => this.view?.formOperation;

    internal async UniTask Init(GameObject target, UILayer uiLayer)
    {
        ownLayer = uiLayer;
        if (null == target) return;
        
        var viewInfo = GetType().GetAttribute<ViewInfo>();
        if (null != viewInfo.viewType)
        {
            view = (BaseView) Activator.CreateInstance(viewInfo.viewType);
            view.Init(target);
            view.FindAndSaveComponents();            
        }
        await OnCreate();
        if (null != FormOperation && FormOperation.CloseBtn != null)
        {
            if (FormOperation.CloseBtn.onClick.GetPersistentEventCount() == 0)
                FormOperation.CloseBtn.onClick.AddListener(this.OnCloseButton);
        }
    }

    protected virtual UniTask OnCreate() { return UniTask.CompletedTask;}
    protected virtual void OnShow() { }
    protected virtual void OnDisable() { }

    protected virtual void OnDestroy() { }

    private void OnCloseButton()
    {
        UIManager.Instance.CloseWindow(this.GetType());
    }

    internal void OnShowInternal(int order)
    {
        this.openState = OpenState.Opened;
        if (null == view || view.target == null) return;
        SetOrderInternal(order);
        OnShow();
        view.target.SetActive(true);
    }

    internal void OnCloseInternal()
    {
        this.openState = OpenState.Closed;
        if (null == view || view.target == null) return;
        view.target.SetActive(false);
        OnDisable();
    }

    internal void OnDestroyInternal()
    {
        if (null == view || view.target == null) return;
        Object.DestroyImmediate(view.target);
        OnDestroy();
    }

    internal int GetOrder() => view.canvas.sortingOrder;

    internal void SetOrderInternal(int order)
    {
        if (view.canvas == null) return;
        view.canvas.sortingOrder = order;
    }

    internal void UnRender()
    {
        if (view.target == null) return;
        view.target.layer = LayerMask.NameToLayer("Default");
        this.isUnRender = true;
        if (null == view.graphicRaycaster) return;
        view.graphicRaycaster.enabled = false;
    }

    internal void ReRender()
    {
        if (view.target == null) return;
        view.target.layer = LayerMask.NameToLayer("UI");
        this.isUnRender = false;
        if (null == view.graphicRaycaster) return;
        view.graphicRaycaster.enabled = true;
    }
}