using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Quest
{
    public abstract class Element : ScriptableObject, IElement
    {
        public abstract void Initialize();
        public abstract void Invoke();
        public abstract void EndInvoke();
        public abstract void Reset();
    }

    public interface IElement 
    {
        public void Initialize();
        public void Invoke();
        public void EndInvoke();
        public void Reset();
    }
}
