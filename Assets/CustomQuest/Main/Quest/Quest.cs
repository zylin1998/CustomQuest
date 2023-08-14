using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Quest 
{
    public abstract class Quest : ScriptableObject, IQuest 
    {
        [SerializeField]
        protected List<Element> _Elements;
        [SerializeField]
        protected Rule _Rule;
        [SerializeField]
        protected Reward _Reward;

        public IRule Rule => this._Rule;
        public IReward Reward => this._Reward;
        public bool IsClear => this.Rule.Progress.HasFlag(IRule.EProgress.FulFilled);

        public abstract IQuest Initialize();
        public abstract IQuest Start();
        public abstract IQuest End();
        public abstract IQuest Reset();

        public IEnumerator<IElement> GetEnumerator() => this._Elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public interface IQuest : IEnumerable<IElement>, IClear
    {
        public IReward Reward { get; }
        public IRule Rule { get; }

        public IQuest Initialize();
        public IQuest Start();
        public IQuest End();
        public IQuest Reset();

        public TElement GetElement<TElement>() where TElement : IElement => this.OfType<TElement>().First();
    }
}
