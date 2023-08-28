using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Custom.Quest;
using TMPro;

namespace QuestDemo
{
    public class MissionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField]
        private Image _Icon;
        [SerializeField]
        private Image _Background;
        [SerializeField]
        private TextMeshProUGUI _DescribeText;

        public IMission Mission { get; private set; }
        
        public void SetMission(IMission mission) 
        {
            if (this.Mission != mission)
            {
                this.Mission = mission;
            }
            
            SetContent(this);
        }

        private Color _LastColor;

        public void OnPointerDown(PointerEventData eventData) 
        {
            if (this.Mission.IsClear) { return; }

            this._LastColor = this._Background.color;

            this._Background.color = MissionImageDetail.ColorPointerDown;
        }

        public void OnPointerUp(PointerEventData eventData) 
        {
            if (this.Mission.IsClear) { return; }

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
            
            if (progress == IMission.EProgress.Complete) 
            {
                button.Mission.End();

                OnClick?.Invoke();
            }
        }

        public static void SetContent(MissionButton button) 
        {
            var mission = button.Mission;

            if (mission.Progress == IMission.EProgress.UnComplete) 
            {
                button._Icon.sprite = MissionImageDetail.SpriteProgress;

                button._Background.color = MissionImageDetail.ColorNormal;
            }

            if (mission.Progress == IMission.EProgress.Complete) 
            {
                button._Icon.sprite = MissionImageDetail.SpriteComplete;
                
                button._Background.color = MissionImageDetail.ColorComplete;
            }

            if (mission.Progress == IMission.EProgress.End)
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