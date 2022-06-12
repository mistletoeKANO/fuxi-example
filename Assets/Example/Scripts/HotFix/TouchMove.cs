using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchMove : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector3 ClickPos { get; set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.ClickPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.ClickPos = Vector2.zero;
    }
}