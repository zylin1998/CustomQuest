using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Custom.Quest
{
    public interface IMission : IInitialize<IMission>, IClear
    {
        public EProgress Progress { get; }
        public IReward Reward { get; }
        public string Describe { get; }

        public IMission End();

        public EProgress OnValueChange(MissionArgs args);

        [Serializable]
        public enum EProgress
        {
            None,
            UnComplete,
            Complete,
            End
        }
    }

    public class MissionArgs : QuestArgs { }

    public class MissionInitArgs : InitArgs { }
}
