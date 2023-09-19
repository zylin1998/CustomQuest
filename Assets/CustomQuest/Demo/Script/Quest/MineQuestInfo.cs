using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom;
using Custom.Quest;

namespace QuestDemo
{
    [System.Serializable]
    public class MineQuestInfo : IQuestInfo, IProvider<MineQuestInfo.Info>
    {
        [SerializeField, Range(1, 34)]
        public int _Column;
        [SerializeField, Range(1, 14)]
        public int _Row;
        [SerializeField]
        private int _MineCount;

        public Vector2Int Size => new Vector2Int(this._Column, this._Row);
        public int MineCount => this._MineCount;

        public object GetInfo() => new Info(this);
        
        Info IProvider<Info>.GetInfo() => this.GetInfo() as Info;

        public class Info : IQuestInfo.Info 
        {
            public Vector2Int Size { get; }
            public int MineCount { get; }

            public Info(MineQuestInfo info)
                => (this.Size, this.MineCount) = (info.Size, info.MineCount);
        }
    }
}