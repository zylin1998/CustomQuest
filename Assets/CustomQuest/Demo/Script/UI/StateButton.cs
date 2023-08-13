using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace QuestDemo
{
    public class StateButton : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
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
            QuestDemo.TypeChangedEvent += (type) =>
            {
                var color = this._CheckType != type ? ImageDetail.Normal : ImageDetail.PointerDown;

                this.SetColor(color);
            };
        }

        private void Start()
        {
            this.Sprite = this._CheckType == EMineMap.Flag ? QuestDemo.ImageDetail.Flag : QuestDemo.ImageDetail.Mine;

            this._TypeImage.sprite = this.Sprite;
        }

        public void OnPointerDown(PointerEventData eventData) => this.SetColor(ImageDetail.PointerDown);
        public void OnPointerClick(PointerEventData eventData)  => QuestDemo.CheckType = this._CheckType;

        private void SetColor(Color color) => this._BackgroundImage.color = color;
    }
}