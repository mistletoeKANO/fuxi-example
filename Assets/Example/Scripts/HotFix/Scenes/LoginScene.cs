
using System;
using Cysharp.Threading.Tasks;

[Path("Scenes/Login.unity")]
public class LoginScene : BaseScene
{
    public override UniTask LoadCompleted()
    {
        GameSceneManager.Instance.SwitchScene<HomeScene>(this.AsyncOperation);
        return UniTask.CompletedTask;
    }

    private async UniTask AsyncOperation(Action<float, string> action)
    {
        await UniTask.DelayFrame(10);
        action?.Invoke(0.1f,"");
        await UniTask.DelayFrame(10);
        action?.Invoke(0.2f,"");
        await UniTask.DelayFrame(10);
        action?.Invoke(0.3f,"");
        await UniTask.DelayFrame(10);
        action?.Invoke(0.4f,"");
        await UniTask.DelayFrame(10);
        action?.Invoke(0.5f,"");
        await UniTask.DelayFrame(10);
        action?.Invoke(0.6f,"");
        await UniTask.DelayFrame(10);
        action?.Invoke(0.7f,"");
        await UniTask.DelayFrame(10);
        action?.Invoke(0.8f,"");
        await UniTask.DelayFrame(10);
        action?.Invoke(0.9f,"");
        await UniTask.DelayFrame(10);
    }
}