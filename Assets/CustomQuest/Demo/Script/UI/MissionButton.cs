using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Custom;
using Custom.Quest;

namespace QuestDemo
{
    public class MissionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IReciever
    {
        [SerializeField]
        private Image _Icon;
        [SerializeField]
        private Image _Background;
        [SerializeField]
        private TextMeshProUGUI _DescribeText;

        public IMissionInfo.Info Mission { get; private set; }
        
        public void SetInfo(object info) 
        {
            if (info is IMissionInfo.Info mission) { this.SetMission(mission); }
        }

        public void SetMission(IMissionInfo.Info mission) 
        {
            if (!this.Mission.IsEqual(mission))
            {
                this.Mission = mission;
            }
            
            SetContent(this);
        }

        private Color _LastColor;

        public void OnPointerDown(PointerEventData eventData) 
        {
            this._LastColor = this._Background.color;

            this._Background.color = MissionImageDetail.ColorPointerDown;
        }

        public void OnPointerUp(PointerEventData eventData) 
        {
            this._Background.color = this._LastColor;
        }

        public void OnPointerClick(PointerEventData eventData) 
        {
            CheckMissionProgress(this);
        }

        private static Action OnClick;
        
        public static event Action ClickEvent
        {
            add => OnClick += value;

            remove => OnClick -= value;
        }

        public static void CheckMissionProgress(MissionButton button) 
        {
            var progress = button.Mission.Progress;
            
            if (progress == IMissionInfo.EProgress.Complete) 
            {
                button.Mission.Progress = IMissionInfo.EProgress.End;

                OnClick?.Invoke();
            }
        }

        public static void SetContent(MissionButton button) 
        {
            var mission = button.Mission;
            
            if (mission.Progress == IMissionInfo.EProgress.UnComplete) 
            {
                button._Icon.sprite = MissionImageDetail.SpriteProgress;

                button._Background.color = MissionImageDetail.ColorNormal;
            }

            if (mission.Progress == IMissionInfo.EProgress.Complete) 
            {
                button._Icon.sprite = MissionImageDetail.SpriteComplete;
                
                button._Background.color = MissionImageDetail.ColorComplete;
            }

            if (mission.Progress == IMissionInfo.EProgress.End)
            {
                button._Icon.sprite = MissionImageDetail.SpriteComplete;

                button._Background.color = MissionImageDetail.ColorEnd;
            }
            
            button._DescribeText.SetText(mission.Describe);
        }
    }

    [Serializable]
    public struct MissionImageDetail
    {
        [SerializeField]
        private Sprite _Progress;
        [SerializeField]
        private Sprite _Complete;
        [SerializeField]
        private ColorBlock _ColorBlock;

        public static MissionImageDetail Detail { get; set; }

        public static Sprite SpriteProgress => Detail._Progress;
        public static Sprite SpriteComplete => Detail._Complete;

        public static Color ColorNormal => Detail._ColorBlock.Normal;
        public static Color ColorPointerDown => Detail._ColorBlock.PointerDown;
        public static Color ColorComplete => Detail._ColorBlock.Complete;
        public static Color ColorEnd => Detail._ColorBlock.End;

        [System.Serializable]
        public struct ColorBlock
        {
            [SerializeField]
            private Color _Normal;
            [SerializeField]
            private Color _PointerDown;
            [SerializeField]
            private Color _Complete;
            [SerializeField]
            private Color _End;

            public Color Normal => this._Normal;
            public Color PointerDown => this._PointerDown;
            public Color Complete => this._Complete;
            public Color End => this._End;
        }
    }
}