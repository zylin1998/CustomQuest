using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom;
using Custom.Quest;

namespace QuestDemo
{
    [CreateAssetMenu(fileName = "MineQuest", menuName = "Quest Demo/Quest/Quest", order = 1)]
    public class MineQuest 
        : ScriptableObject, IQuest, IProvider<ResultInfo>, IProvider<MapInfo>, IReciever
    {
        [SerializeField]
        private MineRule _Rule;

        public IRule Rule => this._Rule;
        public IReward Reward => null;
        public bool IsClear => this.Rule.Progress == IRule.EProgress.FulFilled;

        public IQuest Start()
        {
            this._Rule.Start();

            return this;
        }

        public IQuest End() => this;

        public void SetInfo(object info) 
        {
            if (info is MineQuestInfo.Info questInfo) { this._Rule.QuestInfo = questInfo; }

            if (info is DetectedInfo detected) { this._Rule.CheckRule(detected); }
        }
        
        public TInfo GetInfo<TInfo>() => this is IProvider<TInfo> provider ? provider.GetInfo() : default;

        object IProvider.GetInfo() => new ResultInfo(this._Rule);
        
        MapInfo IProvider<MapInfo>.GetInfo() => new MapInfo(this._Rule);
        ResultInfo IProvider<ResultInfo>.GetInfo() => new ResultInfo(this._Rule);
    }
}