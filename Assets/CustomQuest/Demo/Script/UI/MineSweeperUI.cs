using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Custom;
using Custom.Quest;

namespace QuestDemo
{
    public class MineSweeperUI : MonoBehaviour
    {
        [SerializeField]
        private MineArea _MineArea;
        [SerializeField]
        private ResultMessage _ResultMessage;
        [SerializeField]
        private MissionPanel _MissionPanel;
        [SerializeField]
        private TextMeshProUGUI _MineCount;
        [SerializeField]
        private TextMeshProUGUI _QuestDetail;
        [SerializeField]
        private IMine.ImageDetail _MineImageDetail;
        [SerializeField]
        private MissionImageDetail _MissionImageDetail;
        
        private void Awake()
        {
            IMine.ImageDetail.Detail = this._MineImageDetail;
            MissionImageDetail.Detail = this._MissionImageDetail;

            MineSweeper.DetectedEvent += this.SetResult;
            MineSweeper.QuestChangeEvent += this.SetQuest;
        }

        public void SetMineMap(MapInfo map) 
        {
            this._MineArea.SetMine(map);
        }

        public void SetCoordinate(CoordinateInfo info) 
        {
            var coordinate = string.Format("Quest: {0}", string.Join("-", info.Coordinate.Position.GetRange(1, 2).Select(p => ++p)));

            this._QuestDetail.SetText(coordinate);
        }

        public void SetMineCount(MapInfo info) 
        {
            this._MineCount.SetText(string.Format("{0}", info.FakeMineCount));
        }

        public void SetMineCount(ResultInfo info)
        {
            this._MineCount.SetText(string.Format("{0}", info.FakeMineCount));
        }

        public void SetQuest(CoordinateInfo coordinate, MapInfo map) 
        {
            this.SetMineCount(map);

            this.SetMineMap(map);

            this.SetCoordinate(coordinate);
        }

        public void SetResult(CoordinateInfo coordinate, ResultInfo result)
        {
            this.SetMineCount(result);

            var isFailed = result.Progress == IRule.EProgress.Failed;
            var isFulfilled = result.Progress == IRule.EProgress.FulFilled;
            
            if (isFailed) { this._MineArea.ShowMine(); }

            if (isFailed || isFulfilled)
            {
                this._ResultMessage.ShowMessage(result, coordinate);

                if (isFulfilled) 
                {
                    this._MissionPanel.SetMission(DataFlow.Current.GetMissions(coordinate, result));
                }
            }
        }
    }
}