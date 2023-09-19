using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom;

namespace Custom.Quest
{
    public abstract class MissionSeries<TMissionInfo> : Series<TMissionInfo>, IMissionSeries where TMissionInfo : IMissionInfo
    {
        [SerializeField]
        private bool _Invisible;

        public bool Invisible => this._Invisible;

        public override ISeriesEnumerator GetEnumerator() 
            => new IMissionSeries.Enumerator(Invisible, this._Items.ConvertAll(item => item.IsType<IMissionInfo>()));

        ISeriesEnumerator<IMissionInfo> ISeriesEnumerable<IMissionInfo>.GetEnumerator() 
            => this.GetEnumerator().IsType<ISeriesEnumerator<IMissionInfo>>();

        IEnumerator<CoordinateItem<IMissionInfo>> IEnumerable<CoordinateItem<IMissionInfo>>.GetEnumerator()
            => this.GetEnumerator().IsType<IEnumerator<CoordinateItem<IMissionInfo>>>();
    }

    public interface IMissionSeries : ISeriesEnumerable<IMissionInfo>
    {
        public bool Invisible { get; }

        public class Enumerator : SeriesEnumerator<IMissionInfo>, IReciever
        {
            public bool Invisible { get; }

            private List<IMissionInfo.Info> _Flatten;

            public List<IMissionInfo.Info> UnComplete
            {
                get
                {
                    var list = this.AllUnComplete;

                    return this.Invisible ? new() { list.FirstOrDefault() } : list; 
                }
            }

            public List<IMissionInfo.Info> Complete 
                => IMissionInfo.SelectMission(this._Flatten, mission => mission.Progress == IMissionInfo.EProgress.Complete);
            public List<IMissionInfo.Info> AllUnComplete
                => IMissionInfo.SelectMission(this._Flatten, mission => mission.Progress == IMissionInfo.EProgress.UnComplete);
            public List<IMissionInfo.Info> End
                => IMissionInfo.SelectMission(this._Flatten, mission => mission.Progress == IMissionInfo.EProgress.End);

            public Enumerator(bool invisible, IEnumerable<IMissionInfo> senders) : base(senders)
            {
                this.Invisible = invisible;
                
                this._Flatten = this._Items
                                        .Select(item => item.Item.GetInfo())
                                        .OfType<IMissionInfo.Info>().ToList();
            }

            public virtual void SetInfo(object info) 
            {
                this.AllUnComplete.ForEach(mission => mission.Achieve(info));
            }
        }
    }
}