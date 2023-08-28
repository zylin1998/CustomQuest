using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Quest
{
    public abstract class MissionSeries : ScriptableObject, IMissionSeries
    {
        [SerializeField]
        private bool _Invisible;
        
        public bool Invisible => this._Invisible;

        public abstract List<IMission> OnGoing { get; }
        public abstract List<IMission> AllOnGoing { get; }
        public abstract List<IMission> Complete { get; }
        public abstract List<IMission> End { get; }

        public abstract IMissionSeries Initialize();
        public abstract IMissionSeries Initialize(InitArgs args);

        public virtual void CheckMission(MissionArgs args)
        {
            this.AllOnGoing.ForEach(f => f.OnValueChange(args));
        }

        public IEnumerator<IMission> GetEnumerator() => this.Complete.Concat(this.OnGoing).Concat(this.End).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public abstract class MissionSeries<TMission> : MissionSeries, IMissionSeries where TMission : IMission
    {
        [SerializeField]
        private List<TMission> _Missions;

        public override List<IMission> OnGoing
        {
            get
            {
                var list = this.AllOnGoing;

                return this.Invisible ? new List<IMission> { list.FirstOrDefault() } : list;
            }
        }
        public override List<IMission> AllOnGoing 
            => IMissionSeries.SearchMissions(this._Missions, mission => mission.Progress == IMission.EProgress.UnComplete);
        public override List<IMission> Complete 
            => IMissionSeries.SearchMissions(this._Missions, mission => mission.Progress == IMission.EProgress.Complete);
        public override List<IMission> End
            => IMissionSeries.SearchMissions(this._Missions, mission => mission.Progress == IMission.EProgress.End);

        public override IMissionSeries Initialize() 
        {
            this._Missions.ForEach(f => f.Initialize());

            return this;
        }

        public override IMissionSeries Initialize(InitArgs args)
        {
            if (args is MissionSeriesArgs inits)
            {
                var c = 0;
                this._Missions.ForEach(f =>
                {
                    f.Initialize(inits.MissionArgs[c]);
                    
                    c++;
                });
            }

            return this;
        }
    }

    public interface IMissionSeries : IInitialize<IMissionSeries>, IEnumerable
    {
        public bool Invisible { get; }

        public List<IMission> Complete { get; }
        public List<IMission> OnGoing { get; }
        public List<IMission> AllOnGoing { get; }
        public List<IMission> End { get; }

        public void CheckMission(MissionArgs args);

        protected static List<IMission> SearchMissions<TMission>
            (IEnumerable<TMission> missions, Func<TMission, bool> predicate) where TMission : IMission 
        {
            return missions.Where(predicate).OfType<IMission>().ToList();
        }
    }

    public class MissionSeriesArgs : InitArgs 
    {
        public List<MissionInitArgs> MissionArgs { get; }

        public MissionInitArgs this[int num] => num < this.MissionArgs.Count ? this.MissionArgs[num] : default;

        public MissionSeriesArgs(IEnumerable<MissionInitArgs> missionArgs) 
        {
            this.MissionArgs = missionArgs.ToList();
        }
    }
}