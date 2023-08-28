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
        public override IMissionSeries Initialize()
        {
            MissionEnumeration.Coordinate.Register(this.CheckMission);

            return base.Initialize();
        }

        public override IMissionSeries Initialize(InitArgs args)
        {
            MissionEnumeration.Coordinate.Register(this.CheckMission);

            return base.Initialize(args);
        }
    }
}
