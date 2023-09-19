using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Custom;
using Custom.Quest;
using UnityEngine;

namespace QuestDemo
{
    [Serializable]
    public class MineRule : IRule
    {
        [SerializeField, Range(1, 34)]
        private int _Column;
        [SerializeField, Range(1, 14)]
        private int _Row;
        [SerializeField]
        private int _MineCount;
        [SerializeField]
        private IRule.EProgress _Progress = IRule.EProgress.Start;
        [SerializeField]
        private List<EMineMap> _Map;
        [SerializeField]
        private List<int> _FlagPositions;

        private Timer _Timer;
        
        public IQuestInfo.Info QuestInfo 
        { 
            get => this._QuestInfo;

            set
            {
                if (value is MineQuestInfo.Info info)
                {
                    this._QuestInfo = info;
                    
                    this._Column = this._QuestInfo.Size.x;
                    this._Row = this._QuestInfo.Size.y;
                    this._MineCount = Mathf.Clamp(this._QuestInfo.MineCount, 0, this._Column * this._Row);

                    Init(this);
                }
            }
        }

        private MineQuestInfo.Info _QuestInfo;

        public IRule.EProgress Progress
        {
            get => this._Progress;

            private set 
            {
                this._Progress = value;

                if (this._Progress == IRule.EProgress.FulFilled) 
                {
                    this.QuestInfo.IsClear = true;
                }
            }
        }

        public TimeDisplay PassTime => this._Timer.PassTime;
        public Vector2Int Size => new Vector2Int(this._Column, this._Row);
        public int MineCount => this._MineCount;
        public int FakeMineCount => this._MineCount - this._FlagPositions.Count;
        public List<EMineMap> Map => this._Map;
        public int SpaceLeft { get; private set; }

        public IRule Start() 
        {
            this._Progress = IRule.EProgress.Progress;
            this._Timer = new Timer(0, 10);
            this._Timer.Invoke();
            
            return this;
        }

        public ResultInfo CheckRule(DetectedInfo info)
        {
            return CheckRule(this, info);
        }

        IRule.ProvideInfo IRule.CheckRule(object info) 
        {
            if (info is DetectedInfo detected) { return this.CheckRule(detected); }

            return new IRule.ProvideInfo(this);
        }

        #region Check Rule

        public static ResultInfo CheckRule(MineRule rule, DetectedInfo info) 
        {
            if (info.MineMap == EMineMap.Flag ) { CheckFlag(rule, info); }

            if (info.MineMap == EMineMap.Space && info.Count > 0)  { CheckSpace(rule, info); }

            if (info.MineMap == EMineMap.Mine) { rule.Progress = IRule.EProgress.Failed; }

            if (rule.Progress == IRule.EProgress.FulFilled) { rule._Timer.EndInvoke(); }

            return new ResultInfo(rule);
        }

        private static void CheckFlag(MineRule rule, DetectedInfo info) 
        {
            if (info.Count > 0) { rule._FlagPositions.Add(info.Position); }

            if (info.Count < 0) { rule._FlagPositions.Remove(info.Position); }
            
            rule.Progress = CheckFlagPosi(rule) ? IRule.EProgress.FulFilled : IRule.EProgress.Progress;
        }

        private static void CheckSpace(MineRule rule, DetectedInfo info) 
        {
            rule.SpaceLeft -= info.Count;

            if (rule._FlagPositions.Exists(e => e == info.Position))
            {
                rule._FlagPositions.Remove(info.Position);
            }

            rule.Progress = rule.SpaceLeft <= 0 ? IRule.EProgress.FulFilled : IRule.EProgress.Progress;
        }

        private static bool CheckFlagPosi(MineRule rule) 
        {
            if (rule._FlagPositions.Count > rule._MineCount) { return false; }
            
            if (rule._FlagPositions.Count(c => rule._Map[c] == EMineMap.Mine) == rule._MineCount) { return true; }

            return false;
        }

        #endregion

        #region Init

        public static IRule Init(MineRule rule) 
        {
            var sizeX = rule.Size.x;
            var sizeY = rule.Size.y;
            var length = sizeX * sizeY;

            if (length <= 0) { return rule; }

            rule.SpaceLeft = length - rule._MineCount;

            rule._Progress = IRule.EProgress.Start;

            rule._Map = CreateMap(rule);
            rule._FlagPositions = new List<int>();

            return rule;
        }

        private static List<EMineMap> CreateMap(MineRule rule)
        {
            var length = rule._Column * rule._Row;

            rule._Map.Clear();

            for (var i = 0; i < length; i++) { rule._Map.Add(EMineMap.Space); }

            for (var count = rule.MineCount; count > 0;)
            {
                var locate = Mathf.RoundToInt(UnityEngine.Random.Range(0, length - 1));

                if (rule._Map[locate] == EMineMap.Space)
                {
                    rule._Map[locate] = EMineMap.Mine;

                    count--;
                }
            }
            
            return rule._Map;
        }

        #endregion
    }

    [Serializable]
    public enum EMineMap
    {
        None,
        Space,
        Mine,
        Flag
    }

    public class MapInfo
    {
        public int FakeMineCount { get; }
        public Vector2Int Size { get; }
        public List<EMineMap> Map { get; }

        public MapInfo(MineRule rule) 
            => (this.FakeMineCount, this.Map, this.Size) = (rule.FakeMineCount, rule.Map, rule.Size);
    }

    public class DetectedInfo
    {
        public int Count { get; }
        public EMineMap MineMap { get; }
        public int Position { get; }

        public DetectedInfo(int count, int position, EMineMap mineMap) 
            => (this.Count, this.Position, this.MineMap) = (count, position, mineMap);
    }

    public class ResultInfo : IRule.ProvideInfo
    {
        public TimeDisplay PassTime { get; }
        public bool IsCleared { get; }
        public int MineCount { get; }
        public int FakeMineCount { get; }
        public int MineGatherd => _MineGather;

        private static int _MineGather = 0;

        public ResultInfo(MineRule rule) : base(rule)
        {
            (this.PassTime, this.MineCount, this.FakeMineCount, this.IsCleared)
               = (rule.PassTime, rule.MineCount, rule.FakeMineCount, rule.QuestInfo.IsClear);

            if(this.Progress == IRule.EProgress.FulFilled) 
            {
                _MineGather += this.MineCount;
            }
        }
    }
}
