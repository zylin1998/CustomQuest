using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Custom.Quest;

namespace QuestDemo
{
    public class QuestDemoUI : MonoBehaviour
    {
        [SerializeField]
        private MineArea _MineArea;
        [SerializeField]
        private ResultMessage _ResultMessage;
        [SerializeField]
        private TextMeshProUGUI _MineCount;
        [SerializeField]
        private TextMeshProUGUI _QuestDetail;
        [SerializeField]
        private ImageDetail _ImageDetail;

        public static ImageDetail ImageDetail { get; private set; }

        private void Awake()
        {
            ImageDetail = this._ImageDetail;
        }

        private void OnDestroy()
        {
            ImageDetail = default;
        }

        public void SetMineMap(MineMapArgs args) 
        {
            this._MineArea.Square = args.Size;
            this._MineArea.SetMine(args.MineMap);
        }

        public void SetQuestDetail(QuestDetailArgs args) 
        {
            this._QuestDetail.SetText(string.Format("Quest: {0}", args.Coordinate.CoordinateID));
        }

        public void SetResult(RuleResultArgs args)
        {
            this._MineCount.SetText(string.Format("{0}", args.FakeMineCount));

            if (args.Quest.IsFailed) { this._MineArea.ShowMine(); }

            if (args.Quest.IsClear || args.Quest.IsFailed)
            {
                args.Quest.End();

                this._ResultMessage["Previous"].Interactable = !args.IsFirst;
                this._ResultMessage["Next"].Interactable = !args.IsLast && args.Quest.HasCleared;
            }
        }
    }

    [System.Serializable]
    public struct ImageDetail
    {
        [SerializeField]
        private Sprite _Ground;
        [SerializeField]
        private Sprite _Mine;
        [SerializeField]
        private Sprite _Flag;

        public Sprite Ground => this._Ground;
        public Sprite Mine => this._Mine;
        public Sprite Flag => this._Flag;

        public static Color Normal => Color.white;
        public static Color PointerDown => new Color(0.7f, 0.7f, 0.7f, 1);
        public static Color Clear => Color.clear;

        public static void SetImage(Image image, Sprite sprite, Color color)
        {
            image.sprite = sprite;
            image.color = color;
        }

        public static void SetImage(Image image, Color color)
        {
            image.color = color;
        }
    }

    public class QuestDemoArgs : System.EventArgs 
    {
        
    }

    public class QuestDetailArgs : QuestDemoArgs 
    {
        public Coordinate Coordinate { get; }

        public QuestDetailArgs(Coordinate coordinate) => this.Coordinate = coordinate;
    }

    public class MineMapArgs : QuestDemoArgs
    {
        public Vector2Int Size { get; }
        public List<EMineMap> MineMap { get; }

        public MineMapArgs(Vector2Int size, IEnumerable<EMineMap> map) 
            => (this.Size, this.MineMap) = (size, map.ToList());
    }

    public class RuleResultArgs 
    {
        public int FakeMineCount { get; }
        public IRule.EProgress Progress { get; }
        public MineQuest Quest { get; }
        public bool IsFirst { get; }
        public bool IsLast { get; }

        public RuleResultArgs(int fakeMineCount, IRule.EProgress progress, MineQuest quest, bool isFirst, bool isLast)
            => (this.FakeMineCount, this.Progress, this.Quest, this.IsFirst, this.IsLast) 
             = (fakeMineCount, progress, quest, isFirst, isLast);
    }
}