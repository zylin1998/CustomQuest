using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Custom.Quest
{
    public interface IMission : IClear
    {
        public EProgress Progress { get; }
        public IReward Reward { get; }
        public bool IsComplete { get; }

        public IMission Initialize();
        public IMission Start();
        public IMission End();

        public EProgress OnValueChange(QuestArgs args);

        [Serializable]
        public enum EProgress
        {
            None,
            Start,
            Progress,
            Complete,
            End
        }
    }
}
