using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom;
using Custom.Quest;

namespace QuestDemo
{
    [CreateAssetMenu(fileName = "MineMission Series", menuName = "Quest Demo/Mission/MineMission Series", order = 1)]
    public class MineMissions : MissionSeries<MineMission> 
    {
        public class Enumerator : IMissionSeries.Enumerator
        {
            public Enumerator(bool invisible, IEnumerable<IMissionInfo> senders) : base(invisible, senders) { }

            public override void SetInfo(object info)
            {
                if (info.TryConvert(out CoordinateInfo coordinate))
                {
                    base.SetInfo(coordinate);
                }
            }
        }
    }

    [Serializable]
    public class MineMission : IMissionInfo
    {
        [SerializeField]
        private int _QuestSeriesFlag;
        [SerializeField]
        private int _QuestFlag;

        public int QuestSeriesFlag => this._QuestSeriesFlag;
        public int QuestFlag => this._QuestFlag;
        public string Describe => string.Format("Complete {0} - {1}", this._QuestSeriesFlag, this._QuestFlag);

        public object GetInfo() => new Info(this);

        public class Info : IMissionInfo.Info
        {
            public int QuestSeriesFlag { get; }
            public int QuestFlag { get; }

            public Info(MineMission mission) : base(mission.Describe, IMissionInfo.EProgress.UnComplete)
                => (this.QuestSeriesFlag, this.QuestFlag) = (mission.QuestSeriesFlag, mission.QuestFlag);

            public override IMissionInfo.EProgress Achieve(object sender)
            {
                if (this.IsClear) { return this.Progress; }

                if (sender.TryConvert(out CoordinateInfo coordinate))
                {
                    var c = coordinate.Coordinate;

                    if (c[1] + 1 == this.QuestSeriesFlag && c[2] + 1 == this.QuestFlag)
                    {
                        this.IsClear = true;

                        this.Progress = IMissionInfo.EProgress.Complete;
                    }
                }

                return this.Progress;
            }
        }
    }
}
