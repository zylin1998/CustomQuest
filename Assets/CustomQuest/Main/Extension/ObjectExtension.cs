using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Custom
{
    public static class ObjectExtension
    {
        public static bool IsNull<T>(this T obj) 
        {
            return obj == null;
        }

        public static T IsNullandReturn<T>(this T obj, T isNull) 
        {
            return obj.IsNull() ? isNull : obj;
        }

        public static bool IsEqual(this object object1, object object2) 
        {
            if (object1.IsNull() || object2.IsNull()) { return false; }

            return object1.Equals(object2);
        }

        public static TOutput IsType<TOutput>(this object obj) 
        {
            return obj is TOutput output ? output : default(TOutput);
        }

        public static TOutput Convert<TInput, TOutput>(this TInput obj, Converter<TInput, TOutput> converter)
        {
            return converter.IsNull() ? default(TOutput) : converter.Invoke(obj);
        }

        public static bool TryConvert<TInput, TOutput>(this TInput input, out TOutput output)
        {
            output = input.IsType<TOutput>();

            return !output.IsNull();
        }
    }
}
