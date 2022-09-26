using System.Diagnostics.CodeAnalysis;

namespace category_theory;

public readonly struct Bound<T> : IEquatable<Bound<T>>, IComparable<Bound<T>>, IComparisonOperators<Bound<T>, Bound<T>>, IEqualityOperators<Bound<T>, Bound<T>>
    where T : struct, IComparable<T>, IEquatable<T>, IComparisonOperators<T, T>, IEqualityOperators<T, T>
{
    public Bound(T value, bool isIncluded)
    {
        IsIncluded = isIncluded;
        Value = value;
    }

    public T Value { get; }
    public bool IsIncluded { get; }

    public int CompareTo(Bound<T> other)
    {
        if (Equals(other))
            return 0;

        if (Value.Equals(other.Value))
        {
            return IsIncluded && !other.IsIncluded
                ? 1
                : -1;
        }

        return Value.CompareTo(other.Value);
    }

    public int CompareTo(object? obj) =>
        obj is not null and Bound<T> bound
            ? CompareTo(bound)
            : -1;

    public bool Equals(Bound<T> other) =>
        Value == other.Value &&
        IsIncluded == other.IsIncluded;

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is not null &&
        obj is Bound<T> bound &&
        Equals(bound);

    public override int GetHashCode() =>
        HashCode.Combine(Value, IsIncluded);

    public override string? ToString()
    {
        var brackets =
            IsIncluded
                ? ("[", "]")
                : ("(", ")");

        return $"{brackets.Item1}{Value}{brackets.Item2}";
    }

    public static Bound<T> Inclusive(T value) => new(value, true);
    public static Bound<T> Exclusive(T value) => new(value, false);

    public static bool operator ==(Bound<T> left, Bound<T> right) => left.Equals(right);
    public static bool operator !=(Bound<T> left, Bound<T> right) => !(left == right);
    public static bool operator <(Bound<T> left, Bound<T> right) => left.CompareTo(right) < 0;
    public static bool operator >(Bound<T> left, Bound<T> right) => right.CompareTo(left) < 0;
    public static bool operator <=(Bound<T> left, Bound<T> right) => left == right || left < right;
    public static bool operator >=(Bound<T> left, Bound<T> right) => left == right || left > right;
}