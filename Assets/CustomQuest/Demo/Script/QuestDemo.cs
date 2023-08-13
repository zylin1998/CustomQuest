using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Custom.Quest;
using TMPro;

namespace QuestDemo
{
    public class QuestDemo : MonoBehaviour
    {
        [SerializeField]
        private Transform _Content;
        [SerializeField]
        private GridLayoutGroup _GridLayoutGroup;
        [SerializeField]
        private MineQuestSeries _DemoQuestSeries;
        [SerializeField]
        private ResultMessage _ResultMessage;
        [SerializeField]
        private TextMeshProUGUI _MineCount;
        [SerializeField]
        private ImageDetail _ImageDetail;
        [SerializeField]
        private List<MineButton> _MineButtons;

        private IQuest _Quest;
        
        #region Properties

        public IQuest Quest 
        {
            get => this._Quest;

            private set 
            {
                this._Quest = value;

                if (value.Rule is MineRule mineRule) 
                {
                    this.Rule = mineRule;
                    
                    this.Square = mineRule.Size;
                }
            }
        }

        public MineRule Rule { get; private set; }

        private Vector2Int _Square;

        public Vector2Int Square 
        { 
            get => this._Square;

            private set 
            {
                this._Square = value;

                this._GridLayoutGroup.constraintCount = value.x;
            } 
        }

        public MineButton this[int x, int y] => this._MineButtons[LocateToIndex(new Vector2Int(x, y))];
        public MineButton this[Vector2Int locate] => this._MineButtons[LocateToIndex(locate)];

        #endregion

        #region Static Properties

        public static ImageDetail ImageDetail { get; private set; }
        public static QuestDemo Demo { get; private set; }

        private static EMineMap _CheckType;

        public static EMineMap CheckType 
        { 
            get => _CheckType;

            set 
            {
                _CheckType = value;

                OnTypeChanged.Invoke(value);
            }
        }

        private static System.Action<EMineMap> OnTypeChanged = (type) => { };

        public static event System.Action<EMineMap> TypeChangedEvent 
        {
            add => OnTypeChanged += value;

            remove => OnTypeChanged -= value;
        }

        private static System.Action<IQuest> OnQuestEnd = (quest) => { };

        public static event System.Action<IQuest> QuestEndEvent
        {
            add => OnQuestEnd += value;

            remove => OnQuestEnd -= value;
        }

        #endregion

        private void Awake()
        {
            Demo = this;

            this._MineButtons = new List<MineButton>();

            this._MineButtons.Clear();
            this._MineButtons.AddRange(this._Content.GetComponentsInChildren<MineButton>());

            ImageDetail = this._ImageDetail;
        }

        #region Script Behaviour

        private void Start()
        {
            MineButton.DetectedEvent += this.CheckRule;

            this._ResultMessage["Previous"].ClickEvent += this.PreviousQuest;
            this._ResultMessage["Restart"].ClickEvent += this.RestartQuest;
            this._ResultMessage["Next"].ClickEvent += this.NextQuest;

            this.NextQuest();
        }

        private void OnDestroy()
        {
            Demo = null;
            ImageDetail = default;
            MineButton.DetectedEvent -= this.CheckRule;
        }

        #endregion

        #region Quest Manage

        public void SetQuest(IQuest quest) 
        {
            this.Quest = quest;

            this.Quest.Initialize();
        }

        public void PreviousQuest() => this.StartQuest(this._DemoQuestSeries.QuestGetter.Previous);
        public void NextQuest() => this.StartQuest(this._DemoQuestSeries.QuestGetter.Next);
        public void RestartQuest() => this.StartQuest(this.Quest);

        public void StartQuest(IQuest quest) 
        {
            if (quest != null)
            {
                this.SetQuest(quest);

                this.GameStart();
            }
        }

        public void GameStart() 
        {
            var map = this.Rule.CreateMap(this.Rule.MineCount);

            this.Square = this.Rule.Size;

            this._MineCount.SetText(string.Format("{0}", this.Rule.FakeMineCount));

            var c = 0;
            this._MineButtons.ForEach(space =>
            {
                if (c < map.Count)
                {
                    space.Reset();
                    space.IsMine = map[c] == EMineMap.Mine;
                    
                    this.SetSquare(c, space);
                }
                
                else { space.gameObject.SetActive(false); }

                c++;
            });

            CheckType = EMineMap.Space;

            this.Quest.Start();
        }

        public void CheckRule(MapVariation variation) 
        {
            var mineCount = this.Rule.MineCount;
            var result = this.Rule.CheckRule(variation);

            this._MineCount.SetText(string.Format("{0}", this.Rule.FakeMineCount));

            var failed = result.HasFlag(IRule.EProgress.Failed);
            var fulfilled = result.HasFlag(IRule.EProgress.FulFilled);

            if (failed) { this._MineButtons.ForEach(f => f.ShowMine()); }

            if (fulfilled || failed) 
            {
                this._Quest.End();
                
                var isFirst = this._DemoQuestSeries.QuestGetter.IsFirst;
                var isLast = this._DemoQuestSeries.QuestGetter.IsLast;

                this._ResultMessage["Previous"].Interactable = !isFirst;
                this._ResultMessage["Next"].Interactable = !isLast && fulfilled;

                OnQuestEnd?.Invoke(this.Quest);
            }
        }

        #endregion

        #region UI Map Setting

        private void SetSquare(int index, MineButton mine) 
        {
            var sizeX = this.Square.x;
            var sizeY = this.Square.y;
            var locate = this.IndexToLocate(index);
            var list = new List<MineButton>();

            var locateList = new List<Vector2Int>
            {
                new Vector2Int(-1, -1),
                new Vector2Int( 0, -1),
                new Vector2Int( 1, -1),
                new Vector2Int(-1,  0),
                new Vector2Int( 1,  0),
                new Vector2Int(-1,  1),
                new Vector2Int( 0,  1),
                new Vector2Int( 1,  1),
            };

            locateList.ForEach(l =>
            {
                var target = l + locate;

                if (target.x < 0 || target.y < 0) { return; }
                if (target.x >= sizeX || target.y >= sizeY) { return; }

                list.Add(this[target]);
            });

            mine.Position = index;
            mine.SetSquare(list);
        }

        private int LocateToIndex(Vector2Int vector) 
        {
            var v = Vector2Int.Min(vector, this.Square - Vector2Int.one);

            return v.y * this.Square.x + v.x;
        }

        private Vector2Int IndexToLocate(int index) 
        {
            var x = index % this.Square.x;
            var y = index / this.Square.x;

            return Vector2Int.Min(new Vector2Int(x, y), this.Square - Vector2Int.one); 
        }

        #endregion
    }
}