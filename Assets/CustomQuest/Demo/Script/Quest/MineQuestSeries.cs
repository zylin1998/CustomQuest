using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Quest;

namespace QuestDemo
{
    [CreateAssetMenu(fileName = "MineQuest Series", menuName = "Quest Demo/Quest Series", order = 1)]
    public class MineQuestSeries : QuestSeries
    {
        [SerializeField]
        private List<MineQuest> _Quests;

        public override bool IsClear => this._Quests.All(a => a.Rule.Progress == IRule.EProgress.FulFilled);

        public bool IsFirst => this._Flag <= 0;
        public bool IsLast => this._Flag >= this._Quests.Count - 1;

        public IQuest Next => this.MoveNext() ? this.Current : this.Current;
        public IQuest Previous => this.MovePrevious() ? this.Current : this.Current;

        public override IQuest Current => this._Quests[this._Flag];

        public override bool MoveNext()
        {
            if (this._Flag >= this._Quests.Count - 1) { return false; }

            var hasCleared = this._Flag >= 0 ? (this.Current as MineQuest).HasCleared : false;
            
            if (this._Flag == -1 || hasCleared)
            {
                this._Flag++;

                return true;
            }

            return false;
        }

        public bool MovePrevious()
        {
            if (this._Flag <= 0) { return false; }

            this._Flag--;

            return true;
        }

        public override void Reset()
        {
            this._Quests.ForEach(f => f.Reset());

            base.Reset();
        }
    }
}
