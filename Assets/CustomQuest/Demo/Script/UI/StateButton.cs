using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace QuestDemo
{
    public class StateButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField]
        private EMineMap _CheckType;
        [SerializeField]
        private Image _TypeImage;
        [SerializeField]
        private Image _BackgroundImage;

        public Sprite Sprite { get; private set; }

        private void Awake()
        {
            IMine.TypeChangedEvent += this.OnTypeChanged;
        }

        private void Start()
        {
            this.Sprite = this._CheckType == EMineMap.Flag ? QuestDemoUI.ImageDetail.Flag : QuestDemoUI.ImageDetail.Mine;

            this._TypeImage.sprite = this.Sprite;
        }

        private void OnDestroy()
        {
            IMine.TypeChangedEvent -= this.OnTypeChanged;
        }

        public void OnTypeChanged(EMineMap map) 
        {
            var color = this._CheckType != map ? ImageDetail.Normal : ImageDetail.PointerDown;

            ImageDetail.SetImage(this._BackgroundImage, color);
        }

        public void OnPointerDown(PointerEventData eventData) => ImageDetail.SetImage(this._BackgroundImage, ImageDetail.PointerDown);
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (IMine.DetectedType == this._CheckType) { return; }

            ImageDetail.SetImage(this._BackgroundImage, ImageDetail.Normal);
        }

        public void OnPointerClick(PointerEventData eventData)  => IMine.DetectedType = this._CheckType;
    }
}