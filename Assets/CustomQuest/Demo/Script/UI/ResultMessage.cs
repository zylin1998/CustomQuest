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
            this._QuestButtons.ForEach(f => f.ClickEvent += () => this.gameObject.SetActive(false));

            this.gameObject.SetActive(false);
        }

        public void ShowMessage(RuleResultArgs args) 
        {
            var passTime = (args.Quest as IQuest).GetElement<Timer>().PassTime;

            var strResult = args.Progress == IRule.EProgress.FulFilled ? "Win" : "Lose";
            var strPassTime = string.Format("{0, 2}:{1, 2} {2,3}", passTime.Minute, passTime.Second, passTime.MiniSecond);

            this._ResultText.SetText(string.Format("{0}", strResult));
            this._PassTimeText.SetText(string.Format("{0}", strPassTime));

            Previous.Interactable = !args.IsFront;
            Next.Interactable = !args.IsBack && args.Quest.HasCleared;

            this.gameObject.SetActive(true);
        }
    }
}