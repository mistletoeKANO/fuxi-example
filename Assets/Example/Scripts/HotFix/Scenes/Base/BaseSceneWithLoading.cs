using Cysharp.Threading.Tasks;

public abstract class BaseSceneWithLoading : BaseScene
{
    public override async UniTask<BaseLoading> GetLoadingForm()
    {
        return await UIManager.Instance.OpenWindow<UILoadingForm>(false);
    }

    public override void CloseLoadingForm()
    {
        UIManager.Instance.CloseWindow<UILoadingForm>();
    }
}