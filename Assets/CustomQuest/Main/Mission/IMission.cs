using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Custom.Quest
{
    public interface IMissionInfo : IDescribe, IProvider
    {
        public static List<TMissionInfo> SelectMission<TMissionInfo>(IEnumerable<Info> missions, Func<Info, bool> selector)
                where TMissionInfo : Info
            => SelectMission(missions, selector).OfType<TMissionInfo>().ToList();

        public static List<Info> SelectMission(IEnumerable<Info> missions, Func<Info, bool> selector)
            => missions.Where(coordinate => selector.Invoke(coordinate)).ToList();

        public abstract class Info : IDescribe, IClear
        {
            private EProgress _Progress;

            public string Describe { get; }
            public bool IsClear { get; protected set; }
            
            public EProgress Progress 
            { 
                get => this._Progress;
                
                set 
                {
                    this._Progress = value;

                    if (this._Progress == EProgress.Complete || this._Progress == EProgress.End)
                    {
                        this.IsClear = true;
                    }
                } 
            }

            public abstract EProgress Achieve(object sender);

            public Info(string describe, EProgress progress) 
                => (this.Describe, this.Progress) = (describe, progress);
        }

        [Serializable]
        public enum EProgress
        {
            None,
            UnComplete,
            Complete,
            End
        }
    }

}
