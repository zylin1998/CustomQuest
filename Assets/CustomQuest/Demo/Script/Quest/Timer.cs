using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Custom.Quest;
using UnityEngine;

namespace QuestDemo
{
    [Serializable]
    public class Timer : System.Timers.Timer, IElement
    {
        [SerializeField]
        private float _PassTime = 0f;

        public float PassTime => this._PassTime;

        public Timer() : base()
        {
            this.Elapsed += this.Adding;
        }

        public Timer(double interval) : base(interval)
        {
            this.Elapsed += this.Adding;
        }

        public void Adding(object sender, System.Timers.ElapsedEventArgs e)
        {
            this._PassTime += (float)(this.Interval / 1000f);
        }

        public void Initialize()
        {
            this._PassTime = 0f;
        }

        public void Invoke()
        {
            this.Start();
        }
    }
}
