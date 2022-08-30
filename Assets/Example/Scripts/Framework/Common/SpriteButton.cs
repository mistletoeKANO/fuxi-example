using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

// ReSharper disable once CheckNamespace
namespace UnityEngine
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SpriteButton : MonoBehaviour, IPointerClickHandler
    {
        [Serializable]
        public class ButtonClickedEvent : UnityEvent {}

        // Event delegates triggered on click.
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();
        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }
        
        public void SetOnClick(UnityAction ac)
        {
            this.onClick.AddListener(ac);
        }
        
        public void RSetOnClick(UnityAction ac)
        {
            this.onClick.RemoveAllListeners();
            this.onClick.AddListener(ac);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            this.onClick?.Invoke();
        }

        public void RemoveOnClick()
        {
            this.onClick.RemoveAllListeners();
        }
    }
}
