using FuXi;
using System;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering.Universal;

// ReSharper disable once CheckNamespace
public class GameSceneManager : MonoSingleton<GameSceneManager>
{
    private const string LoadingScenePath = "Assets/Example/BundleResource/Scenes/Loading.unity";
    private BaseScene m_Current;
    private bool m_IsLoading = false;

    public Action<BaseScene> onLoadScene;
    
    public void Init()
    {
        this.m_IsLoading = false;
    }

    private void UpdateCameraStack()
    {
        if (Camera.main == null)
        {
            Debugger.LogError("Main camera not found when switch scene!");
            return;
        }
        var main = Camera.main.GetUniversalAdditionalCameraData();
        main.cameraStack.Add(UIManager.Instance.UICamera);
    }

    /// <summary>
    /// 切换场景
    /// </summary>
    /// <param name="update">请求回调</param>
    /// <typeparam name="T"></typeparam>
    public async UniTask<T> SwitchScene<T>(Func<Action<float, string>, UniTask> update = null) where T : BaseScene
    {
        if (this.m_IsLoading)
        {
            Debugger.ColorWarning(Debugger.ColorStyle.Orange, "重复触发切换场景: {0}", typeof(T));
            return (T) this.m_Current;
        }
        if (this.m_Current != null && this.m_Current.GetType() == typeof(T)) return (T) this.m_Current;
        this.m_IsLoading = true;
        var scene = Activator.CreateInstance<T>();
        var window = await scene.GetLoadingForm();
        window.UpdateSlider(0.0f);
#pragma warning disable 4014
        FxScene.LoadScene(LoadingScenePath);
#pragma warning restore 4014
        await UniTask.NextFrame();
        this.UpdateCameraStack();
        window.UpdateSlider(0.05f);
        if (this.m_Current != null)
            await this.m_Current.Leave();
        window.UpdateSlider(0.1f);
        UIManager.Instance.DestroyExceptLayers(new List<UILayer>{UILayer.Top, UILayer.Tip});
        window.UpdateSlider(0.15f);
        AssetManager.Instance.CleanUP();
        window.UpdateSlider(0.2f);
        GC.Collect();
        await UniTask.NextFrame();
        var attr = typeof(T).GetAttribute<PathAttribute>();
        await FxScene.LoadSceneAsync(attr.PrefabPath, false, f =>
        {
            window.UpdateSlider(0.2f + 0.2f * f);
        });
        this.UpdateCameraStack();
        window.UpdateSlider(0.4f);

        var updateNotNull = update != null;
        if (updateNotNull)
            await update.Invoke((f, desc) => { window.UpdateSlider(0.4f + 0.3f * f, desc); });
        
        await UniTask.NextFrame();
        await scene.PreloadData(f =>
        { window.UpdateSlider(updateNotNull ? 0.7f + 0.2f * f : 0.4f + 0.5f * f); });
        
        window.UpdateSlider(0.9f);
        this.m_IsLoading = false;
        await scene.LoadCompleted();
        this.m_Current = scene;
        window.UpdateSlider(1.0f);
        await UniTask.NextFrame();
        scene.CloseLoadingForm();
        onLoadScene?.Invoke(scene);
        return scene;
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public async UniTask<T> LoadScene<T>() where T : BaseScene
    {
        if (this.m_IsLoading)
        {
            Debugger.ColorWarning(Debugger.ColorStyle.Orange, "正在加载场景: {0}", typeof(T));
            return (T) this.m_Current;
        }
        this.m_IsLoading = true;
        var scene = Activator.CreateInstance<T>();
        var attr = typeof(T).GetAttribute<PathAttribute>();
        await FxScene.LoadSceneAsync(attr.PrefabPath);
        this.UpdateCameraStack();
        await scene.PreloadData();
        this.m_IsLoading = false;
        this.m_Current = scene;
        await scene.LoadCompleted();
        onLoadScene?.Invoke(scene);
        return scene;
    }
}