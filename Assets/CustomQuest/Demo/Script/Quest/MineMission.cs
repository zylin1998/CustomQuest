using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom;
using Custom.Quest;

namespace QuestDemo
{
    [Serializable]
    public class MineMission : IMission
    {
        [SerializeField]
        private int _QuestSeriesFlag;
        [SerializeField]
        private int _QuestFlag;
        [SerializeField]
        private IMission.EProgress _Progress;

        public int QuestSeriesFlag => this._QuestSeriesFlag;
        public int QuestFlag => this._QuestFlag;
        public bool IsClear => this.Progress.HasFlag(IMission.EProgress.End);
        public IReward Reward => null;
        public IMission.EProgress Progress { get => this._Progress; private set => this._Progress = value; }
        public string Describe => string.Format("Complete {0} - {1}", this._QuestSeriesFlag + 1, this._QuestFlag + 1);

        public IMission Initialize() 
        {
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

            if (args is MineMissionArgs missionArgs) 
            {
                return OnValueChange(this, missionArgs);
            }

            return IMission.EProgress.None; 
        }

        private static IMission.EProgress OnValueChange(MineMission mission, MineMissionArgs args)
        {
            var questSeriesFlag = mission._QuestSeriesFlag == args.Coordinate[0];
            var questFlag = mission._QuestFlag == args.Coordinate[1];
            var complete = IMission.EProgress.Complete;
            var unComplete = IMission.EProgress.UnComplete;

            mission.Progress = questSeriesFlag && questFlag ? complete : unComplete;

            return mission.Progress;
        }
    }

    public class MineMissionArgs : MissionArgs
    {
        public Coordinate Coordinate { get; }

        public MineMissionArgs(Coordinate coordinate) => this.Coordinate = coordinate;
    }

    public class MineMissionInitArgs : MissionInitArgs 
    {
        public IMission.EProgress Progress { get; }

        public MineMissionInitArgs(IMission.EProgress progress) => this.Progress = progress;
    }
}
