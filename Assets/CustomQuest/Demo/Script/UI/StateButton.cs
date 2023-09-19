using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Custom;

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
            MineSweeper.DetectedTypeChanged += this.DetectedTypeChanged;
        }

        private void Start()
        {
            this.Sprite = this._CheckType == EMineMap.Flag ? IMine.ImageDetail.Flag : IMine.ImageDetail.Mine;

            this._TypeImage.sprite = this.Sprite;
        }

        private void OnDestroy()
        {
            MineSweeper.DetectedTypeChanged -= this.DetectedTypeChanged;
        }

        public void DetectedTypeChanged(EMineMap map) 
        {
            var color = this._CheckType != map ? IMine.ImageDetail.Normal : IMine.ImageDetail.PointerDown;

            IMine.ImageDetail.SetImage(this._BackgroundImage, color);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IMine.ImageDetail.SetImage(this._BackgroundImage, IMine.ImageDetail.PointerDown);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (MineSweeper.DetectedType == this._CheckType) { return; }

            IMine.ImageDetail.SetImage(this._BackgroundImage, IMine.ImageDetail.Normal);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            MineSweeper.DetectedType = this._CheckType;
        }
    }
}