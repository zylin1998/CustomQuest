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

        public Vector2Int Size => new Vector2Int(this._Column, this._Row);
        public int MineCount => this._MineCount;
        public int SpaceLeft { get; private set; }
        public int FakeMineCount => this._MineCount - this._FlagPositions.Count;
        public IRule.EProgress Progress => this._Progress;

        public bool HasCleared { get; private set; }

        public IRule Initialize() => this.Initialize(new MapInitArgs(this._Column, this._Row, this._MineCount, this.HasCleared));
        public IRule Initialize(InitArgs args) => args is MapInitArgs init ? RuleInit(this, init) : this;

        public IRule Start() 
        {
            this._Progress = IRule.EProgress.Progress;
            
            return this;
        }

        public IRule Reset() 
        {
            this.HasCleared = false;

            return this.Initialize();
        }

        public IRule.EProgress CheckRule(QuestArgs args)
        {
            if (args is MapArgs mapArgs) { return CheckRule(this, mapArgs); }

            return this._Progress;
        }

        public static IRule.EProgress CheckRule(MineRule rule, MapArgs args) 
        {
            if (args.MineMap == EMineMap.Flag ) { CheckFlag(rule, args); }

            if (args.MineMap == EMineMap.Space && args.Count > 0)  { CheckSpace(rule, args); }

            if (args.MineMap == EMineMap.Mine) { rule._Progress = IRule.EProgress.Failed; }

            if (!rule.HasCleared) { rule.HasCleared = rule._Progress.HasFlag(IRule.EProgress.FulFilled); }

            return rule._Progress;
        }

        public static IRule RuleInit(MineRule rule, MapInitArgs args) 
        {
            rule._Column = args.Column;
            rule._Row = args.Row;
            rule._MineCount = args.MineCount;

            var sizeX = rule._Column;
            var sizeY = rule._Row;
            var length = sizeX * sizeY;

            if (length <= 0) { return rule; }

            rule.SpaceLeft = length - rule._MineCount;

            rule._Progress = IRule.EProgress.Start;

            rule._Map = new List<EMineMap>(length);
            rule._FlagPositions = new List<int>();

            return rule;
        }

        private static void CheckFlag(MineRule rule, MapArgs args) 
        {
            if (args.Count > 0) { rule._FlagPositions.Add(args.Position); }

            if (args.Count < 0) { rule._FlagPositions.Remove(args.Position); }
            
            rule._Progress = CheckFlagPosi(rule) ? IRule.EProgress.FulFilled : IRule.EProgress.Progress;
        }

        private static void CheckSpace(MineRule rule, MapArgs args) 
        {
            rule.SpaceLeft -= args.Count;

            if (rule._FlagPositions.Exists(e => e == args.Position))
            {
                rule._FlagPositions.Remove(args.Position);
            }

            rule._Progress = rule.SpaceLeft <= 0 ? IRule.EProgress.FulFilled : IRule.EProgress.Progress;
        }

        private static bool CheckFlagPosi(MineRule rule) 
        {
            if (rule._FlagPositions.Count > rule._MineCount) { return false; }
            
            if (rule._FlagPositions.Count(c => rule._Map[c] == EMineMap.Mine) == rule._MineCount) { return true; }

            return false;
        }

        public static List<EMineMap> CreateMap(MineRule rule)
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
    }
    
    [Serializable]
    public enum EMineMap
    {
        None,
        Space,
        Mine,
        Flag
    }

    [Serializable]
    public class MapArgs : QuestArgs
    {
        public int Count { get; }
        public EMineMap MineMap { get; }
        public int Position { get; }

        public MapArgs(int count, int position, EMineMap mineMap) 
            => (this.Count, this.Position, this.MineMap) = (count, position, mineMap);
    }

    [Serializable]
    public class MapInitArgs : InitArgs 
    {
        public int Column { get; }
        public int Row { get; }
        public int MineCount { get; }
        public bool HasCleared { get; }

        public MapInitArgs(int column, int row, int mineCount, bool hasCleared)
            => (this.Column, this.Row, this.MineCount, this.HasCleared) = (column, row, mineCount, hasCleared);
    }
}
