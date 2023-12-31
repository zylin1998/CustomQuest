﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Quest
{
    public abstract class Element : ScriptableObject, IElement
    {
        public abstract IElement Reset();
        public abstract void Invoke();
        public abstract void EndInvoke();
    }

    public interface IElement
    {
        public void Invoke();
        public void EndInvoke();
        public IElement Reset();
    }
}
