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
        public bool IsFlag { get; private set; }
        public bool IsDetected { get; private set; }
        public List<IMine> Square { get; private set; }

        bool IMine.IsFlag { get => this.IsFlag; set => this.IsFlag = value; }
        bool IMine.IsDetected { get => this.IsDetected; set => this.IsDetected = value; }
        List<IMine> IMine.Square { get => this.Square; set => this.Square = value; }

        private void Awake()
        {
            this.Square = new List<IMine>();
        }

        private void Start()
        {
            var sprite = QuestDemoUI.ImageDetail.Ground;
            var color = ImageDetail.Normal;
            var count = string.Empty;

            this.SetUsersInterface(new IMine.SettingArgs(sprite, color, count));
        }

        public void SetUsersInterface(IMine.SettingArgs args) => Setting(this, args);
        
        private static void Setting(MineButton mineButton, IMine.SettingArgs args) 
        {
            ImageDetail.SetImage(mineButton._AreaImage, args.Sprite, args.Color);

            mineButton._MineNumber.SetText(args.Count);
            mineButton.gameObject.SetActive(args.Active);
        }

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
            var checkType = IMine.CheckType(eventData.button);

            IMine.MineButtonEvent(this, checkType);
        }

        #endregion
    }
}