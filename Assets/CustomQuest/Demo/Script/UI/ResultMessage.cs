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

        public static QuestButton Previous { get; private set; }
        public static QuestButton Restart { get; private set; }
        public static QuestButton Next { get; private set; }

        private void Awake()
        {
            Previous = this["Previous"];
            Restart = this["Restart"];
            Next = this["Next"];
        }

        private void Start()
        {
            QuestDemo.QuestEndEvent += this.ShowMessage;

            this._QuestButtons.ForEach(f => f.ClickEvent += () => this.gameObject.SetActive(false));

            this.gameObject.SetActive(false);
        }

        public void ShowMessage(IQuest quest) 
        {
            var result = quest.Rule.Progress;
            var passTime = quest.GetElement<Timer>().PassTime;

            var strResult = result == IRule.EProgress.FulFilled ? "Win" : "Lose";
            var strPassTime = string.Format("{0, 2}:{1, 2}'{2,3}", passTime.Minute, passTime.Second, passTime.MiniSecond);

            this._ResultText.SetText(string.Format("{0}", strResult));
            this._PassTimeText.SetText(string.Format("{0}", strPassTime));

            this.gameObject.SetActive(true);
        }
    }
}