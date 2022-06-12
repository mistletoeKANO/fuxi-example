using System;
using FuXi;

namespace Game.HotFix
{
    public class CheckUpdateScene : SceneHandle
    {
        private readonly string m_CheckUpdateFormPath = "Assets/Example/BundleResource/Prefabs/UI/Form/CheckUpdateForm.prefab";
        public override async void Enter()
        {
            var window = await UIManager.Instance.OpenWindow(this.m_CheckUpdateFormPath, UILayer.Normal);
            CheckUpdateForm form = window.GetComponent<CheckUpdateForm>();
            
            await FxManager.FxCheckUpdate(f =>
            {
                form.UpdateHandle(0, $"检查更新:{f}");
            });

            var download = await FxManager.FxCheckDownloadSize(true);
            if (download.DownloadSize > 0)
            {
                GameDebugger.Log($"检测到版本变更, 大小:{download.FormatSize}");
                await FxManager.FxCheckDownload(download, a =>
                {
                    form.UpdateHandle(a,$"正在下载: {a}");
                });
                GameDebugger.Log("下载完成!");
            }
            UIManager.Instance.CloseWindow(this.m_CheckUpdateFormPath);

            var json = await FxRawAsset.LoadRawAssetAsync("Assets/Example/BundleResource/Audios/encryptTest.json");
            GameDebugger.Log($"Raw asset : {json.Text}");
        }

        public override void Exist()
        {
            
        }
    }
}