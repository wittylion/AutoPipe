namespace AutoPipe
{
    internal static class NullableStructureExtensionMethods
    {
        public static bool HasNoValue<T>(this T? val) where T : struct
        {
            return !val.HasValue;
        }

        public static bool IsNull<T>(this T? val) where T : struct
        {
            return !val.HasValue;
        }
    }
}