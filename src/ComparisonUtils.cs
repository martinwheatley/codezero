namespace CodeZero.Core
{
    internal static class ComparisonUtils
    {
        public static bool IsGreaterThan<T>(this T current, T other) 
            where T : IComparable<T>
        {
            return current.CompareTo(other) > 0;
        }

        public static bool IsGreaterThanOrEqualTo<T>(this T current, T other) 
            where T : IComparable<T>
        {
            return current.CompareTo(other) >= 0;
        }

        public static bool IsLessThan<T>(this T current, T other) 
            where T : IComparable<T>
        {
            return current.CompareTo(other) < 0;
        }

        public static bool IsLessThanOrEqualTo<T>(this T current, T other) 
            where T : IComparable<T>
        {
            return current.CompareTo(other) <= 0;
        }
    }
}