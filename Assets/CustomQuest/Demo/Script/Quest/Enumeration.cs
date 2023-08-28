using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom
{
    public abstract class Enumeration<TEnum> : IEquatable<Enumeration<TEnum>> where TEnum : Enumeration<TEnum> 
    {
        private int Value { get; }
        private string Name { get; }

        protected Enumeration(int value, string name) 
        {
            this.Value = value;
            this.Name = name;
        }

        private static Dictionary<int, TEnum> _Enumerations;

        private static Dictionary<int, TEnum> Enumerations
        {
            get
            {
                if (_Enumerations == null)
                {
                    _Enumerations = CreateEnumerations();
                }

                return _Enumerations;
            }
        }

        public static TEnum FromValue(int value) 
        {
            return Enumerations.TryGetValue(value, out TEnum enumeration) ? enumeration : default;
        }

        public static TEnum FromName(string name)
        {
            return Enumerations.Values.SingleOrDefault(e => e.Name == name);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is TEnum other ? this.Equals(other) : false;
        }

        public bool Equals(Enumeration<TEnum> other) 
        {
            if (other is null) { return false; }

            return this.GetType() == other.GetType() && this.Value == other.Value;
        }

        public static Dictionary<int, TEnum> CreateEnumerations()
        {
            var type = typeof(TEnum);

            var list = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                           .Where(fieldInfo => type.IsAssignableFrom(fieldInfo.FieldType))
                           .Select(fieldInfo => (TEnum)fieldInfo.GetValue(default)!);
            
            return list.ToDictionary(x => x.Value);
        }
    }
}