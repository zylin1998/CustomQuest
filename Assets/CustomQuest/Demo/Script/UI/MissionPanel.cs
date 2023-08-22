using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Custom.Quest;

namespace QuestDemo
{
    public class MissionPanel : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect _ScrollRect;
        [SerializeField]
        private MineMissionList _MissionList;
        [SerializeField]
        private Animator _Animator;
        [SerializeField]
        private List<MissionButton> _Buttons;

        private void Awake()
        {
            this._Buttons = this._ScrollRect.content.GetComponentsInChildren<MissionButton>().ToList();

            this._MissionList.Initialize();
        }

        private void Start()
        {
            MissionSwitch.ClickEvent += this.PanelState;
            MissionButton.ClickEvent += this.EndMission;

            this.SetMission();
        }

        public void PanelState(bool state) 
        {
            var name = state ? "Open" : "Close";

            if (state) { this._ScrollRect.verticalNormalizedPosition = 1; }

            this._Animator.Play(name);
        }

        public void CheckMission(MineMissionArgs args) 
        {
            this._Buttons.ForEach(f => f.SetMission(args));
        }

        public void EndMission(MineMission mission) 
        {
            if (mission.Progress == IMission.EProgress.Complete) 
            {
                mission.End();

                this.SetMission();
            }
        }

        public void SetMission() 
        {
            var list = this._MissionList.ToList();

            var c = 0;
            this._Buttons.ForEach(f =>
            {
                f.SetMission(list[c]);

                c++;
            });
        }
    }
}