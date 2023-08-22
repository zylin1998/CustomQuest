using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Quest
{
    public abstract class QuestSeries<TQuest> : ScriptableObject, IQuestSeries<TQuest>  where TQuest : IQuest
    {
        [SerializeField]
        protected List<TQuest> _Quests;

        [DefaultValue(-1)]
        public int Flag { get; protected set; }
        public bool IsClear => this._Quests.All(a => a.HasCleared);
        public int Depth => 1 + (this._Quests.First() is IQuestSeries series ? series.Depth : 1);
        public Coordinate Coordinate => new Coordinate(this.Flag);

        public TQuest Current => this._Quests[this.Flag];

        public virtual bool MoveNext()
        {
            if (this.Flag >= this._Quests.Count - 1) { return false; }

            var hasCleared = this.Flag >= 0 ? this.Current.HasCleared : false;

            if (this.Flag == -1 || hasCleared)
            {
                this.Flag++;

                return true;
            }

            return false;
        }

        public void SetFlag(int flag) => this.Flag = flag;
        public void SetFlagToFirst() => this.SetFlag(-1);
        public void SetFlagToLast() => this.SetFlag(this._Quests.Count - 1);

        public virtual IQuestSeries<TQuest> Initialize() 
        {
            this._Quests.ForEach(f => f.Initialize());

            return this;
        }

        public virtual IQuestSeries<TQuest> Reset()
        {
            this.Flag = -1;

            this._Quests.ForEach(f => f.Reset());

            return this;
        }

        void IEnumerator.Reset() => this.Reset();

        object IEnumerator.Current => this.Current;

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

                this._Quests = null;

                this._DisposedValue = true;
            }
        }

        #endregion
    }

    public interface IQuestSeries : IEnumerator, IClear
    {
        public int Flag { get; }
        public int Depth { get; }
        public Coordinate Coordinate { get; }

        public void SetFlag(int flag);
        public void SetFlagToFirst();
        public void SetFlagToLast();

        public IQuestSeries Initialize();
        public new IQuestSeries Reset();
    }

    public interface IQuestSeries<TClearable> : IQuestSeries, IEnumerator<TClearable> where TClearable : IClear
    {
        public new IQuestSeries<TClearable> Initialize();
        public new IQuestSeries<TClearable> Reset();

        IQuestSeries IQuestSeries.Initialize() => this.Initialize();
        IQuestSeries IQuestSeries.Reset() => this.Reset();
    }

    public struct Coordinate : IEnumerable<int>
    {
        public List<int> Position { get; }

        public Coordinate(int position) : this(new int[] { position }) { }
        public Coordinate(IEnumerable<int> positions) => this.Position = new List<int>(positions);

        public int this[int index] => this.Position[index];

        public static Coordinate operator +(Coordinate first, Coordinate second)
        {
            first.Position.AddRange(second.Position);

            return first;
        }

        public string CoordinateID => string.Join(" - ", this.Position.ConvertAll(c => ++c));

        public IEnumerator<int> GetEnumerator() => this.Position.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
