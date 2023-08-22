using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Quest
{
    public abstract class MissionList<TMission> : ScriptableObject, IEnumerable<TMission> where TMission : IMission
    {
        [SerializeField]
        private List<TMission> _Missions;

        public List<TMission> OnGoing => this._Missions.FindAll(f => !f.Progress.HasFlag(IMission.EProgress.End) || !f.Progress.HasFlag(IMission.EProgress.None));
        public List<TMission> End => this._Missions.FindAll(f => f.Progress.HasFlag(IMission.EProgress.End));

        public MissionList<TMission> Initialize() 
        {
            this._Missions.ForEach(f => f.Initialize());

            return this;
        }

        public MissionList<TMission> Start()
        {
            this._Missions.ForEach(f =>
            {
                if (f.Progress == IMission.EProgress.Start) { f.Start(); }
            });

            return this;
        }

        public IEnumerator<TMission> GetEnumerator() => this.OnGoing.Concat(this.End).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}