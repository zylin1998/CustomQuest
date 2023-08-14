using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Quest;
using TMPro;

namespace QuestDemo
{
    public class QuestDemo : MonoBehaviour
    {
        [SerializeField]
        private MineChapter _DemoQuestChapter;
        [SerializeField]
        private MineArea _MineArea;
        [SerializeField]
        private ResultMessage _ResultMessage;
        [SerializeField]
        private TextMeshProUGUI _MineCount;
        [SerializeField]
        private ImageDetail _ImageDetail;

        #region Properties
        
        private IQuest _Quest;

        public IQuest Quest 
        {
            get => this._Quest;

            private set 
            {
                this._Quest = value;

                if (value.Rule is MineRule mineRule) 
                {
                    this.Rule = mineRule;
                    Debug.Log(value);
                    this._MineArea.Square = mineRule.Size;
                }
            }
        }

        private MineQuestSeries _DemoQuestSeries;

        public MineRule Rule { get; private set; }

        #endregion

        #region Static Properties

        public static ImageDetail ImageDetail { get; private set; }
        public static QuestDemo Demo { get; private set; }

        private static EMineMap _CheckType;

        public static EMineMap CheckType 
        { 
            get => _CheckType;

            set 
            {
                _CheckType = value;

                OnTypeChanged.Invoke(value);
            }
        }

        private static System.Action<EMineMap> OnTypeChanged = (type) => { };

        public static event System.Action<EMineMap> TypeChangedEvent 
        {
            add => OnTypeChanged += value;

            remove => OnTypeChanged -= value;
        }

        private static System.Action<IQuest> OnQuestEnd = (quest) => { };

        public static event System.Action<IQuest> QuestEndEvent
        {
            add => OnQuestEnd += value;

            remove => OnQuestEnd -= value;
        }

        #endregion

        private void Awake()
        {
            Demo = this;
            
            ImageDetail = this._ImageDetail;
        }

        #region Script Behaviour

        private void Start()
        {
            MineButton.DetectedEvent += this.CheckRule;

            this._ResultMessage["Previous"].ClickEvent += this.PreviousQuest;
            this._ResultMessage["Restart"].ClickEvent += this.RestartQuest;
            this._ResultMessage["Next"].ClickEvent += this.NextQuest;

            this._DemoQuestChapter.Reset();

            this.NextQuest();
        }

        private void OnDestroy()
        {
            Demo = null;
            ImageDetail = default;
            MineButton.DetectedEvent -= this.CheckRule;
        }

        #endregion

        #region Quest Manage

        public void SetQuest(IQuest quest) 
        {
            this.Quest = quest;

            this.Quest.Initialize();
        }

        public void PreviousQuest()
        {
            if (this._DemoQuestChapter.IsFirst && this._DemoQuestSeries.IsFirst) { return; }

            var quest = default(IQuest);

            if (this._DemoQuestSeries.IsFirst)
            {
                this._DemoQuestChapter.MovePrevious();

                this._DemoQuestSeries = this._DemoQuestChapter.Current as MineQuestSeries;

                quest = this._DemoQuestSeries.Current;
            }
            else
            {
                quest = this._DemoQuestSeries.Previous;
            }

            this.StartQuest(quest);
        }

        public void NextQuest()
        {
            if (this._DemoQuestChapter.IsLast && this._DemoQuestSeries.IsLast) { return; }

            var quest = default(IQuest);

            if (this._DemoQuestSeries == null || this._DemoQuestSeries.IsLast)
            {
                this._DemoQuestChapter.MoveNext();

                this._DemoQuestSeries = this._DemoQuestChapter.Current as MineQuestSeries;
            }
            
            quest = this._DemoQuestSeries.Next; 

            this.StartQuest(quest);
        }
        public void RestartQuest() => this.StartQuest(this.Quest);

        public void StartQuest(IQuest quest) 
        {
            if (quest != null)
            {
                this.SetQuest(quest);
            
                this.GameStart();
            }
        }

        public void GameStart() 
        {
            var map = this.Rule.CreateMap(this.Rule.MineCount);

            this._MineCount.SetText(string.Format("{0}", this.Rule.FakeMineCount));

            this._MineArea.SetMine(map);

            CheckType = EMineMap.Space;

            this.Quest.Start();
        }

        public void CheckRule(MapVariation variation) 
        {
            var quest = this._Quest as MineQuest;
            var result = this.Rule.CheckRule(variation);

            this._MineCount.SetText(string.Format("{0}", this.Rule.FakeMineCount));

            if (quest.IsFailed) { this._MineArea.ShowMine(); }

            if (quest.IsClear || quest.IsFailed) 
            {
                this._Quest.End();

                var isFirst = this._DemoQuestChapter.IsFirst && this._DemoQuestSeries.IsFirst;
                var isLast = this._DemoQuestChapter.IsLast && this._DemoQuestSeries.IsLast;

                this._ResultMessage["Previous"].Interactable = !isFirst;
                this._ResultMessage["Next"].Interactable = !isLast && quest.HasCleared;

                OnQuestEnd?.Invoke(this.Quest);
            }
        }

        #endregion
    }
}