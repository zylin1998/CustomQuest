using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestDemo
{
    public class MineSweeper : MonoBehaviour
    {
        private void Start()
        {
            RestartQuest();
        }

        #region Static Event

        private static EMineMap _DetectedType;

        public static EMineMap DetectedType 
        {
            get => _DetectedType;
            
            set 
            {
                _DetectedType = value;

                OnTypeChanged?.Invoke(_DetectedType);
            } 
        }

        private static Action<EMineMap> OnTypeChanged = (map) => { };

        public static event Action<EMineMap> DetectedTypeChanged 
        {
            add => OnTypeChanged += value;
            
            remove => OnTypeChanged -= value;
        }

        private static Action<CoordinateInfo, MapInfo> OnQuestChange = (map, coordinate) => { };

        public static event Action<CoordinateInfo, MapInfo> QuestChangeEvent 
        {
            add => OnQuestChange += value;

            remove => OnQuestChange -= value;
        }

        private static Action<CoordinateInfo, ResultInfo> OnDetected = (coordinate, result) => { };

        public static event Action<CoordinateInfo, ResultInfo> DetectedEvent
        {
            add => OnDetected += value;

            remove => OnDetected -= value;
        }

        #endregion

        #region Quest Manage

        private static int _QuestFlag = 0;

        public static void NextQuest() 
        {
            var info = DataFlow.Current.GetQuest(++_QuestFlag);
            
            StartQuest(info.map, info.Coordinate);
        }

        public static void RestartQuest()
        {
            var info = DataFlow.Current.GetQuest(_QuestFlag);

            StartQuest(info.map, info.Coordinate);
        }

        public static void PreviousQuest()
        {
            var info = DataFlow.Current.GetQuest(--_QuestFlag);

            StartQuest(info.map, info.Coordinate);
        }

        private static void StartQuest(MapInfo map, CoordinateInfo coordinate) 
        {
            if (map is null) { return; }
            if (coordinate is null) { return; }

            DetectedType = EMineMap.Space;

            OnQuestChange?.Invoke(coordinate, map);
        }

        #endregion

        #region State Setting

        public static void SetSquare(IMine mine, IEnumerable<IMine> others)
        {
            mine.Square.Clear();
            mine.Square.AddRange(others);
        }

        public static void MineButtonEvent(IMine mine, EMineMap checkType)
        {
            var detectedInfo = default(DetectedInfo);
            var coordinate = default(CoordinateInfo);
            var result = default(ResultInfo);
            
            if (checkType == EMineMap.Flag)
            {
                var setFlag = SetFlag(mine);

                detectedInfo = new DetectedInfo(setFlag, mine.Position, EMineMap.Flag);
            }

            if (checkType == EMineMap.Space)
            {
                var detected = Detected(mine);

                var mineMap = detected >= 0 ? EMineMap.Space : EMineMap.Mine;

                detectedInfo = new DetectedInfo(detected, mine.Position, mineMap);
            }
            
            (coordinate, result) = DataFlow.Current.GetResult(detectedInfo);
            
            OnDetected?.Invoke(coordinate, result);
        }
        
        #region Interact Movement

        private static int Detected(IMine mine)
        {
            if (mine.IsDetected || mine.IsFlag) { return 0; }

            mine.IsDetected = true;

            if (!mine.IsMine)
            {
                var mineCount = mine.Square.Count(c => c.IsMine);
                var hasMine = mineCount > 0;

                mine.SetInfo(new IMine.DetailInfo(EMineMap.Space, mineCount));

                return 1 + (hasMine ? 0 : mine.Square.Sum(f => Detected(f)));
            }

            mine.SetInfo(new IMine.DetailInfo(EMineMap.Mine, 0));

            return -1;
        }

        private static int SetFlag(IMine mine)
        {
            if (!mine.IsDetected)
            {
                var isFlag = mine.IsFlag ? -1 : 1;

                mine.IsFlag = !mine.IsFlag;

                mine.SetInfo(new IMine.DetailInfo(EMineMap.Flag, 0));

                return isFlag;
            }

            return 0;
        }

        #endregion

        #endregion
    }
}