using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Quest 
{
    public abstract class QuestInfo : ScriptableObject, IQuestInfo
    {
        public abstract object GetInfo();
    }

    public interface IQuestInfo : IProvider
    {
        public class Info : IClear 
        {
            public bool IsClear { get; set; }
        }
    }
}
