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

        private static System.Action<MapArgs> OnDetected = (var) => { };

        public static event System.Action<MapArgs> DetectedEvent 
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

            ImageDetail.SetImage(this._AreaImage, QuestDemoUI.ImageDetail.Ground, ImageDetail.Normal);
        }

        #region Interact Movement

        private static int Detected(MineButton mine) 
        {
            if (mine.IsDetected || mine.IsFlag) { return 0; }

            mine.IsDetected = true;

            if (!mine.IsMine)
            {
                ImageDetail.SetImage(mine._AreaImage, null, ImageDetail.Clear);

                var mineCount = mine._Square.Count(c => c.IsMine);
                var hasMine = mineCount > 0;

                mine._MineNumber.SetText(string.Format("{0}", hasMine ? mineCount : ""));

                return 1 + (hasMine ? 0 : mine._Square.Sum(f => Detected(f)));
            }

            ImageDetail.SetImage(mine._AreaImage, QuestDemoUI.ImageDetail.Mine, ImageDetail.Normal);

            return -1;
        }

        private static int SetFlag(MineButton mine)
        {
            if (!mine.IsDetected)
            {
                var sprite = mine.IsFlag ? QuestDemoUI.ImageDetail.Ground : QuestDemoUI.ImageDetail.Flag;

                ImageDetail.SetImage(mine._AreaImage, sprite, ImageDetail.Normal);

                var isFlag = mine.IsFlag ? -1 : 1;

                mine.IsFlag = !mine.IsFlag;

                return isFlag;
            }

            return 0;
        }

        #endregion

        #region State Setting

        public static void SetSquare(MineButton mine, IEnumerable<MineButton> mineButtons) 
        {
            mine._Square.Clear();
            mine._Square.AddRange(mineButtons);
        }

        public static void ShowMine(MineButton mine) 
        {
            var sprite = mine.IsMine ? QuestDemoUI.ImageDetail.Mine : null;
            var color = mine.IsMine ? ImageDetail.Normal : ImageDetail.Clear;

            if (!mine.IsMine)
            {
                var mineCount = mine._Square.Count(c => c.IsMine);

                mine._MineNumber.SetText(string.Format("{0}", mineCount > 0 ? mineCount : ""));
            }

            ImageDetail.SetImage(mine._AreaImage, sprite, color);
        }

        public static void Reset(MineButton mine)
        {
            mine.IsMine = false;
            mine.IsFlag = false;
            mine.IsDetected = false;
            mine.Position = 0;

            mine._MineNumber.SetText("");

            ImageDetail.SetImage(mine._AreaImage, QuestDemoUI.ImageDetail.Ground, ImageDetail.Normal);

            mine.gameObject.SetActive(true);
        }

        private static EMineMap CheckType(PointerEventData.InputButton button) 
        {
            if (button == PointerEventData.InputButton.Left) { return QuestDemo.CheckType; }
            if (button == PointerEventData.InputButton.Right) { return EMineMap.Flag; }

            return EMineMap.None;
        }

        private static void MineButtonEvent(MineButton mine, EMineMap checkType) 
        {
            if (checkType == EMineMap.Flag)
            {
                var setFlag = SetFlag(mine);

                OnDetected.Invoke(new MapArgs(setFlag, mine.Position, EMineMap.Flag));
            }

            if (checkType == EMineMap.Space)
            {
                var detected = Detected(mine);

                var mineMap = detected >= 0 ? EMineMap.Space : EMineMap.Mine;

                OnDetected.Invoke(new MapArgs(detected, mine.Position, mineMap));
            }
        }

        #endregion

        #region Pointer Events

        public void OnPointerDown(PointerEventData eventData) 
        {
            if (this.IsDetected) { return; }

            this._AreaImage.color = ImageDetail.PointerDown;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (IsDetected) { this._AreaImage.color = ImageDetail.Clear; }

            else { this._AreaImage.color = ImageDetail.Normal; }
        }

        public void OnPointerClick(PointerEventData eventData) 
        {
            var checkType = CheckType(eventData.button);

            MineButtonEvent(this, checkType);
        }

        #endregion
    }
}