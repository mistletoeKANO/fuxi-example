
using UnityEngine;
using UnityEngine.UI;

public class CircleTouchMove : MonoBehaviour
{
    public GameObject Cube;
    public Image Image;
    public TouchMove TouchMove;
    public Camera MainCamera;
    public Camera UICamera;
    
    private void Start()
    {
        UICamera.clearFlags = CameraClearFlags.Depth;
        UICamera.Render();
        this.UpdatePos();
    }
    

    private void LateUpdate()
    {
        if (this.TouchMove.ClickPos == Vector3.zero) return;
        this.UpdateInput();
        this.UpdatePos();
    }

    private void UpdateInput()
    {
        var clickPos2World = MainCamera.ScreenToWorldPoint(this.TouchMove.ClickPos);
        var dis = Vector3.Distance(clickPos2World, this.TouchMove.ClickPos);
        if(dis < 0.01f) return;

        var cubePos = this.Cube.transform.position;
        var nor = (clickPos2World - cubePos).normalized;
        cubePos += nor * Time.deltaTime * 5;
        this.Cube.transform.position = cubePos;
    }

    private void UpdatePos()
    {
        Vector2 sourceScreenPos = MainCamera.WorldToScreenPoint(Cube.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(Image.transform.parent.GetComponent<RectTransform>(),
            sourceScreenPos, UICamera, out var anchoredPos);
        Image.rectTransform.anchoredPosition = anchoredPos;
    }
}