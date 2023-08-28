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
        public void OnPointerClick(PointerEventData eventData) 
        {
            OnClick.Invoke();
        }

        private static Action OnClick;

        public static event Action ClickEvent 
        {
            add => OnClick += value;
            
            remove => OnClick -= value;
        }
    }
}