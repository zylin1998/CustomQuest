using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Quest
{
    public abstract class Chapter : ScriptableObject, IQuestSeries<QuestSeries>
    {
        protected int _Flag = -1;

        public int Flag => this._Flag;
        public abstract bool IsClear { get; }
        public abstract QuestSeries Current { get; }

        object IEnumerator.Current => this.Current;

        public abstract bool MoveNext();

        public virtual void Reset()
        {
            this._Flag = -1;
        }

        #region Dispose

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

                this._DisposedValue = true;
            }
        }

        #endregion
    }
}
