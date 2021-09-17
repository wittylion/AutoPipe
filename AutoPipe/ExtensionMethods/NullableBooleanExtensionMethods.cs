namespace AutoPipe
{
    internal static class NullableBooleanExtensionMethods
    {
        public static bool HasTrueValue(this bool? val)
        {
            return val.HasValue && val.Value;
        }

        public static bool HasFalseValue(this bool? val)
        {
            return val.HasValue && !val.Value;
        }

        public static bool HasPositiveValue(this bool? val)
        {
            return HasTrueValue(val);
        }

        public static bool HasNegativeValue(this bool? val)
        {
            return HasFalseValue(val);
        }
    }
}