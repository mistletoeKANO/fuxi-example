/// <Summary>
/// 当前脚本由工具自动生成，请勿手动修改!!!
/// </summary>
[Path("Builtin/UILoadingForm.prefab")]
public sealed class UILoadingFormView : BaseView
{
    private UnityEngine.UI.Image imgHandle;
    public UnityEngine.UI.Image ImgHandle => imgHandle;
    private TMPro.TextMeshProUGUI textDesc;
    public TMPro.TextMeshProUGUI TextDesc => textDesc;

    
    public override void FindAndSaveComponents()
    {
        if (target == null) return;
		var f_3 = target.transform.Find("SliderBG/Handle");
		if (null != f_3) 
		{
			imgHandle = f_3.GetComponent<UnityEngine.UI.Image>();
		}
		var f_4 = target.transform.Find("Desc");
		if (null != f_4) 
		{
			textDesc = f_4.GetComponent<TMPro.TextMeshProUGUI>();
		}

    }
}