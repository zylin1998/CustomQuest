using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom;
using Custom.Quest;

namespace QuestDemo
{
    public abstract class MissionEnumeration : Enumeration<MissionEnumeration>
    {
        public static readonly MissionEnumeration Coordinate = new CoordinateMission();
        public static readonly MissionEnumeration MineCount = new MineCountMission();

        protected MissionEnumeration(int value, string name) : base(value, name) { }

        #region

        private event Action<MissionArgs> MissionEvent = (args) => { };

        public void Register(Action<MissionArgs> register) => this.MissionEvent += register;
        public void Cancel(Action<MissionArgs> cancel) => this.MissionEvent -= cancel;
        public void Invoke(MissionArgs args) => this.MissionEvent?.Invoke(args);

        #endregion

        public sealed class CoordinateMission : MissionEnumeration
        {
            public CoordinateMission() : base(0, "Coordinate") { }
        }

        public sealed class MineCountMission : MissionEnumeration
        {
            public MineCountMission() : base(1, "MineCount") { }
        }
    }
}