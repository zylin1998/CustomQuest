using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Quest;

namespace QuestDemo
{
    [CreateAssetMenu(fileName = "MineQuest", menuName = "Quest Demo", order = 1)]
    public class MineQuest : ScriptableObject, IQuest
    {
        [SerializeField]
        private Timer _Timer;
        [SerializeField]
        private MineRule _Rule; 

        public IRule Rule => this._Rule;
        public IReward Reward => null;

        public IQuest Initialize() 
        {
            this._Rule.Initialize();

            this.ToList().ForEach(f => f.Initialize());

            return this;
        }

        public IQuest Start()
        {
            this._Rule.Start();

            this.ToList().ForEach(f => f.Invoke());

            return this;
        }

        public IEnumerator<IElement> GetEnumerator() => new List<IElement> { this._Timer }.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}