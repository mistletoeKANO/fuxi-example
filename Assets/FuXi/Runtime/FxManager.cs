using System;
using UnityEngine;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace FuXi
{
    /// <summary>
    /// 伏羲 资源管理器
    /// </summary>
    public static class FxManager
    {
        internal static readonly FxManifestDriver ManifestVC = new FxManifestDriver("FuXiAssetWindow");
        internal static System.Func<FxManifest> ParseManifestCallback;  // 用在Unity编辑器下初始化本地配置
        internal static RuntimeMode RuntimeMode = RuntimeMode.Editor;
        internal static string PlatformURL => "http://192.168.31.111/Windows/";
        
        // FuXi启动器
        public static async Task FxLauncherAsync(RuntimeMode runtimeMode = RuntimeMode.Editor)
        {
            FxManager.RuntimeMode = runtimeMode;
            if (FxManager.RuntimeMode == RuntimeMode.Editor && !Application.isEditor)
            {
                FxManager.RuntimeMode = RuntimeMode.Offline;
            }
            var root = GameObject.Find("__FuXi Asset Manager__");
            if (root == null)
            {
                root = new GameObject("__FuXi Asset Manager__");
                root.AddComponent<AssetPolling>();
            }
            UnityEngine.Object.DontDestroyOnLoad(root);
            if (FxManager.RuntimeMode == RuntimeMode.Editor)
                FxManager.ManifestVC.NewManifest = ParseManifestCallback?.Invoke();
            else
            {
                FxAsset.FxAssetCreate = FxAsset.CreateAsset;
                FxScene.FxSceneCreate = FxScene.CreateScene;
                FxRawAsset.FxRawAssetCreate = FxRawAsset.CreateRawAsset;
                await new CheckLocalManifest().Execute();
            }
            FxManager.ManifestVC.InitEncrypt();
            FxDebug.Log("Init FuXi finished!");
        }
        
        #region Task 版本
        
        /// <summary>
        /// 检查服务器版本文件
        /// </summary>
        /// <param name="checkUpdate"></param>
        /// <returns></returns>
        public static async Task FxCheckUpdate(Action<float> checkUpdate = null)
        {
            if (FxManager.RuntimeMode != RuntimeMode.Runtime) return;
            await new CheckWWWManifest(checkUpdate).Execute();
            FxDebug.Log("Check update finished!");
        }

        /// <summary>
        /// 获取更新大小，分包更新
        /// </summary>
        /// <param name="packages">分包列表</param>
        /// <param name="checkDownload"></param>
        /// <returns></returns>
        public static async Task<DownloadInfo> FxCheckDownloadSize(string[] packages, Action<float> checkDownload = null)
        {
            if (FxManager.RuntimeMode != RuntimeMode.Runtime) return default;
            CheckDownloadSize c = (CheckDownloadSize) await new CheckDownloadSize(packages, checkDownload).Execute();
            return c.DownloadInfo;
        }

        /// <summary>
        /// 获取更新大小，默认全局更新
        /// </summary>
        /// <param name="containsPackage">是否包含分包</param>
        /// <param name="checkDownload"></param>
        /// <returns></returns>
        public static async Task<DownloadInfo> FxCheckDownloadSize(bool containsPackage = false, Action<float> checkDownload = null)
        {
            if (FxManager.RuntimeMode != RuntimeMode.Runtime) return default;
            CheckDownloadSize c = (CheckDownloadSize) await new CheckDownloadSize(containsPackage, checkDownload).Execute();
            return c.DownloadInfo;
        }

        /// <summary>
        /// 检查下载
        /// </summary>
        /// <param name="downloadInfo"></param>
        /// <param name="checkDownload"></param>
        /// <returns></returns>
        public static async Task FxCheckDownload(DownloadInfo downloadInfo, Action<float> checkDownload = null)
        {
            if (FxManager.RuntimeMode != RuntimeMode.Runtime) return;
            await new CheckDownloadBundle(downloadInfo, checkDownload).Execute();
        }

        #endregion

        #region Coroutine 协程版本

        /// <summary>
        /// 检查服务器版本文件 协程版本
        /// </summary>
        /// <param name="checkUpdate"></param>
        /// <returns></returns>
        public static CheckWWWManifest FxCheckUpdateCo(Action<float> checkUpdate = null)
        {
            if (FxManager.RuntimeMode != RuntimeMode.Runtime) return null;
            var check = new CheckWWWManifest(checkUpdate);
            check.Execute();
            return check;
        }
        
        /// <summary>
        /// 获取更新大小，分包更新 协程版本
        /// </summary>
        /// <param name="packages"></param>
        /// <param name="checkDownload"></param>
        /// <returns></returns>
        public static CheckDownloadSize FxCheckDownloadSizeCo(string[] packages, Action<float> checkDownload = null)
        {
            if (FxManager.RuntimeMode != RuntimeMode.Runtime) return null;
            var check = new CheckDownloadSize(packages, checkDownload);
            check.Execute();
            return check;
        }
        
        /// <summary>
        /// 获取更新大小，默认全局更新 协程版本
        /// </summary>
        /// <param name="containsPackage">是否包含分包</param>
        /// <param name="checkDownload"></param>
        /// <returns></returns>
        public static CheckDownloadSize FxCheckDownloadSizeCo(bool containsPackage = false, Action<float> checkDownload = null)
        {
            if (FxManager.RuntimeMode != RuntimeMode.Runtime) return null;
            var check = new CheckDownloadSize(containsPackage, checkDownload);
            check.Execute();
            return check;
        }
        
        /// <summary>
        /// 检查下载 协程版本
        /// </summary>
        /// <param name="downloadInfo"></param>
        /// <param name="checkDownload"></param>
        /// <returns></returns>
        public static CheckDownloadBundle FxCheckDownloadCo(DownloadInfo downloadInfo, Action<float> checkDownload = null)
        {
            if (FxManager.RuntimeMode != RuntimeMode.Runtime) return null;
            var check = new CheckDownloadBundle(downloadInfo, checkDownload);
            check.Execute();
            return check;
        }

        #endregion
    }
}