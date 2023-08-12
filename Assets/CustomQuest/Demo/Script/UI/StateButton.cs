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
                if (this._CheckType != type)
                {
                    this.SetImage(this.Sprite, ImageDetail.Normal);
                }

                else
                {
                    this.SetImage(this.Sprite, ImageDetail.PointerDown);
                }
            };
        }

        private void Start()
        {
            this.Sprite = this._CheckType == EMineMap.Flag ? QuestDemo.ImageDetail.Flag : QuestDemo.ImageDetail.Mine;
        }

        public void OnPointerDown(PointerEventData eventData) 
        {
            this.SetImage(this.Sprite, ImageDetail.PointerDown);
        }

        public void OnPointerClick(PointerEventData eventData) 
        {
            QuestDemo.CheckType = this._CheckType;
        }

        private void SetImage(Sprite sprite, Color color) 
        {
            this._TypeImage.sprite = sprite;
            this._BackgroundImage.color = color;
        }
    }
}