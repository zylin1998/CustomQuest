using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Quest
{
    public abstract class Reward : ScriptableObject, IReward
    {
        public abstract void GetReward();
    }

    public interface IReward
    {
        public void GetReward();
    }
}