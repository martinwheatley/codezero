using System.Diagnostics.CodeAnalysis;

namespace category_theory;

public readonly struct Bound<T> : IEquatable<Bound<T>>, IComparable<Bound<T>>
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
            if (IsIncluded && !other.IsIncluded)
                return 1;
            
            return -1;
        }

        return Value.CompareTo(other.Value);
    }

    public bool Equals(Bound<T> other) => 
        Value == other.Value && 
        IsIncluded == other.IsIncluded;

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is not null &&
        obj is Bound<T> bound &&
        Equals(bound);

    public override int GetHashCode() => 
        HashCode.Combine(Value, IsIncluded);

    public override string? ToString() => $"{Value}";
}