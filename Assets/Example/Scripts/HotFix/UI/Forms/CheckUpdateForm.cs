using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.HotFix
{
    public class CheckUpdateForm : MonoSingleton
    {
        public TextMeshProUGUI Desc;
        public Image Handle;

        public void UpdateHandle(float value, string desc)
        {
            this.Handle.fillAmount = Mathf.Clamp(value, 0f, 1f);
            this.Desc.SetText(desc);
        }
    }
}