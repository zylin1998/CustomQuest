using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom;
using Custom.Quest;

namespace QuestDemo
{
    public class DataFlow: Singleton<DataFlow>
    {
        [SerializeField]
        private MineQuest _Quest;
        [SerializeField]
        private SeriesRoot _QuestRoot;
        [SerializeField]
        private SeriesRoot _MissionRoot;

        #region Script Behaviour

        private void Awake()
        {
            this.QuestFlag = 0;

            this.QuestList = this._QuestRoot.Flatten<MineQuestInfo.Info>();
            this.MissionList = this._MissionRoot.
                Flatten(1, c => c.Item.Convert(c 
                    => c.IsType<IMissionSeries>().GetEnumerator().IsType<IMissionSeries.Enumerator>()
                ));
        }

        #endregion

        public int QuestFlag { get; private set; }
        public List<CoordinateItem<MineQuestInfo.Info>> QuestList { get; private set; }
        public List<IMissionSeries.Enumerator> MissionList { get; private set; }
        
        private CoordinateItem _CoordinateItem;
        
        public CoordinateItem CoordinateItem
        {
            get => this._CoordinateItem;

            private set 
            {
                if (value.Item is MineQuestInfo.Info info) 
                {
                    this._CoordinateItem = value;

                    this._Quest.SetInfo(info);
                    this._Quest.Start();
                }
            }
        }

        private CoordinateInfo CoordinateInfo 
            => new CoordinateInfo(
                this.CoordinateItem.Coordinate, 
                this.QuestFlag == 0, 
                this.QuestFlag == this.QuestList.Count - 1);

        public (MapInfo map, CoordinateInfo Coordinate) GetQuest(int index)
        {
            if (index < 0) { return (default, default); }
            if (index >= this.QuestList.Count) { return (default, default); }

            this.QuestFlag = index;

            this.CoordinateItem = this.QuestList[index];

            return (this._Quest.GetInfo<MapInfo>(), this.CoordinateInfo);
        }
        
        public (CoordinateInfo Coordinate, ResultInfo result) GetResult(DetectedInfo detected)
        {
            var coordinate = this.CoordinateInfo;
            var result = this._Quest.Rule.CheckRule(detected) as ResultInfo;

            return (coordinate, result);
        }

        public MissionInfoPack GetMissions()
        {
            return new MissionInfoPack(this.MissionList);
        }

        public MissionInfoPack GetMissions(CoordinateInfo coordinate, ResultInfo result)
        {
            this.MissionList.ForEach(list =>
            {
                list.SetInfo(coordinate);
                list.SetInfo(result);
            });

            return new MissionInfoPack(this.MissionList);
        }
    }

    public class CoordinateInfo 
    {
        public bool IsFront { get; }
        public bool IsBack { get; }
        public Coordinate Coordinate { get; }

        public CoordinateInfo(Coordinate coordinate, bool isFront, bool isBack)
            => (this.Coordinate, this.IsFront, this.IsBack) = (coordinate, isFront, isBack);
    }

    public class MissionInfoPack
    {
        public List<IMissionInfo.Info> Infos { get; }

        public MissionInfoPack(IEnumerable<IMissionSeries.Enumerator> enumerators) 
        {
            var complete = new List<IMissionInfo.Info>();
            var unComplete = new List<IMissionInfo.Info>();
            var end = new List<IMissionInfo.Info>();

            enumerators.ToList().ForEach(enumerator =>
            {
                complete.AddRange(enumerator.Complete);
                unComplete.AddRange(enumerator.UnComplete);
                end.AddRange(enumerator.End);
            });

            this.Infos = complete.Concat(unComplete).Concat(end).ToList();
        }
    }

    public class Singleton<T> : MonoBehaviour where T : Singleton<T> 
    {
        protected static T _Current;

        public static T Current 
        {
            get 
            {
                if (_Current.IsNull()) 
                {
                    var t = FindObjectOfType<T>();

                    _Current = !t.IsNull() ? t : new GameObject(typeof(T).Name).AddComponent<T>();
                }

                return _Current;
            }
        }

        protected virtual void OnDestroy() 
        {
            if (!_Current.IsNull()) { _Current = null; }
        }
    }
}
