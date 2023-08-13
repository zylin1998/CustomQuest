using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Quest
{
    public interface IQuestSeries : IEnumerable<IQuest>
    {
        public bool IsClear { get; }

        [Serializable]
        public class Enumerator : IEnumerator<IQuest>
        {
            protected IQuest[] _Quests;

            protected int _Flag = -1;

            public bool IsEmpty => this._Quests == null || this._Quests.Length <= 0;
            public bool IsFirst => this._Flag <= 0;
            public bool IsLast => this._Flag >= this._Quests.Length - 1;

            public Enumerator(IEnumerable<IQuest> quests)
            {
                this._Quests = quests.ToArray();
            }

            public IQuest Current => this._Quests[this._Flag];
            
            public bool MoveNext() 
            {
                if (this._Flag >= this._Quests.Length - 1) { return false; }

                if (this._Flag == -1)
                {
                    this._Flag++;

                    return true; 
                }

                var progress = this.Current.Rule.Progress;
                var fulfilled = IRule.EProgress.FulFilled;
                
                if (progress.HasFlag(fulfilled)) 
                {
                    this._Flag++;

                    return true;
                }

                return false;
            }

            public void Reset() 
            {
                this._Flag = -1;
            }

            object IEnumerator.Current => this.Current;

            private bool _DisposedValue;

            public void Dispose()
            {
                Dispose(true);
            }

            // Protected implementation of Dispose pattern.
            protected virtual void Dispose(bool disposing)
            {
                if (!this._DisposedValue)
                {
                    if (disposing)
                    {
                        
                    }

                    this._Quests = null;

                    this._DisposedValue = true;
                }
            }
        }
    }
}
