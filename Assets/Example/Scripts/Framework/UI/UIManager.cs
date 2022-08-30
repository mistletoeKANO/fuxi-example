using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FuXi;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class UIManager : MonoSingleton<UIManager>
{
    private readonly string m_UIRootPath = "Assets/Example/BundleResource/Builtin/UIRoot.prefab";
    private readonly UIFormLayer[] m_FormLayers = new UIFormLayer[6];
    private Camera m_UICamera;
    private EventSystem m_UIEventSystem;

    public Camera UICamera => m_UICamera;
    public EventSystem UIEventSystem => m_UIEventSystem;

    //窗口层级间隔
    internal const int IntervalOfWindow = 10;

    private readonly Dictionary<Type, BasePresenter> d_Windows = new Dictionary<Type, BasePresenter>();

    public async UniTask Init()
    {
        FxAsset fxAsset = await FxAsset.LoadAsync<GameObject>(m_UIRootPath);
        var mUIRoot = (GameObject) Instantiate(fxAsset.asset);
        DontDestroyOnLoad(mUIRoot);
        ObjectReference reference = mUIRoot.GetComponent<ObjectReference>();

        for (int i = 0; i < 6; i++)
        {
            var _L = reference.ObjectReferences[i];
            int min = _L.GetComponent<Canvas>().sortingOrder;
            int max = int.MaxValue;
            if (i < 5)
                max = reference.ObjectReferences[i + 1].GetComponent<Canvas>().sortingOrder;
            m_FormLayers[i] = new UIFormLayer(min, max, _L.transform);
        }

        if (reference.ObjectReferences.Length < 6) return;

        var additionData = Camera.main.GetUniversalAdditionalCameraData();
        m_UICamera = reference.ObjectReferences[6].GetComponent<Camera>();
        additionData.cameraStack.Add(m_UICamera);

        m_UIEventSystem = FindObjectOfType<EventSystem>();
        if (m_UIEventSystem == null)
        {
            var uiEvent = new GameObject("UIEventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            m_UIEventSystem = uiEvent.GetComponent<EventSystem>();
        }

        DontDestroyOnLoad(m_UIEventSystem);
    }
    
    public Action<BasePresenter> onOpenWindow; 
    public Action<BasePresenter> onCloseWindow; 
    
    public async UniTask<TPresenter> OpenWindow<TPresenter>(bool destroyOnSwitchScene = true) 
        where TPresenter : BasePresenter, new()
    {
        if (this.d_Windows.TryGetValue(typeof(TPresenter), out var uiPresenter))
        {
            if (uiPresenter.OpenState == OpenState.Opening)
            {
                return (TPresenter) uiPresenter.tcs.Task.Result;
            }
            if (uiPresenter.OpenState == OpenState.Opened)
            {
                onOpenWindow?.Invoke(uiPresenter);
                return (TPresenter) uiPresenter;
            }
            
            var uiLayer = typeof(TPresenter).GetAttribute<ViewInfo>().layer;
            var uiFormLayer = m_FormLayers[(int) uiLayer];

            this.ShowWindowInternal(uiFormLayer, uiPresenter);
            uiPresenter.tcs.TrySetResult(uiPresenter);
            onOpenWindow?.Invoke(uiPresenter);
            return (TPresenter) uiPresenter.tcs.Task.Result;
        }
        
        var presenterType = typeof(TPresenter);
        var viewInfo = presenterType.GetAttribute<ViewInfo>();
        var formLayer = m_FormLayers[(int) viewInfo.layer];
        
        uiPresenter = new TPresenter();
        d_Windows.Add(presenterType, uiPresenter);

        GameObject go = null;
        if (null != viewInfo.viewType)
        {
            var path = viewInfo.viewType.GetAttribute<PathAttribute>();
            if (null != path)
            {
                go = await AssetManager.Instance.InstantiateAsync(path.PrefabPath, formLayer.root,
                    destroyOnSwitchScene);                
            }
        }

        if (null == go)
        {
            go = new GameObject($"{nameof(TPresenter)}View");
            go.transform.SetParent(formLayer.root);            
        }
        
        await uiPresenter.Init(go, viewInfo.layer);
        
        this.ShowWindowInternal(formLayer, uiPresenter);
        uiPresenter.tcs.TrySetResult(uiPresenter);
        onOpenWindow?.Invoke(uiPresenter);
        return (TPresenter) uiPresenter.tcs.Task.Result;
    }

    private void ShowWindowInternal(UIFormLayer formLayer, BasePresenter window)
    {
        if (window.FormOperation.IsOccludeUi)
        {
            foreach (var mFormLayer in m_FormLayers)
            {
                if (mFormLayer.MaxOrder > formLayer.MaxOrder) continue;
                mFormLayer.CloseStackWindow(window);
            }
        }
        formLayer.ShowWindow(window);
    }

    public T GetWindow<T>() where T : BasePresenter
    {
        if (!this.d_Windows.TryGetValue(typeof(T), out var window))
        {
            Debugger.LogError("Window not open, please open it before get it.");
            return default;
        }
        return (T) window;
    }

    public void CloseWindow<T>() where T : BasePresenter
    {
        CloseWindow(typeof(T));
    }
    
    /// <summary>
    /// 关闭某个层级窗口
    /// </summary>
    /// <param name="uiLayer"></param>
    public void CloseWindow(UILayer uiLayer)
    {
        var _L = m_FormLayers[(int) uiLayer];
        _L.CloseCurLayerWindow();
    }

    internal void CloseWindow(Type type)
    {
        if (!d_Windows.TryGetValue(type, out var window))
        {
            Debugger.ColorError(Color.red, "Window: {0} is not opened! cant close.", type);
            return;
        }

        var _L = m_FormLayers[(int) window.ownLayer];
        this.CloseWindowInternal(_L, window);
        onCloseWindow?.Invoke(window);
    }

    private void CloseWindowInternal(UIFormLayer formLayer, BasePresenter window)
    {
        if (window.FormOperation.IsOccludeUi)
        {
            foreach (var mFormLayer in m_FormLayers)
            {
                if (mFormLayer.MaxOrder > formLayer.MaxOrder) continue;
                mFormLayer.ReOpenStackWindow(window);
            }
        }
        formLayer.CloseWindow(window);
    }

    public void DestroyWithLayer(UILayer uiLayer)
    {
        List<Type> windows = new List<Type>();
        foreach (var window in d_Windows)
        {
            if (window.Value.ownLayer != uiLayer) continue;
            DestroyWindowInternal(window.Value);
            windows.Add(window.Key);
        }

        foreach (var w in windows)
            d_Windows.Remove(w);
    }

    public void DestroyExceptLayers(List<UILayer> layers)
    {
        if (d_Windows.Count == 0) return;
        List<Type> windows = new List<Type>();
        foreach (var window in d_Windows)
        {
            if (layers.Contains(window.Value.ownLayer)) continue;
            if (window.Value.IsDontDestroyOnLoad) continue;
            DestroyWindowInternal(window.Value);
            windows.Add(window.Key);
        }

        foreach (var w in windows)
            d_Windows.Remove(w);
    }

    public void DestroyExceptLayer(UILayer uiLayer)
    {
        if (d_Windows.Count == 0) return;
        List<Type> windows = new List<Type>();
        foreach (var window in d_Windows)
        {
            if (window.Value.ownLayer == uiLayer) continue;
            DestroyWindowInternal(window.Value);
            windows.Add(window.Key);
        }

        foreach (var w in windows)
            d_Windows.Remove(w);
    }

    private void DestroyWindowInternal(BasePresenter window)
    {
        Type type = window.GetType();
        if (window.OpenState == OpenState.Opened) CloseWindow(type);
        var _L = m_FormLayers[(int) window.ownLayer];
        _L.RemoveWindow(type);
        window.OnDestroyInternal();
    }
}