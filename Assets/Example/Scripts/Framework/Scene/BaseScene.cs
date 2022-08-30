using System;
using Cysharp.Threading.Tasks;

public abstract class BaseScene
{
    public virtual UniTask PreloadData(Action<float> action = null) { return UniTask.CompletedTask;}
    public virtual UniTask LoadCompleted() { return UniTask.CompletedTask;}
    public virtual UniTask Leave(){return UniTask.CompletedTask;}

    public virtual UniTask<BaseLoading> GetLoadingForm()
    {
        return default;
    }

    public virtual void CloseLoadingForm(){}
}