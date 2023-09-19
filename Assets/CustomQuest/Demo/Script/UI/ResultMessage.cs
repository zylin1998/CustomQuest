using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Custom.Quest;

namespace QuestDemo
{
    public class ResultMessage : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _ResultText;
        [SerializeField]
        private TextMeshProUGUI _PassTimeText;
        [SerializeField]
        private List<QuestButton> _QuestButtons;

        public QuestButton this[string name] => this._QuestButtons.Find(f => f.Name == name);

        private QuestButton _Previous;
        private QuestButton _Restart;
        private QuestButton _Next;

        private void Awake()
        {
            this._Previous = this["Previous"];
            this._Restart = this["Restart"];
            this._Next = this["Next"];
            
            this._Previous.ClickEvent += MineSweeper.PreviousQuest;
            this._Restart.ClickEvent += MineSweeper.RestartQuest;
            this._Next.ClickEvent += MineSweeper.NextQuest;
        }

        private void Start()
        {
            this._QuestButtons.ForEach(f => f.ClickEvent += () => this.gameObject.SetActive(false));

            this.gameObject.SetActive(false);
        }

        public void ShowMessage(ResultInfo result, CoordinateInfo coordinate) 
        {
            var passTime = result.PassTime;

            var strResult = string.Empty;

            if (result.Progress == IRule.EProgress.FulFilled) { strResult = "Win"; }
            if (result.Progress == IRule.EProgress.Failed) { strResult = "Lose"; }

            var strPassTime = string.Format("{0, 2}:{1, 2} {2,3}", passTime.Minute, passTime.Second, passTime.MiniSecond);

            this._ResultText.SetText(string.Format("{0}", strResult));
            this._PassTimeText.SetText(string.Format("{0}", strPassTime));

            this._Previous.Interactable = !coordinate.IsFront;
            this._Next.Interactable = !coordinate.IsBack && result.IsCleared;

            this.gameObject.SetActive(true);
        }
    }
}