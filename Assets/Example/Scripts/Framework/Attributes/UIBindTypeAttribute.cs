
using System;

public class ViewInfo : Attribute
{
    public Type viewType;

    public UILayer layer;
    
    public ViewInfo(Type viewType, UILayer layer = UILayer.Normal)
    {
        this.viewType = viewType;
        this.layer = layer;
    }
}