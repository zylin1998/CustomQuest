using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        public int QuestSeriesFlag => this._QuestSeriesFlag;
        public int QuestFlag => this._QuestFlag;
        public bool IsComplete => this.Progress.HasFlag(IMission.EProgress.Complete);
        public bool IsClear => this.Progress.HasFlag(IMission.EProgress.End);
        public IReward Reward => null;
        public IMission.EProgress Progress { get; private set; }

        public IMission Initialize() 
        {
            this.Progress = IMission.EProgress.Start;

            return this;
        }

        public IMission Start()
        {
            this.Progress = IMission.EProgress.Progress;

            return this;
        }

        public IMission End()
        {
            this.Progress = IMission.EProgress.End;

            return this;
        }

        public IMission.EProgress OnValueChange(QuestArgs args) 
        { 
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

            if (questSeriesFlag && questFlag) { mission.Progress = IMission.EProgress.Complete; }

            else { mission.Progress = IMission.EProgress.Progress; }

            return mission.Progress;
        }
    }

    public class MineMissionArgs : QuestArgs
    {
        public Coordinate Coordinate { get; }

        public MineMissionArgs(Coordinate coordinate) => this.Coordinate = coordinate;
    }
}
