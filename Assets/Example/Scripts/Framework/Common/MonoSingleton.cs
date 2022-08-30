using System;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T mInstance = null;
    public static T Instance
    {
        get
        {
            if (mInstance == null && UnityEngine.Application.isPlaying)
            {
                mInstance = GameObject.FindObjectOfType(typeof(T)) as T;
                if (mInstance != null) return mInstance;
                
                GameObject go = new GameObject(typeof(T).Name);
                mInstance = go.AddComponent<T>();
                GameObject parent = GameObject.Find("Boot");
                if (parent == null)
                {
                    parent = new GameObject("Boot");
                    GameObject.DontDestroyOnLoad(parent);
                }
                if (parent != null)
                {
                    go.transform.parent = parent.transform;
                }
                mInstance.OnInit();
            }

            return mInstance;
        }
    }
    protected virtual void OnInit(){}
    /*
     * 没有任何实现的函数，用于保证MonoSingleton在使用前已创建
     */
    public void Startup() { }

    private void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this as T;
        }
        DontDestroyOnLoad(gameObject);
    }
    public void DestroySelf()
    {
        Dispose();
        MonoSingleton<T>.mInstance = null;
        UnityEngine.Object.Destroy(gameObject);
    }
    public virtual void Dispose() { }
}

public abstract class Singleton<T> where T : new()
{
    private static T mInstance;
    public static T Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new T();
            }
            return mInstance;
        }
    }
}