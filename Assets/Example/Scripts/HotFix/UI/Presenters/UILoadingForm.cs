using UnityEngine;

[ViewInfo(typeof(UILoadingFormView), UILayer.Top)]
public class UILoadingForm : BaseLoading
{
    private UILoadingFormView View => (UILoadingFormView) this.view;

    public override void UpdateSlider(float progress, string desc = null)
    {
        if (this.View.target == null)
            return;
        this.View.ImgHandle.fillAmount = Mathf.Clamp(progress, 0.0f, 1f);
        this.View.TextDesc.SetText(desc);
    }
}