using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Custom.Quest;

namespace QuestDemo
{
    public class MineButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField]
        private Image _AreaImage;
        [SerializeField]
        private TextMeshProUGUI _MineNumber;
        [SerializeField]
        private List<MineButton> _Square;
        
        public bool IsMine { get; set; }
        public int Position { get; set; }
        public bool IsFlag { get; private set; }
        public bool IsDetected { get; private set; }

        private static System.Action<MapVariation> OnDetected = (var) => { };

        public static event System.Action<MapVariation> DetectedEvent 
        {
            add => OnDetected += value;
            
            remove => OnDetected -= value;
        }

        private void Awake()
        {
            this._Square = new List<MineButton>();
        }

        private void Start()
        {
            this._MineNumber.SetText("");

            this.SetImage(QuestDemo.ImageDetail.Ground, ImageDetail.Normal);
        }

        public void SetSquare(IEnumerable<MineButton> mineButtons) 
        {
            this._Square.Clear();
            this._Square.AddRange(mineButtons);
        }

        public int DetectedMine() 
        {
            if (this.IsDetected || this.IsFlag) { return 0;  }

            this.IsDetected = true;

            if (!this.IsMine) 
            {
                this.SetImage(null, ImageDetail.Clear);

                var mineCount = this._Square.Count(c => c.IsMine);

                if (mineCount > 0) 
                { 
                    this._MineNumber.SetText(string.Format("{0}", mineCount));

                    return 1; 
                }

                return 1 + this._Square.Sum(f => f.DetectedMine());
            }

            else 
            {
                this.SetImage(QuestDemo.ImageDetail.Mine, ImageDetail.Normal);

                return -1;
            }
        }

        public void SetFlag() 
        {
            this.SetImage(QuestDemo.ImageDetail.Flag, ImageDetail.Normal);
        }

        public void ShowMine() 
        {
            var sprite = this.IsMine ? QuestDemo.ImageDetail.Mine : null;
            var color = this.IsMine ? ImageDetail.Normal : ImageDetail.Clear;

            if (!IsMine)
            {
                var mineCount = this._Square.Count(c => c.IsMine);

                if (mineCount > 0)
                {
                    this._MineNumber.SetText(string.Format("{0}", mineCount));
                }
            }

            this.SetImage(sprite, color);
        }

        #region Pointer Events

        public void OnPointerDown(PointerEventData eventData) 
        {
            if (this.IsDetected) { return; }

            this.SetImage(QuestDemo.ImageDetail.Ground, ImageDetail.PointerDown);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (IsDetected) { this._AreaImage.color = ImageDetail.Clear; }

            else { this._AreaImage.color = ImageDetail.Normal; }
        }

        public void OnPointerClick(PointerEventData eventData) 
        {
            if (QuestDemo.CheckType == EMineMap.Flag) 
            {
                if (!this.IsFlag && !IsDetected)
                {
                    this.IsFlag = true;

                    this.SetImage(QuestDemo.ImageDetail.Flag, ImageDetail.Normal);

                    OnDetected.Invoke(new MapVariation(1, this.Position, EMineMap.Flag));
                }
            }

            if (QuestDemo.CheckType == EMineMap.Space) 
            {
                this.IsFlag = false;

                var detected = this.DetectedMine();

                if (detected > 0)
                {
                    OnDetected.Invoke(new MapVariation(detected, this.Position, EMineMap.Space));
                }

                if (detected < 0) 
                {
                    OnDetected.Invoke(new MapVariation(detected, this.Position, EMineMap.Mine));
                }
            }
        }

        #endregion

        private void SetImage(Sprite sprite, Color color) 
        {
            this._AreaImage.sprite = sprite;
            this._AreaImage.color = color;
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
        public static Color PointerDown => new Color(0.3f, 0.3f, 0.3f, 1);
        public static Color Clear => Color.clear;
    }
}