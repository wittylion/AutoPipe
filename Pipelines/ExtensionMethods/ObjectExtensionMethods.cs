namespace Pipelines.ExtensionMethods
{
    internal static class ObjectExtensionMethods
    {
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

        public static T Ensure<T>(this T obj, T value) where T : class
        {
            return obj.HasValue() ? obj : value;
        }

        public static T[] ToArray<T>(this T obj)
        {
            return new[] {obj};
        }
    }
}