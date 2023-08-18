using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Quest
{
    public abstract class Chapter<TQuestSeries> : ScriptableObject, IQuestSeries<TQuestSeries> where TQuestSeries : IQuestSeries
    {
        [SerializeField]
        protected List<TQuestSeries> _QuestSeries;

        [DefaultValue(-1)]
        public int Flag { get; protected set; }
        public bool IsClear => this._QuestSeries.All(a => a.IsClear);
        public int Depth => 1 + (this._QuestSeries.First() is IQuestSeries series ? series.Depth : 1);
        public Coordinate Coordinate => new Coordinate(this.Flag) + this.Current.Coordinate;

        public TQuestSeries Current => this._QuestSeries[this.Flag];

        object IEnumerator.Current => this.Current;

        public bool MoveNext()
        {
            if (this.Flag == this._QuestSeries.Count - 1) { return false; }

            var isClear = this.Flag >= 0 ? this.Current.IsClear : false;

            if (this.Flag == -1 || isClear)
            {
                this.Flag++;

                return true;
            }

            return false;
        }

        public void SetFlag(int flag) => this.Flag = flag; 
        public void SetFlagToFirst() => this.SetFlag(-1);
        public void SetFlagToLast() => this.SetFlag(this._QuestSeries.Count - 1);


        public virtual IQuestSeries<TQuestSeries> Initialize() 
        {
            this._QuestSeries.ForEach(f => f.Initialize());

            return this;
        }

        public virtual IQuestSeries<TQuestSeries> Reset()
        {
            this.Flag = -1;

            this._QuestSeries.ForEach(f => f.Reset());

            return this;
        }

        void IEnumerator.Reset() => this.Reset();

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

                this._QuestSeries = null;

                this._DisposedValue = true;
            }
        }

        #endregion
    }
}
