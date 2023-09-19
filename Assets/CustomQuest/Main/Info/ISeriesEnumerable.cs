using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Custom
{
    public interface ISeriesEnumerable<T> : ISeriesEnumerable, IEnumerable<CoordinateItem<T>>
    {
        public new ISeriesEnumerator<T> GetEnumerator();
    }

    public interface ISeriesEnumerable : IEnumerable<CoordinateItem>
    {
        public int Depth { get; }

        public new ISeriesEnumerator GetEnumerator();
    }

    public interface ISeriesEnumerator<T> : ISeriesEnumerator, IEnumerator<CoordinateItem<T>>
    {

    }

    public interface ISeriesEnumerator : IEnumerator<CoordinateItem>
    {
        public int Flag { get; }
        public bool IsFirst { get; }
        public bool IsLast { get; }
        public Coordinate Coordinate { get; }

        public CoordinateItem DepthCurrent(int depth);

        public bool MoveNext(int depth);
    }

    public class SeriesEnumerator<T> : ISeriesEnumerator<T>
    {
        protected EnumeratorItem[] _Items;

        public int Flag { get; protected set; } = -1;
        public bool IsFirst => this.Flag == 0;
        public bool IsLast => this.Flag == this._Items.Length - 1;

        public SeriesEnumerator(IEnumerable<T> enumerable)
            => this._Items = enumerable.Select(item => new EnumeratorItem(item)).ToArray();

        public Coordinate Coordinate
        {
            get
            {
                var flag = this.Flag <= 0 ? 0 : this.Flag;
                var coordinate = new Coordinate(flag);
                var enumerator = this._Items[this.Flag].Enumerator;

                return enumerator != null ? coordinate + enumerator.Coordinate : coordinate;
            }
        }

        public CoordinateItem Current
        {
            get
            {
                if (this.Flag < 0) { return default; }

                var item = this._Items[this.Flag];

                return new(this.Coordinate, item.Enumerator != null ? item.Enumerator.Current.Item : item.Item);
            }
        }

        object IEnumerator.Current => this.Current;
        CoordinateItem<T> IEnumerator<CoordinateItem<T>>.Current
            => this.Current.Item is T result ? new(this.Coordinate, result) : default;

        public CoordinateItem DepthCurrent(int depth)
        {
            if (this.Flag < 0) { return default; }

            var item = this._Items[this.Flag];

            if (depth == 1) { return new CoordinateItem(new(this.Flag), item.Item); }

            if (item.Enumerator != null)
            {
                var current = item.Enumerator.DepthCurrent(--depth);

                return new CoordinateItem(new Coordinate(this.Flag) + current.Coordinate, current.Item);
            }

            return default;
        }

        public bool MoveNext()
        {
            var hasMove = false;

            if (this.Flag == -1)
            {
                this.Flag = 0;

                hasMove = true;
            }

            var enumerator = this._Items[this.Flag].Enumerator;

            if (!hasMove && this.Flag < this._Items.Length - 1)
            {
                var canMove = enumerator != null ? enumerator.IsLast : true;

                this.Flag += canMove ? 1 : 0;

                hasMove = canMove;
            }

            enumerator = this._Items[this.Flag].Enumerator;

            return enumerator != null ? enumerator.MoveNext() : hasMove;
        }

        public bool MoveNext(int depth)
        {
            var hasMove = false;

            if (this.Flag == -1)
            {
                this.Flag = 0;

                hasMove = true;
            }

            var enumerator = this._Items[this.Flag].Enumerator;

            if (!hasMove && this.Flag < this._Items.Length - 1)
            {
                var canMove = false;

                if (depth == 1) { canMove = true; }

                else
                {
                    canMove = enumerator != null ? enumerator.IsLast : true;
                }

                this.Flag += canMove ? 1 : 0;

                hasMove = canMove;
            }

            enumerator = this._Items[this.Flag].Enumerator;

            if (depth > 1 && enumerator != null)
            {
                return enumerator.MoveNext(--depth);
            }

            return hasMove;
        }

        public void Reset()
        {
            this.Flag = -1;

            this._Items.ToList().ForEach(e => e.Enumerator?.Reset());
        }

        #region Dispose

        private bool _DisposeValue = false;

        public void Dispose() => this.Dispose(false);

        public void Dispose(bool disposing)
        {
            if (!_DisposeValue)
            {
                if (disposing)
                {

                }

                this._Items = null;
                this._DisposeValue = true;
            }
        }

        #endregion

        protected class EnumeratorItem
        {
            public T Item { get; }
            public ISeriesEnumerator Enumerator { get; }

            public EnumeratorItem(T item)
            {
                this.Item = item;

                this.Enumerator = item is ISeriesEnumerable enumerable ? enumerable.GetEnumerator() : default;
            }
        }
    }

    public struct Coordinate : IEnumerable<int>
    {
        public List<int> Position { get; }

        public Coordinate(int position) : this(new int[] { position }) { }
        public Coordinate(IEnumerable<int> positions) => this.Position = new List<int>(positions);

        public int this[int index] => this.Position[index];

        public static Coordinate operator +(Coordinate first, Coordinate second)
        {
            var posi = new List<int>();

            posi.AddRange(first.Position);
            posi.AddRange(second.Position);

            return new Coordinate(posi);
        }

        public string CoordinateID => string.Join(" - ", this.Position.ConvertAll(c => ++c));

        public IEnumerator<int> GetEnumerator() => this.Position.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public class CoordinateItem
    {
        public Coordinate Coordinate { get; }
        public object Item { get; }

        public CoordinateItem(Coordinate coordinate, object item)
            => (this.Coordinate, this.Item) = (coordinate, item);
    }

    public class CoordinateItem<T> : CoordinateItem
    {
        public new T Item { get; }

        public CoordinateItem(Coordinate coordinate, T item) : base(coordinate, item)
            => this.Item = item;
    }
}
