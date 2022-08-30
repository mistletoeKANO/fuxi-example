using System;
using System.Collections.Generic;
using UnityEngine;

public class UIFormLayer
{
    internal readonly Transform root;
    private readonly int m_minOrder;
    private readonly int m_maxOrder;
    private int m_CurrentTopOrder;
    internal int MaxOrder => this.m_maxOrder;

    private readonly Dictionary<Type, BasePresenter> d_Windows = new Dictionary<Type, BasePresenter>();
    private readonly Stack<BasePresenter> s_Windows = new Stack<BasePresenter>();
    private readonly Stack<BasePresenter> s_Temp = new Stack<BasePresenter>();
    private readonly Dictionary<BasePresenter, List<BasePresenter>> d_Recorder = new Dictionary<BasePresenter, List<BasePresenter>>();

    public UIFormLayer(int min, int max, Transform root)
    {
        m_minOrder = min;
        m_maxOrder = max;
        m_CurrentTopOrder = min;
        this.root = root;
    }

    public void ShowWindow<T>(T window) where T : BasePresenter
    {
        if (!d_Windows.ContainsKey(typeof(T)))
            d_Windows.Add(typeof(T), window);
        if (s_Windows.Count > 0)
        {
            var topWindow = s_Windows.Peek();
            if (topWindow == window) return;
            m_CurrentTopOrder = topWindow.GetOrder();
        }
        else m_CurrentTopOrder = m_minOrder;
        int nextOrder = m_CurrentTopOrder + UIManager.IntervalOfWindow;
        if (nextOrder >= m_maxOrder)
        {
            RefreshLayer();
            nextOrder = m_CurrentTopOrder + UIManager.IntervalOfWindow;
        }
        window.OnShowInternal(nextOrder);
        m_CurrentTopOrder = nextOrder;
        s_Windows.Push(window);
    }

    public void CloseWindow<T>(T window) where T : BasePresenter
    {
        if (s_Windows.Count == 0) return;
        var topWindow = s_Windows.Peek();
        if (topWindow == window)
        {
            window.OnCloseInternal();
            if (s_Windows.Count == 0) return;
            s_Windows.Pop();
            return;
        }
        this.CloseCenterWindow(window);
    }

    /// <summary>
    /// 关闭 中间层 窗口
    /// </summary>
    /// <param name="window"></param>
    /// <typeparam name="T"></typeparam>
    private void CloseCenterWindow<T>(T window) where T : BasePresenter
    {
        window.OnCloseInternal();
        if (!s_Windows.Contains(window)) return;
        this.s_Temp.Clear();
        var curWindow = s_Windows.Pop();
        while (curWindow != window)
        {
            this.s_Temp.Push(curWindow);
            curWindow = s_Windows.Pop();
        }
        while (this.s_Temp.Count > 0)
        {
            s_Windows.Push(this.s_Temp.Pop());
        }
    }

    internal void CloseCurLayerWindow()
    {
        while (this.s_Windows.Count > 0)
        {
            var window = this.s_Windows.Pop();
            window.OnCloseInternal();
        }
    }
    
    internal void CloseStackWindow(BasePresenter link)
    {
        if (this.s_Windows.Count == 0) return;
        if (!this.d_Recorder.TryGetValue(link, out var windows))
        {
            windows = new List<BasePresenter>();
            this.d_Recorder.Add(link, windows);
        }
        foreach (var window in s_Windows)
        {
            if (window.IsUnRender) continue;
            window.UnRender();
            windows.Add(window);
        }
    }

    internal void ReOpenStackWindow(BasePresenter link)
    {
        if (this.s_Windows.Count == 0) return;
        if (!this.d_Recorder.TryGetValue(link, out var windows)) return;
        foreach (var window in windows)
        {
            window.ReRender();
        }
        this.d_Recorder.Remove(link);
    }

    private void RefreshLayer()
    {
        if (s_Windows.Count == 0) return;
        int topOrder = m_minOrder;
        foreach (var window in s_Windows)
        {
            window.SetOrderInternal(topOrder);
            topOrder += UIManager.IntervalOfWindow;
        }
        m_CurrentTopOrder = s_Windows.Peek().GetOrder();
    }

    internal void RemoveWindow(Type T)
    {
        if (this.d_Windows.TryGetValue(T, out var window))
        {
            this.d_Windows.Remove(T);
        }
        if (window == null) return;
        if (!this.d_Recorder.ContainsKey(window)) return;
        this.d_Recorder.Remove(window);
    }
}