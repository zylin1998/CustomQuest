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

        public abstract void Start();
        public abstract void Initialize();
        public abstract void Reset();
        public abstract IRule.EProgress CheckRule(QuestArgs args);
    }

    public interface IRule 
    {
        public EProgress Progress { get; }
        
        public void Initialize();
        public void Start();
        public void Reset();
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

    public class QuestArgs : EventArgs 
    {

    }
}
