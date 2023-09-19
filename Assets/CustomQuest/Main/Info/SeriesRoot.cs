using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom
{
    [CreateAssetMenu(fileName = "Series Root", menuName = "InfoDeliver/Series Root", order = 1)]
    public class SeriesRoot : Series<SeriesBase>
    {
        public override int Depth => this._Items.First().TryConvert(out ISeriesEnumerable series) ? series.Depth : 1;
    }
}
