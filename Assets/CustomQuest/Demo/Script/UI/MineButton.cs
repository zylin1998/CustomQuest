using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

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
        
        public int Position { get; set; }
        public bool IsMine { get; set; }
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

        private int DetectedMine() 
        {
            if (this.IsDetected) { return 0;  }

            this.IsDetected = true;

            if (!this.IsMine) 
            {
                this.SetImage(null, ImageDetail.Clear);

                var mineCount = this._Square.Count(c => c.IsMine);
                var hasMine = mineCount > 0;
                
                this._MineNumber.SetText(string.Format("{0}", hasMine ? mineCount : ""));

                return 1 + (hasMine ? 0 : this._Square.Sum(f => f.DetectedMine()));
            }

            this.SetImage(QuestDemo.ImageDetail.Mine, ImageDetail.Normal);

            return -1;
        }

        private int SetFlag() 
        {
            if (!IsDetected)
            {
                var sprite = this.IsFlag ? QuestDemo.ImageDetail.Ground : QuestDemo.ImageDetail.Flag;

                this.SetImage(sprite, ImageDetail.Normal);

                var isFlag = this.IsFlag ? -1 : 1;

                this.IsFlag = !this.IsFlag;

                return isFlag;
            }

            return 0;
        }


        public void ShowMine() 
        {
            var sprite = this.IsMine ? QuestDemo.ImageDetail.Mine : null;
            var color = this.IsMine ? ImageDetail.Normal : ImageDetail.Clear;

            if (!IsMine)
            {
                var mineCount = this._Square.Count(c => c.IsMine);

                this._MineNumber.SetText(string.Format("{0}", mineCount > 0 ? mineCount : ""));
            }

            this.SetImage(sprite, color);
        }

        public void Reset()
        {
            this.IsMine = false;
            this.IsFlag = false;
            this.IsDetected = false;
            this.Position = 0;

            this._MineNumber.SetText("");

            this.SetImage(QuestDemo.ImageDetail.Ground, ImageDetail.Normal);

            this.gameObject.SetActive(true);
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
                var setFlag = this.SetFlag();

                OnDetected.Invoke(new MapVariation(setFlag, this.Position, EMineMap.Flag));
            }

            if (QuestDemo.CheckType == EMineMap.Space) 
            {
                var detected = this.DetectedMine();

                var mineMap = detected >= 0 ? EMineMap.Space : EMineMap.Mine;

                OnDetected.Invoke(new MapVariation(detected, this.Position, mineMap));
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