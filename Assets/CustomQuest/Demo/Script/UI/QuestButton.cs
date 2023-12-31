using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace QuestDemo
{
    public class QuestButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private string _Name;
        [SerializeField]
        private Image _Background;
        [SerializeField]
        private bool _Interactable = true;

        public string Name => this._Name;
        public bool Interactable 
        { 
            get => this._Interactable;

            set 
            { 
                this._Interactable = value;

                IMine.ImageDetail.SetImage(this._Background, value ? IMine.ImageDetail.Normal : IMine.ImageDetail.PointerDown);
            }
        }

        private Action OnClick = () => { };

        public event Action ClickEvent 
        {
            add => this.OnClick += value;
            
            remove => this.OnClick -= value;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!Interactable) { return; }

            IMine.ImageDetail.SetImage(this._Background, IMine.ImageDetail.PointerDown);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!Interactable) { return; }

            IMine.ImageDetail.SetImage(this._Background, IMine.ImageDetail.Normal);
        }

        public void OnPointerClick(PointerEventData eventData) 
        {
            if (!Interactable) { return; }

            this.OnClick?.Invoke();
        }
    }
}