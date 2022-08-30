
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[AddComponentMenu("UI/UIFormOperation")]
public class UIFormOperation : MonoBehaviour
{
    [SerializeField]
    private bool m_OccludeScene;
    
    [SerializeField]
    private bool m_OccludeUi;

    [SerializeField]
    private bool m_UseSystemEsc;

    [SerializeField]
    private bool m_UseBgClose;
    
    [SerializeField]
    private Button m_CloseBtn;

    public bool IsOccludeScene
    {
        get { return this.m_OccludeScene; }
    }
    
    public bool IsOccludeUi
    {
        get { return this.m_OccludeUi; }
    }
    
    public bool IsUseSystemEsc
    {
        get { return this.m_UseSystemEsc; }
    }
    
    public bool IsUseBgClose
    {
        get { return this.m_UseBgClose; }
    }
    
    public Button CloseBtn
    {
        get { return this.m_CloseBtn; }
    }
}