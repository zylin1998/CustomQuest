using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuestDemo
{
    public class MineArea : MonoBehaviour, IEnumerable<MineButton>
    {
        [SerializeField]
        private Transform _Content;
        [SerializeField]
        private GridLayoutGroup _GridLayoutGroup;
        [SerializeField]
        private List<MineButton> _MineButtons;

        private Vector2Int _Square;

        public Vector2Int Square
        {
            get => this._Square;

            set
            {
                this._Square = value;

                this._GridLayoutGroup.constraintCount = value.x;
            }
        }

        public MineButton this[int x, int y] => this._MineButtons[LocateToIndex(new Vector2Int(x, y))];
        public MineButton this[Vector2Int locate] => this._MineButtons[LocateToIndex(locate)];

        private void Awake()
        {
            this._MineButtons = new List<MineButton>();

            this._MineButtons.Clear();
            this._MineButtons.AddRange(this._Content.GetComponentsInChildren<MineButton>());
        }

        private void Start()
        {
            
        }

        #region UI Map Setting

        public void SetMine(IEnumerable<EMineMap> mineMap) 
        {
            var map = mineMap.ToList();

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
        }

        public void ShowMine() 
        {
            this._MineButtons.ForEach(f => f.ShowMine());
        }

        public void SetSquare(int index, MineButton mine)
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

        public IEnumerator<MineButton> GetEnumerator() => this._MineButtons.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}