using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace QuestDemo
{
    public class MineButton : MonoBehaviour, IMine, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField]
        private Image _AreaImage;
        [SerializeField]
        private TextMeshProUGUI _MineNumber;
        
        public int Position { get; set; }
        public bool IsMine { get; set; }
        public bool IsFlag { get; set; }
        public bool IsDetected { get; set; }
        public List<IMine> Square { get; set; }

        private void Awake()
        {
            this.Square = new List<IMine>();
        }

        public void SetInfo(object info) 
        {
            if (info is IMine.ImageInfo image) { Setting(this, image); }

            if (info is IMine.DetailInfo detail) { SetImage(detail, this); }
        }

        #region Pointer Events

        public void OnPointerDown(PointerEventData eventData) 
        {
            if (this.IsDetected) { return; }

            this._AreaImage.color = IMine.ImageDetail.PointerDown;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (this.IsDetected) { return; }

            this._AreaImage.color = IMine.ImageDetail.Normal;
        }

        public void OnPointerClick(PointerEventData eventData) 
        {
            if (this.IsDetected) { return; }

            var checkType = CheckType(eventData.button);

            MineSweeper.MineButtonEvent(this, checkType);
        }

        #endregion
        
        //For PointerClickEvent
        protected static EMineMap CheckType(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left) { return MineSweeper.DetectedType; }
            if (button == PointerEventData.InputButton.Right) { return EMineMap.Flag; }

            return EMineMap.None;
        }
        
        //Show Area Type at failed
        public static void Show(IMine mine)
        {
            var sprite = mine.IsMine ? IMine.ImageDetail.Mine : null;
            var color = mine.IsMine ? IMine.ImageDetail.Normal : IMine.ImageDetail.Clear;
            var count = string.Empty;

            if (!mine.IsMine)
            {
                var mineCount = mine.Square.Count(c => c.IsMine);

                count = string.Format("{0}", mineCount > 0 ? mineCount : "");
            }

            mine.SetInfo(new IMine.ImageInfo(sprite, color, count));
        }

        private static void Setting(MineButton mineButton, IMine.ImageInfo info)
        {
            IMine.ImageDetail.SetImage(mineButton._AreaImage, info.Sprite, info.Color);

            mineButton._MineNumber.SetText(info.Count);
            mineButton.gameObject.SetActive(info.Active);
        }

        protected static void SetImage(IMine.DetailInfo info, MineButton mineButton) 
        {
            var imageInfo = default(IMine.ImageInfo);

            if (info.AreaType == EMineMap.Space)
            {
                var str = string.Format("{0}", info.MineCount > 0 ? info.MineCount : "");

                imageInfo = new IMine.ImageInfo(null, IMine.ImageDetail.Clear, str);
            }

            if (info.AreaType == EMineMap.Flag) 
            {
                var sprite = mineButton.IsFlag ? IMine.ImageDetail.Flag : IMine.ImageDetail.Ground;

                imageInfo = new IMine.ImageInfo(sprite, IMine.ImageDetail.Normal, string.Empty);
            }

            if (info.AreaType == EMineMap.Mine)
            {
                imageInfo = new IMine.ImageInfo(IMine.ImageDetail.Mine, IMine.ImageDetail.Normal, string.Empty);
            }

            Setting(mineButton, imageInfo);
        }

        public static void Reset(IMine mine)
        {
            mine.IsMine = false;
            mine.IsFlag = false;
            mine.IsDetected = false;
            mine.Position = 0;

            var sprite = IMine.ImageDetail.Ground;
            var color = IMine.ImageDetail.Normal;
            var count = string.Empty;
            var active = true;

            mine.SetInfo(new IMine.ImageInfo(sprite, color, count, active));
        }
    }
}