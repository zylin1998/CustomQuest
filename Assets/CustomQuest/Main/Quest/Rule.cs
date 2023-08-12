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
        public abstract IRule.EProgress CheckRule(object value);
    }

    public interface IRule 
    {
        public EProgress Progress { get; }

        public void Initialize();
        public void Start();
        public EProgress CheckRule(object value);

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
