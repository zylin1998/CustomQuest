using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Quest;

namespace QuestDemo
{
    [CreateAssetMenu(fileName = "MineQuest Series", menuName = "Quest Demo/Quest Series", order = 1)]
    public class MineQuestSeries : QuestSeries<MineQuest>
    {
        public bool IsFirst => this.Flag <= 0;
        public bool IsLast => this.Flag >= this._Quests.Count - 1;

        public IQuest Next => this.MoveNext() ? this.Current : this.Current;
        public IQuest Previous => this.MovePrevious() ? this.Current : this.Current;

        public bool MovePrevious()
        {
            if (this.Flag <= 0) { return false; }

            this.Flag--;

            return true;
        }

        public new MineQuestSeries Initialize() 
        {
            return base.Initialize() as MineQuestSeries;
        }

        public new MineQuestSeries Reset()
        {
            return base.Reset() as MineQuestSeries;
        }
    }
}
