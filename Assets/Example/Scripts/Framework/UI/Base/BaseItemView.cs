using UnityEngine;

public abstract class BaseItemView
{
    protected BaseItemView(GameObject transform) { this.target = transform;}
    public GameObject target;
}