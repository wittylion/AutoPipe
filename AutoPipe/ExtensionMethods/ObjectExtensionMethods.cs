using System;

namespace AutoPipe
{
    internal static class ObjectExtensionMethods
    {
        public static bool Is<T>(this T obj, T reference) where T : class
        {
            return obj == reference;
        }

        public static bool IsNull<T>(this T obj) where T : class
        {
            return obj == null;
        }

        public static bool IsNotNull<T>(this T obj) where T : class
        {
            return !IsNull(obj);
        }

        public static bool HasValue<T>(this T obj) where T : class
        {
            return IsNotNull(obj);
        }

        public static bool HasNoValue<T>(this T obj) where T : class
        {
            return !HasValue(obj);
        }

        public static T[] ToAnArray<T>(this T obj)
        {
            return new[] {obj};
        }
    }
}