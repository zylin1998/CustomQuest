using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Quest;

namespace QuestDemo
{
    [CreateAssetMenu(fileName = "MineQuest Series", menuName = "Quest Demo/Quest Series", order = 1)]
    public class MineQuestSeries : ScriptableObject, IQuestSeries
    {
        [SerializeField]
        private List<MineQuest> _Quests;

        private Enumerator _QuestGetter;

        public Enumerator QuestGetter 
        {
            get
            {
                if (this._QuestGetter.IsEmpty) { this._QuestGetter = new Enumerator(this._Quests); }

                return this._QuestGetter;
            }
        }

        public bool IsClear => this._Quests.All(a => a.Rule.Progress == IRule.EProgress.FulFilled);

        public IEnumerator<IQuest> GetEnumerator() => this.QuestGetter;

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        [Serializable]
        public class Enumerator : IQuestSeries.Enumerator 
        {
            public Enumerator(IEnumerable<IQuest> quests) : base(quests) { }

            public IQuest Next => this.MoveNext() ? this.Current : null;

            public IQuest Previous => this.MovePrevious() ? this.Current : null;

            public bool MovePrevious() 
            {
                if (this._Flag <= 0) { return false; }

                this._Flag--;

                return true;
            }
        }
    }
}
