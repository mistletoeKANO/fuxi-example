
using Cysharp.Threading.Tasks;

[Path("Scenes/Home.unity")]
public class HomeScene : BaseSceneWithLoading
{
    public override async UniTask LoadCompleted()
    {
        var raw = await FuXi.FxRawAsset.LoadAsync("RawFile/encryptTest.json");
        Debugger.Log($"RAW FILE: {raw.Text}");
    }
}