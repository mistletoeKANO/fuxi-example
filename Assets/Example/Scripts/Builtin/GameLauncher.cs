using System;
using System.Collections;
using FuXi;
using Game.HotFix;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Builtin
{
    public class GameLauncher : MonoBehaviour
    {
        public RuntimeMode RuntimeMode;

        private IEnumerator Start()
        {
            yield return FxManager.FxLauncherAsyncCo("FuXiAssetWindow", "http://192.168.1.2/Windows/", RuntimeMode);

            yield return FxManager.FxCheckUpdateCo();

            var checkSize = FxManager.FxCheckDownloadSizeCo();
            yield return checkSize;

            if (checkSize.DownloadInfo.DownloadSize > 0)
            {
                GameDebugger.Log($"下载大小{checkSize.DownloadInfo.FormatSize}");
                yield return FxManager.FxCheckDownloadCo(checkSize.DownloadInfo);
                GameDebugger.Log("下载完成");
            }else GameDebugger.Log("未检测到更新内容!");

            // var uiRoot = FxAsset.LoadAssetCo("Assets/Example/BundleResource/Prefabs/Game/UIRoot.prefab", typeof(GameObject));
            // yield return uiRoot;
            // UnityEngine.Object.Instantiate(uiRoot.asset);
            
            FxAsset.LoadAssetAsync("Assets/Example/BundleResource/Prefabs/Game/UIRoot.prefab", typeof(GameObject), f =>
            {
                UnityEngine.Object.Instantiate(f.asset);
            });
            
        }

        // private async void Start()
        // {
        //     await FxManager.FxLauncherAsync(RuntimeMode);
        //     
        //     SceneManager.Instance.Init();
        //     UIManager.Instance.Init();
        //     
        //     SceneManager.Instance.SwitchScene<CheckUpdateScene>(SceneConfigs.CheckUpdateScene);
        // }
    }
}
