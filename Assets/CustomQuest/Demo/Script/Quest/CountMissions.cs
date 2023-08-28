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
        public override IMissionSeries Initialize()
        {
            MissionEnumeration.MineCount.Register(this.CheckMission);

            return base.Initialize();
        }

        public override IMissionSeries Initialize(InitArgs args)
        {
            MissionEnumeration.MineCount.Register(this.CheckMission);

            return base.Initialize(args);
        }
    }

    [Serializable]
    public class CountMission : IMission
    {
        [SerializeField]
        private int _Count;
        [SerializeField]
        private IMission.EProgress _Progress;

        private int _CurrentCount = 0;

        public int Count => this._Count;
        public bool IsClear => this.Progress.HasFlag(IMission.EProgress.End);
        public IReward Reward => null;
        public IMission.EProgress Progress { get => this._Progress; private set => this._Progress = value; }
        public string Describe => string.Format("Find Mine {0} / {1}", this._CurrentCount, this.Count);

        public IMission Initialize()
        {
            this._CurrentCount = 0;

            this.Progress = IMission.EProgress.UnComplete;

            return this;
        }

        public IMission Initialize(InitArgs args)
        {
            if (args is MineMissionInitArgs init) { this.Progress = init.Progress; }

            else { this.Progress = IMission.EProgress.UnComplete; }

            return this;
        }

        public IMission End()
        {
            this.Progress = IMission.EProgress.End;

            return this;
        }

        public IMission.EProgress OnValueChange(MissionArgs args)
        {
            if (this.IsClear) { return this.Progress; }

            if (args is MineCountArgs missionArgs)
            {
                return OnValueChange(this, missionArgs);
            }

            return IMission.EProgress.None;
        }

        private static IMission.EProgress OnValueChange(CountMission mission, MineCountArgs args)
        {
            var complete = IMission.EProgress.Complete;
            var unComplete = IMission.EProgress.UnComplete;

            mission._CurrentCount = Mathf.Clamp(args.Count, 0, mission.Count);

            mission.Progress = mission._CurrentCount >= mission._Count ? complete : unComplete;

            return mission.Progress;
        }
    }

    public class MineCountArgs : MissionArgs 
    {
        public int Count { get; }

        public MineCountArgs(int count) => this.Count = count;
    }
}
