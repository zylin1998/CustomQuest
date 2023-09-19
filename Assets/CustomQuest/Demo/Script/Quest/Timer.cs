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
        private double _PassTime = 0f;
        
        public TimeDisplay PassTime => new TimeDisplay(this._PassTime);

        public Timer() : base()
        {
            this._PassTime = 0;
            this.Elapsed += this.Adding;
        }
        
        public Timer(double passTime, double interval) : base(interval)
        {
            this._PassTime = passTime;
            this.Elapsed += this.Adding;
        }

        public void Adding(object sender, System.Timers.ElapsedEventArgs e) => this._PassTime += this.Interval;

        public IElement Reset() 
        {
            this._PassTime = 0;

            return this;
        }

        public void Invoke() => this.Start();
        public void EndInvoke() => this.Stop();
    }

    [Serializable]
    public struct TimeDisplay 
    {
        [SerializeField]
        private int _Hour;
        [SerializeField]
        private int _Minute;
        [SerializeField]
        private int _Second;
        [SerializeField]
        private int _MiniSecond;

        public int Hour => this._Hour;
        public int Minute => this._Minute;
        public int Second => this._Second;
        public int MiniSecond => this._MiniSecond;

        public double msTime { get; }
        public int SecondOnly { get; }

        public TimeDisplay(double time) 
        {
            this.msTime = time;

            this.SecondOnly = (int)this.msTime / 1000;

            this._MiniSecond = (int)this.msTime % 1000;
            this._Hour = this.SecondOnly / 3600;
            this._Minute = this.SecondOnly % 3600 / 60;
            this._Second = this.SecondOnly % 60;
        }
    }
}
