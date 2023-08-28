using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QuestDemo
{
    public interface IMine
    {
        public int Position { get; set; }
        public bool IsMine { get; set; }
        public bool IsFlag { get; protected set; }
        public bool IsDetected { get; protected set; }
        public List<IMine> Square { get; protected set; }

        public void SetUsersInterface(SettingArgs args);

        #region Static Properties

        protected static Action<MapArgs> OnDetected = (var) => { };

        public static event Action<MapArgs> DetectedEvent
        {
            add => OnDetected += value;

            remove => OnDetected -= value;
        }

        private static EMineMap _DetectedType;

        public static EMineMap DetectedType
        {
            get => _DetectedType;

            set
            {
                _DetectedType = value;

                OnTypeChanged.Invoke(value);
            }
        }

        private static Action<EMineMap> OnTypeChanged = (type) => { };

        public static event Action<EMineMap> TypeChangedEvent
        {
            add => OnTypeChanged += value;

            remove => OnTypeChanged -= value;
        }

        #endregion

        #region Interact Movement

        protected static int Detected(IMine mine)
        {
            if (mine.IsDetected || mine.IsFlag) { return 0; }

            mine.IsDetected = true;

            var sprite = QuestDemoUI.ImageDetail.Mine;
            var color = ImageDetail.Normal;
            var count = string.Empty;

            if (!mine.IsMine)
            {
                var mineCount = mine.Square.Count(c => c.IsMine);
                var hasMine = mineCount > 0;

                sprite = null;
                color = ImageDetail.Clear;
                count = string.Format("{0}", hasMine ? mineCount : "");

                mine.SetUsersInterface(new SettingArgs(sprite, color, count));

                return 1 + (hasMine ? 0 : mine.Square.Sum(f => Detected(f)));
            }

            mine.SetUsersInterface(new SettingArgs(sprite, color, count));

            return -1;
        }

        protected static int SetFlag(IMine mine)
        {
            if (!mine.IsDetected)
            {
                var sprite = mine.IsFlag ? QuestDemoUI.ImageDetail.Ground : QuestDemoUI.ImageDetail.Flag;
                var color = ImageDetail.Normal;
                var count = string.Empty;
                var isFlag = mine.IsFlag ? -1 : 1;

                mine.IsFlag = !mine.IsFlag;
                mine.SetUsersInterface(new SettingArgs(sprite, color, count));

                return isFlag;
            }

            return 0;
        }

        #endregion

        #region State Setting

        public static void SetSquare(IMine mine, IEnumerable<IMine> others)
        {
            mine.Square.Clear();
            mine.Square.AddRange(others);
        }

        public static void ShowMine(IMine mine)
        {
            var sprite = mine.IsMine ? QuestDemoUI.ImageDetail.Mine : null;
            var color = mine.IsMine ? ImageDetail.Normal : ImageDetail.Clear;
            var count = string.Empty;

            if (!mine.IsMine)
            {
                var mineCount = mine.Square.Count(c => c.IsMine);

                count = string.Format("{0}", mineCount > 0 ? mineCount : "");
            }

            mine.SetUsersInterface(new SettingArgs(sprite, color, count));
        }

        public static void Reset(IMine mine)
        {
            mine.IsMine = false;
            mine.IsFlag = false;
            mine.IsDetected = false;
            mine.Position = 0;

            var sprite = QuestDemoUI.ImageDetail.Ground;
            var color = ImageDetail.Normal;
            var count = string.Empty;
            var active = true;

            mine.SetUsersInterface(new SettingArgs(sprite, color, count, active));
        }

        protected static EMineMap CheckType(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left) { return DetectedType; }
            if (button == PointerEventData.InputButton.Right) { return EMineMap.Flag; }

            return EMineMap.None;
        }

        protected static void MineButtonEvent(IMine mine, EMineMap checkType)
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

        public class SettingArgs
        {
            public Sprite Sprite { get; }
            public Color Color { get; }
            public string Count { get; }
            public bool Active { get; }

            public SettingArgs(Sprite sprite, Color color, string count) : this(sprite, color, count, true) { }

            public SettingArgs(Sprite sprite, Color color, string count, bool active)
                => (this.Sprite, this.Color, this.Count, this.Active) = (sprite, color, count, active);
        }
    }
}
