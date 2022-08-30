using UnityEngine;
using UnityEngine.UI;

public abstract class BaseView
{
    public GameObject target;
    public RectTransform rectTransform;
    public Canvas canvas;
    internal UIFormOperation formOperation;
    internal GraphicRaycaster graphicRaycaster;

    public void Init(GameObject hold)
    {
        target = hold;
        canvas = target.GetComponent<Canvas>();
        if (null == canvas)
            Debugger.ColorError(Color.red, "UIForm: {0} does not contain canvas", target.name);
        else
            canvas.overrideSorting = true;
        rectTransform = target.GetComponent<RectTransform>();
        if (null == rectTransform)
            Debugger.ColorError(Color.red, "UIForm: {0} does not contain rectTransform", target.name);
        formOperation = target.GetComponent<UIFormOperation>();
        graphicRaycaster = target.GetComponent<GraphicRaycaster>();
    }
    
    /// <summary>
    /// 从预制体上查找所有需要用到的组件并存下来。
    /// </summary>
    public abstract void FindAndSaveComponents();
}