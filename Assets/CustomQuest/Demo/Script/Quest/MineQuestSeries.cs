using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom;

namespace QuestDemo
{
    [CreateAssetMenu(fileName = "Quest Series", menuName = "Quest Demo/Quest/Quest Series", order = 1)]
    public class MineQuestSeries : Series<MineQuestInfo> 
    {
        public override ISeriesEnumerator GetEnumerator()
            => new SeriesEnumerator<MineQuestInfo.Info>(this._Items.ConvertAll(p => p.GetInfo().IsType<MineQuestInfo.Info>()));
    }
}
