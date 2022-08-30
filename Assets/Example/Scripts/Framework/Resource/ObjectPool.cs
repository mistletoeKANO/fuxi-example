using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ObjectPool : MonoSingleton<ObjectPool>
{
    // 池子容量，满容量时触发强制清理 活跃值低于 0.5 * ActiveFactory 的 池子
    private const ushort Capacity = 128;
    // 单个池子容量, 满时触发强制清理 清理当前池子 80% 对象
    private const ushort CapacityOnePool = 256;
    // 默认活跃值
    private const float ActiveFactory = 300;
    // 采样间隔
    private const float SampleSlice = 5;
    // 上一次采样时间
    private float lastSampleTime = 0;
    // 缓存池
    private readonly Dictionary<string, Queue<GameObject>> m_Pools =  new Dictionary<string, Queue<GameObject>>();
    // 活跃池, 记录活跃状态 缓存对象，非活跃的将 定时自动清理。规则: 初始访问对象 活跃值为 300，每隔一秒自动减一，直到0时自动销毁。被访问之后 刷新活跃值
    private readonly Dictionary<string, float> m_ActivePool = new Dictionary<string, float>();

    private Transform m_Root;
    private const string m_PoolCacheRoot = "PoolCacheRoot";

    protected override void OnInit()
    {
        this.m_Root = new GameObject(m_PoolCacheRoot).transform;
        this.m_Root.position = new Vector3(10000, 10000);
        Object.DontDestroyOnLoad(this.m_Root);
    }

    private bool TryGetFromPool(string path, out GameObject instance)
    {
        var res = false; instance = default;
        if (this.m_Pools.TryGetValue(path, out var queue))
        {
            if (queue.Count > 0)
            {
                instance = queue.Dequeue();
                if (!instance.activeSelf) instance.SetActive(true);
                res = true;
            }
            if (!this.m_ActivePool.TryGetValue(path, out _))
                this.m_ActivePool.Add(path, ActiveFactory);
            else
                this.m_ActivePool[path] = ActiveFactory;
        }
        return res;
    }

    public GameObject Instantiate(string path, Transform root)
    {
        if (!this.TryGetFromPool(path, out var instance))
        {
            var obj = AssetManager.Instance.LoadAsset<GameObject>(path);
            instance = Object.Instantiate(obj, root);
        }
        else instance.transform.SetParent(root);
        return instance;
    }

    public List<GameObject> Instantiate(string path, Transform root, int num)
    {
        List<GameObject> res = new List<GameObject>();
        if (num <= 0) num = 1;
        for (int i = 0; i < num; i++)
        {
            var instance = this.Instantiate(path, root);
            res.Add(instance);
        }
        return res;
    }
    
    public async UniTask<GameObject> InstantiateAsync(string path, Transform root)
    {
        if (!this.TryGetFromPool(path, out var instance))
        {
            var obj = await AssetManager.Instance.LoadAssetAsync<GameObject>(path);
            instance = Object.Instantiate(obj, root);
        }
        else instance.transform.SetParent(root);
        return instance;
    }

    public async UniTask<List<GameObject>> InstantiateAsync(string path, Transform root, int num)
    {
        List<GameObject> res = new List<GameObject>();
        if (num <= 0) num = 1;
        for (int i = 0; i < num; i++)
        {
            var instance = await this.InstantiateAsync(path, root);
            res.Add(instance);
        }
        return res;
    }

    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="path"></param>
    /// <param name="instance"></param>
    /// <param name="activeFalse">普通不带特效对象回收建议保持默认值，特效对象回收建议false,视野外特效active时依然会有消耗</param>
    public void RecycleObject(string path, GameObject instance, bool activeFalse = true)
    {
        if (string.IsNullOrEmpty(path)) return;
        if (!this.m_Pools.TryGetValue(path, out var queue))
        {
            queue = new Queue<GameObject>();
            this.m_Pools.Add(path, queue);
        }
        if (!this.m_ActivePool.TryGetValue(path, out _))
            this.m_ActivePool.Add(path, ActiveFactory);
        else
            this.m_ActivePool[path] = ActiveFactory;
        if (!activeFalse) instance.SetActive(false);
        instance.transform.SetParent(this.m_Root);
        instance.transform.localPosition = Vector3.zero;
        queue.Enqueue(instance);
        if (queue.Count > CapacityOnePool)
            this.DestroyOutOfOnePoolCapacity(path);
        else
            this.DestroyOutOfCapacity();
    }
    
    /// <summary>
    /// 批量回收对象
    /// </summary>
    /// <param name="path"></param>
    /// <param name="instances"></param>
    /// <param name="activeFalse">普通不带特效对象回收建议保持默认值，特效对象回收建议false,视野外特效active时依然会有消耗</param>
    public void RecycleObjects(string path, List<GameObject> instances, bool activeFalse = true)
    {
        if (string.IsNullOrEmpty(path)) return;
        if (!this.m_Pools.TryGetValue(path, out var queue))
        {
            queue = new Queue<GameObject>();
            this.m_Pools.Add(path, queue);
        }
        if (!this.m_ActivePool.TryGetValue(path, out _))
            this.m_ActivePool.Add(path, ActiveFactory);
        else
            this.m_ActivePool[path] = ActiveFactory;
        foreach (var instance in instances)
        {
            if (!activeFalse) instance.SetActive(false);
            instance.transform.SetParent(this.m_Root);
            instance.transform.localPosition = Vector3.zero;
            queue.Enqueue(instance);
        }
        if (queue.Count > CapacityOnePool)
            this.DestroyOutOfOnePoolCapacity(path);
        else
            this.DestroyOutOfCapacity();
    }

    private async void DestroyOutOfCapacity()
    {
        if (this.m_Pools.Count < Capacity) return;
        await UniTask.NextFrame();
        if (this.m_Pools.Count < Capacity) return;
        List<string> dirtyPool = new List<string>();
        // 清理 活跃值 低于 50% 的池子
        float factory = ActiveFactory * 0.5f;
        foreach (var pool in m_ActivePool)
        {
            if (pool.Value > factory) continue;
            dirtyPool.Add(pool.Key);
        }
        foreach (var path in dirtyPool)
        {
            if (!this.m_Pools.TryGetValue(path, out var queue)) continue;
            while (queue.Count > 0)
                Object.Destroy(queue.Dequeue());
            this.m_Pools.Remove(path);
            this.m_ActivePool.Remove(path);
        }
    }
    
    private async void DestroyOutOfOnePoolCapacity(string path)
    {
        await UniTask.NextFrame();
        if (!this.m_Pools.TryGetValue(path, out var queue)) return;
        // 池子满了，清理当前池子 80% 对象
        int dirtyCount = Mathf.CeilToInt(queue.Count * 0.8f);
        while (dirtyCount > 0)
        {
            dirtyCount--;
            Object.Destroy(queue.Dequeue());
        }
    }

    private void Update()
    {
        var duration = Time.realtimeSinceStartup - this.lastSampleTime;
        if (duration < SampleSlice) return;
        this.lastSampleTime = Time.realtimeSinceStartup;
        this.Sample();
        this.RecycleNonActiveObject();
    }

    // 定时采样活跃池子
    private void Sample()
    {
        if (this.m_ActivePool.Count == 0) return;
        List<string> keys = this.m_ActivePool.Keys.ToList();
        foreach (var path in keys)
            this.m_ActivePool[path] -= SampleSlice;
    }

    private void RecycleNonActiveObject()
    {
        List<string> dirtyPool = new List<string>();
        foreach (var pool in this.m_ActivePool)
        {
            if (pool.Value > 0) continue;
            dirtyPool.Add(pool.Key);
        }
        foreach (var path in dirtyPool)
        {
            if (!this.m_Pools.TryGetValue(path, out var queue)) continue;
            while (queue.Count > 0)
                Object.Destroy(queue.Dequeue());
            this.m_Pools.Remove(path);
            this.m_ActivePool.Remove(path);
        }
    }

    public override void Dispose()
    {
        
    }
}