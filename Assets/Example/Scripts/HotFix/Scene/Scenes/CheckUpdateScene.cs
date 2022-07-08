using System;
using FuXi;

namespace Game.HotFix
{
    public class CheckUpdateScene : SceneHandle
    {
        private readonly string m_CheckUpdateFormPath = "Assets/Example/BundleResource/Prefabs/Builtin/CheckUpdateForm.prefab";
        public override async void Enter()
        {
            var window = await UIManager.Instance.OpenWindow(this.m_CheckUpdateFormPath, UILayer.Normal);
            CheckUpdateForm form = window.GetComponent<CheckUpdateForm>();
            
            await FuXiManager.FxCheckUpdate(f =>
            {
                form.UpdateHandle(0, $"检查更新:{f}");
            });

            var download = await FuXiManager.FxCheckDownloadSize(true);
            if (download.DownloadSize > 0)
            {
                GameDebugger.Log($"检测到版本变更, 大小:{download.FormatSize}");
                await FuXiManager.FxCheckDownload(download, a =>
                {
                    form.UpdateHandle(a.progress,$"正在下载: {a.FormatDownloadSize}");
                });
                GameDebugger.Log("下载完成!");
            }
            UIManager.Instance.CloseWindow(this.m_CheckUpdateFormPath);

            var json = await FxRawAsset.LoadAsync("Assets/Example/BundleResource/RawFile/encryptTest.json");
            GameDebugger.Log($"Raw asset : {json.Text}");
            
            SceneManager.Instance.SwitchScene<MainHomeScene>(SceneConfigs.MainHomeScene);
        }

        public override void Exist()
        {
            
        }
    }
}