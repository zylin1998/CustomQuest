using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuestDemo
{
    public class MissionPanel : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect _ScrollRect;
        [SerializeField]
        private Animator _Animator;
        [SerializeField]
        private List<MissionButton> _Buttons;

        private bool _State = false;

        private void Awake()
        {
            this._Buttons = this._ScrollRect.content.GetComponentsInChildren<MissionButton>().ToList();
        }

        private void Start()
        {
            MissionSwitch.ClickEvent += this.PanelState;
            MissionButton.ClickEvent += this.SetMission;

            this.SetMission();
        }

        public void PanelState() 
        {
            this._State = !this._State;

            var name = this._State ? "Open" : "Close";

            if (this._State) 
            { 
                this._ScrollRect.verticalNormalizedPosition = 1;
            }

            this._Animator.Play(name);
        }

        public void SetMission(MissionInfoPack pack) 
        {
            var list = pack?.Infos;
            
            var c = 0;
            this._Buttons.ForEach(f =>
            {
                f.gameObject.SetActive(true);

                if (c < list.Count) { f.SetMission(list[c]); }

                else { f.gameObject.SetActive(false); }

                c++;
            });
        }

        public void SetMission() 
        {
            this.SetMission(DataFlow.Current.GetMissions());
        }
    }
}