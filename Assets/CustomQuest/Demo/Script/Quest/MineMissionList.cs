using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Quest;

namespace QuestDemo
{
    [CreateAssetMenu(fileName = "MineMission List", menuName = "Quest Demo/Mission List", order = 1)]
    public class MineMissionList : MissionList<MineMission>
    {
    }
}
