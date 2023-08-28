using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Quest;

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
                    var size = mineRule.Size;
                    var map = MineRule.CreateMap(mineRule);

                    this._QuestDemoUI.SetMineMap(new MineMapArgs(size, map));
                }

                var coordinate = this._DemoQuestChapter.Coordinate;

                this._QuestDemoUI.SetQuestDetail(new QuestDetailArgs(coordinate));
            }
        }

        #endregion

        #region Script Behaviour

        private void Start()
        {
            IMine.DetectedEvent += this.CheckRule;

            ResultMessage.Previous.ClickEvent += this.PreviousQuest;
            ResultMessage.Restart.ClickEvent += this.RestartQuest;
            ResultMessage.Next.ClickEvent += this.NextQuest;

            this._DemoQuestChapter.Reset();

            this.NextQuest();
        }

        private void OnDestroy()
        {
            IMine.DetectedEvent -= this.CheckRule;

            ResultMessage.Previous.ClickEvent -= this.PreviousQuest;
            ResultMessage.Restart.ClickEvent -= this.RestartQuest;
            ResultMessage.Next.ClickEvent -= this.NextQuest;
        }

        #endregion

        #region Quest Manage

        public void PreviousQuest() => this.StartQuest(this._DemoQuestChapter.PreviousQuest);
        public void NextQuest() => this.StartQuest(this._DemoQuestChapter.NextQuest);
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

            IMine.DetectedType = EMineMap.Space;
            
            this.Quest.Start();
        }

        public void CheckRule(MapArgs variation) 
        {
            var quest = this._Quest as MineQuest;
            var chapter = this._DemoQuestChapter;
            
            quest.Rule.CheckRule(variation);
            
            var resultArgs = new RuleResultArgs(quest, chapter);
            
            this._QuestDemoUI.SetResult(resultArgs);
        }

        #endregion
    }
}