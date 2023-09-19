using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom;
using Custom.Quest;

namespace QuestDemo
{
    [CreateAssetMenu(fileName = "CountMission Series", menuName = "Quest Demo/Mission/CountMission Series", order = 1)]
    public class CountMissions : MissionSeries<CountMission> 
    {
        public class Enumerator : IMissionSeries.Enumerator 
        {
            public Enumerator(bool invisible, IEnumerable<IMissionInfo> senders) : base(invisible, senders) { }

            public override void SetInfo(object info)
            {
                if (info.TryConvert(out ResultInfo result))
                {
                    base.SetInfo(result);
                }
            }
        }
    }

    [Serializable]
    public class CountMission : IMissionInfo
    {
        [SerializeField]
        private int _Count;

        public int Count => this._Count;
        public string Describe => string.Format("Find {0} Mines.", this.Count);

        public object GetInfo() => new Info(this);

        public class Info : IMissionInfo.Info 
        {
            public int Count { get; }

            public Info(CountMission mission) : base(mission.Describe, IMissionInfo.EProgress.UnComplete)
                => this.Count = mission.Count;

            public override IMissionInfo.EProgress Achieve(object sender)
            {
                if (this.IsClear) { return this.Progress; }

                if (sender.TryConvert(out ResultInfo result)) 
                {
                    if (result.MineGatherd >= this.Count) 
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
