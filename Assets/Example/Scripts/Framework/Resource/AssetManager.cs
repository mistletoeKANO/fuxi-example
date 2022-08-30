using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FuXi;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class AssetManager : Singleton<AssetManager>
{
    private const string RootPath = "Assets/Example/BundleResource/";
    private readonly Dictionary<string, FxAsset> m_Cache = new Dictionary<string, FxAsset>();
    private readonly List<string> m_KeepOnSwitchScene = new List<string>();

    public string GetAssetPath(string path)
    {
        return $"{RootPath}{path}";
    }

    /// <summary>
    /// 异步加载
    /// </summary>
    /// <param name="path"></param>
    /// <param name="destroyOnSwitchScene"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async UniTask<T> LoadAssetAsync<T>(string path, bool destroyOnSwitchScene = true)
        where T : UnityEngine.Object
    {
        path = $"{RootPath}{path}";
        if (!this.m_Cache.TryGetValue(path, out var fxAsset))
        {
            fxAsset = await FxAsset.LoadAsync<T>(path);
            this.m_Cache.Add(path, fxAsset);
        }

        if (!destroyOnSwitchScene && !this.m_KeepOnSwitchScene.Contains(path))
            this.m_KeepOnSwitchScene.Add(path);
        return (T) fxAsset.asset;
    }
    
    /// <summary>
    /// 同步加载, 尽量少用
    /// </summary>
    /// <param name="path"></param>
    /// <param name="destroyOnSwitchScene"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T LoadAsset<T>(string path, bool destroyOnSwitchScene = true) where T : UnityEngine.Object
    {
        path = $"{RootPath}{path}";
        if (!this.m_Cache.TryGetValue(path, out var fxAsset))
        {
            fxAsset = FxAsset.Load<T>(path);
            this.m_Cache.Add(path, fxAsset);
        }

        if (!destroyOnSwitchScene && !this.m_KeepOnSwitchScene.Contains(path))
            this.m_KeepOnSwitchScene.Add(path);
        return (T) fxAsset.asset;
    }

    /// <summary>
    /// 异步加载实例化
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parent"></param>
    /// <param name="destroyOnSwitchScene"></param>
    /// <returns></returns>
    public async UniTask<UnityEngine.GameObject> InstantiateAsync(string path, UnityEngine.Transform parent,
        bool destroyOnSwitchScene = true)
    {
        var asset = await this.LoadAssetAsync<UnityEngine.GameObject>(path, destroyOnSwitchScene);
        if (asset == null) return default;
        return UnityEngine.Object.Instantiate(asset, parent);
    }

    /// <summary>
    /// 同步加载并实例化
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parent"></param>
    /// <param name="destroyOnSwitchScene"></param>
    /// <returns></returns>
    public UnityEngine.GameObject Instantiate(string path, UnityEngine.Transform parent,
        bool destroyOnSwitchScene = true)
    {
        var asset = this.LoadAsset<UnityEngine.GameObject>(path, destroyOnSwitchScene);
        if (asset == null) return default;
        return UnityEngine.Object.Instantiate(asset, parent);
    }

    /// <summary>
    /// 加载精灵图
    /// </summary>
    /// <param name="path"></param>
    /// <param name="destroyOnSwitchScene"></param>
    /// <returns></returns>
    public async UniTask<UnityEngine.Sprite> LoadSpriteAsync(string path, bool destroyOnSwitchScene = true)
    {
        return await this.LoadAssetAsync<UnityEngine.Sprite>(path, destroyOnSwitchScene);
    }

    /// <summary>
    /// 加载精灵图
    /// </summary>
    /// <param name="path"></param>
    /// <param name="destroyOnSwitchScene"></param>
    /// <returns></returns>
    public Sprite LoadSprite(string path, bool destroyOnSwitchScene = true)
    {
        return LoadAsset<Sprite>(path, destroyOnSwitchScene);
    }

    /// <summary>
    /// 加载预制体
    /// </summary>
    /// <param name="path"></param>
    /// <param name="destroyOnSwitchScene"></param>
    /// <returns></returns>
    public async UniTask<UnityEngine.GameObject> LoadPrefab(string path, bool destroyOnSwitchScene = true)
    {
        return await this.LoadAssetAsync<UnityEngine.GameObject>(path, destroyOnSwitchScene);
    }

    /// <summary>
    /// 清理 已加载 资源
    /// </summary>
    /// <param name="forceClean">强制清理所有资源</param>
    public void CleanUP(bool forceClean = false)
    {
        if (forceClean)
        {
            foreach (var asset in m_Cache)
                asset.Value.Release();
            this.m_Cache.Clear();
            this.m_KeepOnSwitchScene.Clear();
            return;
        }

        List<string> removed = new List<string>();
        foreach (var asset in m_Cache)
        {
            if (m_KeepOnSwitchScene.Contains(asset.Key)) continue;
            asset.Value.Release();
            removed.Add(asset.Key);
        }

        foreach (var r in removed)
            this.m_Cache.Remove(r);
    }
}