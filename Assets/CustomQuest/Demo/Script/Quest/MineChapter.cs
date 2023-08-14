using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Quest;

namespace QuestDemo
{
    [CreateAssetMenu(fileName = "MineQuest Chapter", menuName = "Quest Demo/Quest Chapter", order = 1)]
    public class MineChapter : Chapter
    {
        [SerializeField]
        private List<MineQuestSeries> _QuestSeries;

        public override bool IsClear => this._QuestSeries.All(a => a.IsClear);

        public bool IsFirst => this._Flag <= 0;
        public bool IsLast => this._Flag >= this._QuestSeries.Count - 1;

        public override QuestSeries Current => this._QuestSeries[this._Flag];

        public override bool MoveNext()
        {
            if (this._Flag == this._QuestSeries.Count - 1) { return false; }

            var isClear = this._Flag >= 0 ? this.Current.IsClear : false;
            
            if (this._Flag == -1 || isClear)
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
            this._QuestSeries.ForEach(f => f.Reset());

            base.Reset();
        }
    }
}