using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace QuestDemo
{
    public class MissionSwitch : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private bool _Switch = false;
        
        public void OnPointerClick(PointerEventData eventData) 
        {
            this._Switch = !this._Switch;

            OnClick.Invoke(this._Switch);
        }

        private static Action<bool> OnClick;

        public static event Action<bool> ClickEvent 
        {
            add => OnClick += value;
            
            remove => OnClick -= value;
        }
    }
}