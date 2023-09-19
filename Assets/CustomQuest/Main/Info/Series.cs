using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom
{
    public abstract class Series<TProvider> : SeriesBase, ISeriesEnumerable<TProvider>
    {
        [SerializeField]
        protected List<TProvider> _Items;

        public override int Depth 
            => 1 + (this._Items.First().TryConvert(out ISeriesEnumerable series) ? series.Depth : 1);

        public override ISeriesEnumerator GetEnumerator() 
            => new SeriesEnumerator<TProvider>(this._Items);

        ISeriesEnumerator<TProvider> ISeriesEnumerable<TProvider>.GetEnumerator()
            => this.GetEnumerator().IsType<SeriesEnumerator<TProvider>>();

        IEnumerator<CoordinateItem<TProvider>> IEnumerable<CoordinateItem<TProvider>>.GetEnumerator()
            => this.GetEnumerator().IsType<SeriesEnumerator<TProvider>>();
    }

    [CreateAssetMenu(fileName = "Provider Series", menuName = "InfoDeliver/Series", order = 1)]
    public class Series : SeriesBase, ISeriesEnumerable<SeriesBase>
    {
        [SerializeField]
        protected List<SeriesBase> _Items;

        public override int Depth 
            => 1 + (this._Items.First().TryConvert(out ISeriesEnumerable series) ? series.Depth : 1);

        public override ISeriesEnumerator GetEnumerator()
            => new SeriesEnumerator<SeriesBase>(this._Items);

        ISeriesEnumerator<SeriesBase> ISeriesEnumerable<SeriesBase>.GetEnumerator()
            => this.GetEnumerator().IsType<SeriesEnumerator<SeriesBase>>();

        IEnumerator<CoordinateItem<SeriesBase>> IEnumerable<CoordinateItem<SeriesBase>>.GetEnumerator()
            => this.GetEnumerator().IsType<SeriesEnumerator<SeriesBase>>();
    }

    public abstract class SeriesBase : ScriptableObject, ISeriesEnumerable
    {
        public abstract int Depth { get; }

        public abstract ISeriesEnumerator GetEnumerator();

        IEnumerator<CoordinateItem> IEnumerable<CoordinateItem>.GetEnumerator() => this.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
