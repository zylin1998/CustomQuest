using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Custom.Quest;
using TMPro;

namespace QuestDemo
{
    public class QuestDemo : MonoBehaviour
    {
        [SerializeField]
        private MineChapter _DemoQuestChapter;
        [SerializeField]
        private QuestDemoUI _QuestDemoUI;
        
        #region Properties
        
        private IQuest _Quest;

        public IQuest Quest 
        {
            get => this._Quest;

            private set 
            {
                this._Quest = value;
                
                if (this._Quest.Initialize().Rule is MineRule mineRule) 
                {
                    this.Rule = mineRule;
                    
                    var size = mineRule.Size;
                    var map = MineRule.CreateMap(mineRule);

                    this._QuestDemoUI.SetMineMap(new MineMapArgs(size, map));
                }

                var coordinate = this._DemoQuestChapter.Coordinate;

                this._QuestDemoUI.SetQuestDetail(new QuestDetailArgs(coordinate));
            }
        }

        private MineQuestSeries _DemoQuestSeries;

        public MineRule Rule { get; private set; }

        #endregion

        #region Static Properties

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

        #region Script Behaviour

        private void Start()
        {
            MineButton.DetectedEvent += this.CheckRule;

            ResultMessage.Previous.ClickEvent += this.PreviousQuest;
            ResultMessage.Restart.ClickEvent += this.RestartQuest;
            ResultMessage.Next.ClickEvent += this.NextQuest;

            this._DemoQuestChapter.Reset();

            this.NextQuest();
        }

        private void OnDestroy()
        {
            MineButton.DetectedEvent -= this.CheckRule;

            ResultMessage.Previous.ClickEvent -= this.PreviousQuest;
            ResultMessage.Restart.ClickEvent -= this.RestartQuest;
            ResultMessage.Next.ClickEvent -= this.NextQuest;
        }

        #endregion

        #region Quest Manage

        public void PreviousQuest()
        {
            if (this._DemoQuestChapter.IsFirst && this._DemoQuestSeries.IsFirst) { return; }

            var quest = default(IQuest);

            if (this._DemoQuestSeries.IsFirst)
            {
                this._DemoQuestChapter.MovePrevious();

                this._DemoQuestSeries = this._DemoQuestChapter.Initialize().Current;
                this._DemoQuestSeries.SetFlagToLast();

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

                this._DemoQuestSeries = this._DemoQuestChapter.Initialize().Current;
                this._DemoQuestSeries.SetFlagToFirst();
            }
            
            quest = this._DemoQuestSeries.Next; 

            this.StartQuest(quest);
        }

        public void RestartQuest() => this.StartQuest(this.Quest);

        public void StartQuest(IQuest quest) 
        {
            if (quest != null)
            {
                this.Quest = quest;
            
                this.GameStart();
            }
        }

        public void GameStart() 
        {
            this.CheckRule(new MapArgs(0, 0, EMineMap.None));

            CheckType = EMineMap.Space;

            this.Quest.Start();
        }

        public void CheckRule(MapArgs variation) 
        {
            var result = this.Rule.CheckRule(variation);
            var quest = this._Quest as MineQuest;
            var isFirst = this._DemoQuestChapter.IsFirst && this._DemoQuestSeries.IsFirst;
            var isLast = this._DemoQuestChapter.IsLast && this._DemoQuestSeries.IsLast;
            var fakeMineCount = this.Rule.FakeMineCount;

            this._QuestDemoUI.SetResult(new RuleResultArgs(fakeMineCount, result, quest, isFirst, isLast));

            if (quest.IsClear || quest.IsFailed) 
            {
                OnQuestEnd?.Invoke(this.Quest);
            }
        }

        #endregion
    }
}