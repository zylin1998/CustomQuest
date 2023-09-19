using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom
{
    public static class SeriesEnumeratorExtension
    {
        public static List<CoordinateItem> Flatten(this ISeriesEnumerator enumerator)
        {
            var list = new List<CoordinateItem>();

            enumerator.Reset();
            
            for (; enumerator.MoveNext();) { list.Add(enumerator.Current); }

            return list;
        }

        public static List<CoordinateItem<T>> Flatten<T>(this ISeriesEnumerator enumerator)
        {
            var list = new List<CoordinateItem<T>>();

            enumerator
                .Flatten()
                .ForEach(item =>
                {
                    if (item.Item.TryConvert(out T result)) { list.Add(new(item.Coordinate, result)); }
                });
            
            return list;
        }

        public static List<T> Flatten<T>(this ISeriesEnumerator enumerator, Converter<CoordinateItem, T> converter)
        {
            return Flatten(enumerator).ConvertAll(item => converter.Invoke(item));
        }

        public static List<CoordinateItem> Flatten(this ISeriesEnumerator enumerator, int depth)
        {
            var list = new List<CoordinateItem>();

            enumerator.Reset();
            for (; enumerator.MoveNext(depth);)
            {
                list.Add(enumerator.DepthCurrent(depth));
            }

            return list;
        }

        public static List<CoordinateItem<T>> Flatten<T>(this ISeriesEnumerator enumerator, int depth)
        {
            var list = new List<CoordinateItem<T>>();

            enumerator
                .Flatten(depth)
                .ForEach(item =>
                {
                    if (item.Item.TryConvert(out T result)) { list.Add(new(item.Coordinate, result)); }
                });

            return list;
        }

        public static List<T> Flatten<T>(this ISeriesEnumerator enumerator, int depth, Converter<CoordinateItem, T> converter)
        {
            return Flatten(enumerator, depth).ConvertAll(item => converter.Invoke(item));
        }

        public static List<CoordinateItem> Flatten(this ISeriesEnumerable enumerable)
        {
            return enumerable.GetEnumerator().Flatten();
        }

        public static List<CoordinateItem<T>> Flatten<T>(this ISeriesEnumerable enumerable) 
        {
            return enumerable.GetEnumerator().Flatten<T>();
        }

        public static List<T> Flatten<T>(this ISeriesEnumerable enumerable, Converter<CoordinateItem, T> converter) 
        {
            return enumerable.Flatten().ConvertAll(item => converter.Invoke(item));
        }

        public static List<CoordinateItem> Flatten(this ISeriesEnumerable enumerable, int depth)
        {
            return enumerable.GetEnumerator().Flatten(depth);
        }

        public static List<CoordinateItem<T>> Flatten<T>(this ISeriesEnumerable enumerable, int depth) 
        {
            return enumerable.GetEnumerator().Flatten<T>(depth);
        }

        public static List<T> Flatten<T>(this ISeriesEnumerable enumerable, int depth, Converter<CoordinateItem, T> converter)
        {
            return enumerable.Flatten(depth).ConvertAll(item => converter.Invoke(item));
        }
    }
}
