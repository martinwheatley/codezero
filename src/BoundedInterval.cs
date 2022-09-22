namespace category_theory;

public readonly struct BoundedInterval<T> : IEquatable<BoundedInterval<T>>
    where T : struct, IComparable<T>, IEquatable<T>, IComparisonOperators<T, T>, IEqualityOperators<T, T>
{
    public BoundedInterval(T from, T to)
    {
        if (to < from)
            throw new ArgumentException($"Invalid interval: {nameof(to)} ({to}) must be greater than or equal to {nameof(from)} ({from}).", nameof(to));

        From = from;
        To = to;
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsDefault => Equals(default);

    /// <summary>
    /// Inclusive from.
    /// </summary>
    public T From { get; }

    /// <summary>
    /// Inclusive to.
    /// </summary>
    public T To { get; }

    /// <summary>
    /// Determines whether <paramref name="item"/> is contained within the bounds of the current <see cref="BoundedInterval{T}"/>.
    /// </summary>
    /// <param name="item">The element to determine whether is included or not.</param>
    /// <returns></returns>
    public bool Contains(T item) =>
        item >= From &&
        item <= To;

    /// <summary>
    /// Determines whether <paramref name="other"/> is a sub-interval of the current <see cref="BoundedInterval{T}"/>.
    /// </summary>
    /// <param name="other">The interval to determine whether is included or not.</param>
    /// <returns></returns>
    public bool Contains(BoundedInterval<T> other) =>
        Contains(other.From) &&
        Contains(other.To);

    public bool Overlaps(BoundedInterval<T> other) =>
        Contains(other.From) ||
        Contains(other.To) ||
        other.Contains(From) ||
        other.Contains(To);

    /// <summary>
    /// Merges <paramref name="other"/> with the current <see cref="BoundedInterval{T}"/>. The return value indicates whether the merge succeeded.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="result">
    /// When this method returns <see langword="true"/>, <paramref name="result"/> contains is <see cref="BoundedInterval{T}"/> with From = Min(<see langword="this"/>.From, <paramref name="other"/>.From) 
    /// and To = Max(<see langword="this"/>.To, <paramref name="other"/>.To).
    /// <para>
    /// When this method returns <see langword="false"/>, <see langword="this"/> and <paramref name="other"/> does not overlap, and result is <see langword="default"/>(<see cref="BoundedInterval{T}"/>).
    /// </para>
    /// </param>
    /// <returns><see langword="true"/> if the two <see cref="BoundedInterval{T}"/> overlap; otherwise <see langword="false"/>.</returns>
    public bool TryMerge(BoundedInterval<T> other, out BoundedInterval<T> result)
    {
        result = default;

        if (!Overlaps(other))
            return false;

        var from = Min(From, other.From);
        var to = Max(To, other.To);

        result = new(from, to);

        return true;
    }

    public bool Equals(BoundedInterval<T> other) =>
        From.Equals(other.From) &&
        To.Equals(other.To);

    public override bool Equals(object? obj) =>
        obj is not null &&
        obj is BoundedInterval<T> interval &&
        Equals(interval);

    public override int GetHashCode() =>
        HashCode.Combine(From, To);

    public override string ToString() => $"[{From}..{To}]";

    private static T Max(T x1, T x2) =>
        x1 > x2 ? x1 : x2;

    private static T Min(T x1, T x2) =>
        x1 < x2 ? x1 : x2;

    public static bool operator ==(BoundedInterval<T> left, BoundedInterval<T> right) =>
        left.Equals(right);

    public static bool operator !=(BoundedInterval<T> left, BoundedInterval<T> right) =>
        !(left == right);
}