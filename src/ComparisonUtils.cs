namespace CodeZero.Core
{
    internal static class ComparisonUtils
    {
        internal static bool IsGreaterThan<T>(this T current, T other) 
            where T : IComparable<T>
        {
            return current.CompareTo(other) > 0;
        }

        internal static bool IsGreaterThanOrEqualTo<T>(this T current, T other) 
            where T : IComparable<T>
        {
            return current.CompareTo(other) >= 0;
        }

        internal static bool IsLessThan<T>(this T current, T other) 
            where T : IComparable<T>
        {
            return current.CompareTo(other) < 0;
        }

        internal static bool IsLessThanOrEqualTo<T>(this T current, T other) 
            where T : IComparable<T>
        {
            return current.CompareTo(other) <= 0;
        }
    }
}