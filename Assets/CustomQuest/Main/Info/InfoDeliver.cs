using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom
{
    public interface IProvider 
    {
        public object GetInfo();

        public T GetInfo<T>() => this is IProvider<T> provider ? provider.GetInfo() : default;
    }

    public interface IProvider<TInfo> : IProvider
    {
        public new TInfo GetInfo();
    }

    public interface IReciever 
    {
        public void SetInfo(object info);
    }

    public interface IReciever<TInfo> : IReciever
    {
        public void SetInfo(TInfo info);
    }
}