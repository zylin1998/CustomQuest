using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Quest
{
    public abstract class Rule : ScriptableObject, IRule
    {
        public abstract IRule.EProgress Progress { get; }
        public abstract IQuestInfo.Info QuestInfo { get; }

        public abstract IRule Start();
        public abstract IRule.ProvideInfo CheckRule(object info);
    }

    public interface IRule
    {
        public EProgress Progress { get; }
        public IQuestInfo.Info QuestInfo { get; }
        
        public IRule Start();
        public ProvideInfo CheckRule(object info);

        [Serializable]
        public enum EProgress
        {
            None,
            Start,
            Progress,
            FulFilled,
            Failed
        }

        public class ProvideInfo
        {
            public EProgress Progress { get; }

            public ProvideInfo(IRule rule) => this.Progress = rule.Progress;
        }
    }
}
