using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Quest;

namespace QuestDemo
{
    [CreateAssetMenu(fileName = "MineQuest Chapter", menuName = "Quest Demo/Quest/Quest Chapter", order = 1)]
    public class MineChapter : Chapter<MineQuestSeries>
    {
        public bool IsFirst => this.Flag <= 0;
        public bool IsLast => this.Flag >= this._QuestSeries.Count - 1;

        public bool IsFront => this.IsFirst && this.Current.IsFirst;
        public bool IsBack => this.IsLast && this.Current.IsLast;

        public IQuest PreviousQuest
        { 
            get
            {
                if (this.Current.MovePrevious()) 
                {
                    return this.Current.Current;
                }

                if (this.MovePrevious()) 
                {
                    this.Current.Initialize().SetFlagToLast();

                    return this.Current.Current;
                }
                
                return null;
            }
        }

        public IQuest NextQuest
        {
            get
            {
                if (this.MoveNext())
                {
                    this.Current.Initialize().SetFlagToFirst();
                }

                if (this.Current.MoveNext())
                {
                    return this.Current.Current;
                }

                return null;
            }
        }

        public bool MovePrevious()
        {
            if (this.Flag <= 0) { return false; }

            this.Flag--;

            return true;
        }
    }
}