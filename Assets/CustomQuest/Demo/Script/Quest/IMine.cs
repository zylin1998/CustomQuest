using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Custom;
using Custom.Quest;

namespace QuestDemo
{
    public interface IMine : IReciever
    {
        public int Position { get; set; }
        public bool IsMine { get; set; }
        public bool IsFlag { get; set; }
        public bool IsDetected { get; set; }
        public List<IMine> Square { get; set; }
        
        public class DetailInfo 
        {
            public EMineMap AreaType { get; }
            public int MineCount { get; }

            public DetailInfo(EMineMap areaType, int mineCount)
                => (this.AreaType, this.MineCount) = (areaType, mineCount);
        }

        public class ImageInfo
        {
            public Sprite Sprite { get; }
            public Color Color { get; }
            public string Count { get; }
            public bool Active { get; }

            public ImageInfo(Sprite sprite, Color color, string count) : this(sprite, color, count, true) { }

            public ImageInfo(Sprite sprite, Color color, string count, bool active)
                => (this.Sprite, this.Color, this.Count, this.Active) = (sprite, color, count, active);
        }

        [Serializable]
        public struct ImageDetail
        {
            [SerializeField]
            private Sprite _Ground;
            [SerializeField]
            private Sprite _Mine;
            [SerializeField]
            private Sprite _Flag;
            [SerializeField]
            private ColorBlock _ColorBlock;

            public static ImageDetail Detail { get; set; }

            public static Sprite Ground => Detail._Ground;
            public static Sprite Mine => Detail._Mine;
            public static Sprite Flag => Detail._Flag;
            public static Color Normal => Detail._ColorBlock.Normal;
            public static Color PointerDown => Detail._ColorBlock.PointerDown;
            public static Color Clear => Detail._ColorBlock.Clear;

            public static void SetImage(Image image, Sprite sprite, Color color)
            {
                image.sprite = sprite;
                image.color = color;
            }

            public static void SetImage(Image image, Color color)
            {
                image.color = color;
            }

            [Serializable]
            public struct ColorBlock 
            {
                [SerializeField]
                private Color _Normal;
                [SerializeField]
                private Color _PointerDown;
                [SerializeField]
                private Color _Clear;

                public Color Normal => this._Normal;
                public Color PointerDown => this._PointerDown;
                public Color Clear => this._Clear;

                public ColorBlock(Color normal, Color pointerDown, Color clear) 
                {
                    this._Normal = normal;
                    this._PointerDown = pointerDown;
                    this._Clear = clear;
                }
            }
        }
    }
}
