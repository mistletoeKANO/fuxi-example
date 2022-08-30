using UnityEngine;
using UnityEngine.UI;

public class UICheckUpdateForm : MonoBehaviour
{
    public Image Handle;
    public TMPro.TextMeshProUGUI Desc;

    public void UpdateProgress(float progress)
    {
        if (this.Handle == null) return;
        this.Handle.fillAmount = Mathf.Clamp01(progress);
    }

    public void UpdateDesc(string desc)
    {
        if (this.Desc == null) return;
        this.Desc.SetText(desc);
    }
}