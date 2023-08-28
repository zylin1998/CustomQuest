using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Quest
{
    [CreateAssetMenu(fileName = "Mission Collect", menuName = "Quest Demo/Mission/Mission Collect", order = 1)]
    public class MissionCollect : ScriptableObject, IMissionCollect
    {
        [SerializeField]
        private List<MissionSeries> _MissionSeries;

        public List<IMission> Missions 
        {
            get 
            {
                var complete = new List<IMission>();
                var onGoing = new List<IMission>();
                var end = new List<IMission>();

                this.ToList().ForEach(series =>
                {
                    complete.AddRange(series.Complete);
                    onGoing.AddRange(series.OnGoing);
                    end.AddRange(series.End);
                });

                return complete.Concat(onGoing).Concat(end).ToList();
            }
        }

        public IMissionCollect Initialize() 
        {
            this.ToList().ForEach(f => f.Initialize());

            return this;
        }

        public IMissionCollect Initialize(InitArgs args)
        {
            if (args is MissionCollectInitArgs init)
            {
                var c = 0;
                this.ToList().ForEach(series =>
                {
                    series.Initialize(init[c]);

                    c++;
                });
            }
            return this;
        }

        public IEnumerator<IMissionSeries> GetEnumerator() => this._MissionSeries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public interface IMissionCollect : IInitialize<IMissionCollect>, IEnumerable<IMissionSeries>
    {
        public List<IMission> Missions { get; }
    }

    public class MissionCollectInitArgs : InitArgs
    {
        List<MissionSeriesArgs> SeriesArgs { get; }

        public MissionSeriesArgs this[int num] => num < this.SeriesArgs.Count ? this.SeriesArgs[num] : default;

        public MissionCollectInitArgs(IEnumerable<MissionSeriesArgs> seriesArgs) 
        {
            this.SeriesArgs = seriesArgs.ToList();
        }
    }
}
