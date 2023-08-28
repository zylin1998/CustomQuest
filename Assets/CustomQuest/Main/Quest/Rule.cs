using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Quest
{
    public abstract class Rule : ScriptableObject, IRule
    {
        public virtual IRule.EProgress Progress { get; }

        public abstract IRule Start();
        public abstract IRule Initialize();
        public abstract IRule Initialize(InitArgs args);
        public abstract IRule Reset();
        public abstract IRule.EProgress CheckRule(QuestArgs args);
    }

    public interface IRule : IInitialize<IRule>
    {
        public EProgress Progress { get; }
        
        public IRule Start();
        public IRule Reset();
        public EProgress CheckRule(QuestArgs args);

        [Serializable]
        public enum EProgress
        {
            None,
            Start,
            Progress,
            FulFilled,
            Failed
        }
    }
}
