using System.Threading.Tasks;
using FuXi;
using UnityEngine;

public class CheckUpdater
{
    private const string UIFormPath = "Builtin/UICheckUpdateForm.prefab";
    private UICheckUpdateForm m_CheckForm;
    private FxAsset formAsset;

    public async Task Start()
    {
        if (FuXiManager.RuntimeMode != RuntimeMode.Runtime) return;
        this.formAsset = await FxAsset.LoadAsync<GameObject>(UIFormPath);
        if (this.formAsset.asset == null)
        {
            Debugger.LogError("启动更新页面加载失败!");
            return;
        }
        var form = (GameObject) UnityEngine.Object.Instantiate(this.formAsset.asset);
        this.m_CheckForm = form.GetComponent<UICheckUpdateForm>();
        await this.CheckUpdate();
    }

    private async Task CheckUpdate()
    {
        this.m_CheckForm.UpdateDesc("检查版本.");
        await FuXiManager.FxCheckUpdate(f =>
        {
            this.m_CheckForm.UpdateProgress(f);
        });
        this.m_CheckForm.UpdateDesc("检查下载内容.");
        var downloadInfo = await FuXiManager.FxCheckDownloadSize(true, f =>
        {
            this.m_CheckForm.UpdateProgress(f);
        });
        if (downloadInfo.DownloadSize > 0)
        {
            this.m_CheckForm.UpdateDesc("正在下载更新内容.");
            await FuXiManager.FxCheckDownload(downloadInfo, f =>
            {
                this.m_CheckForm.UpdateProgress(f.progress);
            });
        }
        this.formAsset.Release();
    }
}