using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Quest
{
    public abstract class Quest : ScriptableObject, IQuest
    {
        [SerializeField]
        protected Rule _Rule;
        [SerializeField]
        protected Reward _Reward;

        public IRule Rule => this._Rule;
        public IReward Reward => this._Reward;
        public bool IsClear => this.Rule.Progress.HasFlag(IRule.EProgress.FulFilled);
        
        public abstract IQuest Start();
        public abstract IQuest End();
        public abstract void SetInfo(object info);
        public abstract object GetInfo();
    }

    public interface IQuest : IReciever, IProvider, IClear
    {
        public IReward Reward { get; }
        public IRule Rule { get; }
        
        public IQuest Start();
        public IQuest End();
    }
}
