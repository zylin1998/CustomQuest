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
        private MineQuest _DemoQuest;
        [SerializeField]
        private ImageDetail _ImageDetail;
        [SerializeField]
        private TextMeshProUGUI _MineCount;
        [SerializeField]
        private List<MineButton> _MineButtons;

        private IQuest _Quest;

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

        public static ImageDetail ImageDetail { get; private set; }
        public static QuestDemo Demo { get; private set; }

        private static EMineMap _CheckType;

        private static System.Action<EMineMap> OnTypeChanged = (type) => { };

        public static EMineMap CheckType 
        { 
            get => _CheckType;

            set 
            {
                _CheckType = value;

                OnTypeChanged.Invoke(value);
            }
        }

        public static event System.Action<EMineMap> TypeChangedEvent 
        {
            add => OnTypeChanged += value;

            remove => OnTypeChanged -= value;
        }

        private void Awake()
        {
            Demo = this;

            this._MineButtons = new List<MineButton>();

            this._MineButtons.Clear();
            this._MineButtons.AddRange(this._Content.GetComponentsInChildren<MineButton>());

            ImageDetail = this._ImageDetail;
        }

        private void Start()
        {
            this.SetQuest(this._DemoQuest);

            MineButton.DetectedEvent += this.CheckRule;

            this.GameStart();
        }

        private void OnDestroy()
        {
            Demo = null;
            ImageDetail = default;
            MineButton.DetectedEvent -= this.CheckRule;
        }

        public void SetQuest(IQuest quest) 
        {
            this.Quest = quest;

            this.Quest.Initialize();
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
                    space.gameObject.SetActive(true);
                    
                    this.SetSquare(c, space);

                    space.IsMine = map[c] == EMineMap.Mine;
                }

                else 
                {
                    space.gameObject.SetActive(false);
                }

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

            Debug.Log(result);

            if (result == IRule.EProgress.Failed) 
            {
                this._MineButtons.ForEach(f => f.ShowMine());
            }
        }

        #region UI Map Setting

        private void SetSquare(int index, MineButton mine) 
        {
            var locate = IndexToLocate(index);
            var list = new List<MineButton>();

            for(var y = -1; y <= 1; y++) 
            {
                var locateY = locate.y + y;

                if (locateY <= -1 || locateY >= this.Square.y) { continue; }

                for(var x = -1; x <= 1; x++) 
                {
                    var locateX = locate.x + x;

                    if (locateX <= -1 || locateX >= this.Square.x) { continue; }

                    var l = new Vector2Int(locateX, locateY);

                    if (l == locate) { continue; }
                    
                    list.Add(this[l]);
                }
            }

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