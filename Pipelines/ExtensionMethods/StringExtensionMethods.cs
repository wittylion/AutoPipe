namespace Pipelines.ExtensionMethods
{
    internal static class StringExtensionMethods
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        public static bool IsNotNullOrWhiteSpace(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }
        
        public static bool HasValue(this string str)
        {
            return IsNotNullOrWhiteSpace(str);
        }

        public static bool HasNoValue(this string str)
        {
            return !HasValue(str);
        }

        public static string EnsureValue(this string str, string value)
        {
            return str.HasValue() ? str : value;
        }

        public static string InsureWithEmptyString(this string str)
        {
            return str ?? string.Empty;
        }

        public static string FormatWith(this string str, params object[] args)
        {
            if (str.HasValue() && args.HasValue())
                return string.Format(str, args);

            return str;
        }
    }
}