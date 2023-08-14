using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
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

        public void Initialize()
        {
            var sizeX = this._Column;
            var sizeY = this._Row;
            var length = sizeX * sizeY;

            if (length <= 0) { return; }
            
            this.SpaceLeft = length - this._MineCount;

            this._Progress = IRule.EProgress.Start;

            this._Map = new List<EMineMap>(length);
            this._FlagPositions = new List<int>();
        }

        public void Start() 
        {
            this._Progress = IRule.EProgress.Progress;
        }

        public void Reset() 
        {
            this.HasCleared = false;

            this.Initialize();
        }

        public IRule.EProgress CheckRule(object value)
        {
            if (value is MapVariation variation) { return this.CheckRule(variation); }

            return this._Progress;
        }

        public IRule.EProgress CheckRule(MapVariation variation) 
        {
            if (variation.MineMap == EMineMap.Flag ) 
            {
                if (variation.Count > 0) { this._FlagPositions.Add(variation.Position); }

                if (variation.Count < 0) { this._FlagPositions.Remove(variation.Position); }

                this._Progress = this.CheckFlagPosi() ? IRule.EProgress.FulFilled : IRule.EProgress.Progress;
            }

            if (variation.MineMap == EMineMap.Space) 
            {
                this.SpaceLeft -= variation.Count;

                if (this._FlagPositions.Exists(e => e == variation.Position)) 
                {
                    this._FlagPositions.Remove(variation.Position); 
                }

                this._Progress = this.SpaceLeft <= 0 ? IRule.EProgress.FulFilled : IRule.EProgress.Progress;
            }

            if (variation.MineMap == EMineMap.Mine) 
            {
                this._Progress = IRule.EProgress.Failed;
            }

            if (!this.HasCleared)
            {
                this.HasCleared = this._Progress.HasFlag(IRule.EProgress.FulFilled);
            }

            return this._Progress;
        }

        private bool CheckFlagPosi() 
        {
            if (this._FlagPositions.Count > this._MineCount) { return false; }

            if (this._FlagPositions.Count(c => this._Map[c] == EMineMap.Mine) == this._MineCount) { return true; }

            return false;
        }

        public List<EMineMap> CreateMap(int mineCount)
        {
            var length = this._Column * this._Row;

            this._Map.Clear();

            for (var i = 0; i < length; i++) { this._Map.Add(EMineMap.Space); }

            for (var count = mineCount; count > 0;)
            {
                var locate = Mathf.RoundToInt(UnityEngine.Random.Range(0, length - 1));

                if (this._Map[locate] == EMineMap.Space)
                {
                    this._Map[locate] = EMineMap.Mine;

                    count--;
                }
            }

            return this._Map;
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
    public struct MapVariation
    {
        public int Count { get; }
        public EMineMap MineMap { get; }
        public int Position { get; }

        public MapVariation(int count, int position, EMineMap mineMap) 
            => (this.Count, this.Position, this.MineMap) = (count, position, mineMap);
    }
}
