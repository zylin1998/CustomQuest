using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Quest;

namespace QuestDemo
{
    [CreateAssetMenu(fileName = "MineQuest Chapter", menuName = "Quest Demo/Quest Chapter", order = 1)]
    public class MineChapter : Chapter<MineQuestSeries>
    {
        public bool IsFirst => this.Flag <= 0;
        public bool IsLast => this.Flag >= this._QuestSeries.Count - 1;

        public bool MovePrevious()
        {
            if (this.Flag <= 0) { return false; }

            this.Flag--;

            return true;
        }

        public new MineChapter Initialize()
        {
            return base.Initialize() as MineChapter;
        }

        public new MineChapter Reset()
        {
            return base.Reset() as MineChapter;
        }
    }
}