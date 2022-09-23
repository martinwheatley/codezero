namespace category_theory;

/// <summary>
/// Represents an interval of <typeparamref name="T"/>s, where <typeparamref name="T"/> follows total order.
/// </summary>
/// <typeparam name="T">The type of elements in the interval.</typeparam>
public readonly struct BoundedInterval<T> : IEquatable<BoundedInterval<T>>
    where T : struct, IComparable<T>, IEquatable<T>, IComparisonOperators<T, T>, IEqualityOperators<T, T>
{
    /// <summary>
    /// Initializes a <see cref="BoundedInterval{T}"/> with endpoints <paramref name="from"/> and <paramref name="to"/>.
    /// </summary>
    /// <param name="from">The lower bound which might or might not be included.</param>
    /// <param name="to">The upper bound which might or might not be included.</param>
    /// <exception cref="ArgumentException"></exception>
    public BoundedInterval(Bound<T> from, Bound<T> to)
    {
        if (to.Value < from.Value)
            throw new ArgumentException($"Invalid interval: {nameof(to)} ({to}) must be greater than or equal to {nameof(from)} ({from}).", nameof(to));

        From = from;
        To = to;
    }

    /// <summary>
    /// Initializes a <see cref="BoundedInterval{T}"/> with inclusive endpoints <paramref name="from"/> and <paramref name="to"/>.
    /// </summary>
    /// <param name="from">The lower inclusive bound.</param>
    /// <param name="to">The upper inclusive bound.</param>
    /// <exception cref="ArgumentException"></exception>
    public BoundedInterval(T from, T to)
        : this(new Bound<T>(from, true), new Bound<T>(to, true))
    {
    }

    public bool IsEmpty =>
        From.Value.Equals(To.Value) &&
        !(To.IsIncluded && From.IsIncluded);

    /// <summary>
    /// Determines whether the <see cref="BoundedInterval{T}"/> contains exactly one element; i.e. <see cref="From"/> == <see cref="To"/>.
    /// </summary>
    public bool IsSingeton =>
        From.Value.Equals(To.Value) &&
        From.IsIncluded &&
        To.IsIncluded;

    /// <summary>
    /// Determines whether both <see cref="From"/> and <see cref="To"/> are default.
    /// </summary>
    public bool IsDefault => Equals(default);

    /// <summary>
    /// The lowerbound endpoint.
    /// </summary>
    public Bound<T> From { get; }

    /// <summary>
    /// The upperbound endpoint.
    /// </summary>
    public Bound<T> To { get; }

    /// <summary>
    /// Determines whether <paramref name="item"/> is contained within the bounds of the current <see cref="BoundedInterval{T}"/>.
    /// </summary>
    /// <param name="item">The element that might or might not be contained in the <see cref="BoundedInterval{T}"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is included in the current <see cref="BoundedInterval{T}"/>; otherwise <see langword="false"/>.</returns>
    public bool Contains(T item)
    {
        return
            (From.IsIncluded, To.IsIncluded) switch
            {
                (true, true) => From.Value <= item && item <= To.Value,
                (true, false) => From.Value <= item && item < To.Value,
                (false, true) => From.Value < item && item <= To.Value,
                (false, false) => From.Value < item && item < To.Value
            };
    }

    /// <summary>
    /// Determines whether <paramref name="other"/> is a sub-interval of the current <see cref="BoundedInterval{T}"/>.
    /// </summary>
    /// <param name="other">The other <see cref="BoundedInterval{T}"/> that might or might not be contained in the current <see cref="BoundedInterval{T}"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="other"/> is a sub-interval of the current <see cref="BoundedInterval{T}"/>; otherwise <see langword="false"/>.</returns>
    public bool Contains(BoundedInterval<T> other)
    {
        var containsLower = other.From >= From;
        var containsUpper = other.To <= To;

        return containsLower && containsUpper;
    }

    /// <summary>
    /// Determines whether <paramref name="other"/> overlaps the current <see cref="BoundedInterval{T}"/>.
    /// </summary>
    /// <param name="other">The other <see cref="BoundedInterval{T}"/> that might or might not overlap the current <see cref="BoundedInterval{T}"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="other"/> overlaps the current <see cref="BoundedInterval{T}"/>; otherwise <see langword="false"/>.</returns>
    public bool Overlaps(BoundedInterval<T> other) =>
        Contains(other.From) ||
        Contains(other.To) ||
        other.Contains(From) ||
        other.Contains(To);

    private bool Contains(Bound<T> bound) =>
        bound >= From && bound <= To;

    /// <summary>
    /// Merges an<paramref name="other"/> <see cref="BoundedInterval{T}"/> with the current <see cref="BoundedInterval{T}"/>. The return value indicates whether the merge succeeded.
    /// </summary>
    /// <param name="other">The other <see cref="BoundedInterval{T}"/> to merge with the current <see cref="BoundedInterval{T}"/>.</param>
    /// <param name="result">
    /// When this method returns <see langword="true"/>, <paramref name="result"/> is a new <see cref="BoundedInterval{T}"/> with From = Min(<see langword="this"/>.From, <paramref name="other"/>.From) 
    /// and To = Max(<see langword="this"/>.To, <paramref name="other"/>.To).
    /// <para>
    /// When this method returns <see langword="false"/>, the current <see cref="BoundedInterval{T}"/> and the <paramref name="other"/> <see cref="BoundedInterval{T}"/> don't overlap, and <paramref name="result"/> is <see langword="default"/>(<see cref="BoundedInterval{T}"/>).
    /// </para>
    /// </param>
    /// <returns><see langword="true"/> if the <paramref name="other"/> <see cref="BoundedInterval{T}"/> overlaps the current <see cref="BoundedInterval{T}"/>; otherwise <see langword="false"/>.</returns>
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

    /// <summary>
    /// Determines whether the <paramref name="other"/> <see cref="BoundedInterval{T}"/> is equal to the current <see cref="BoundedInterval{T}"/>.
    /// </summary>
    /// <param name="other">The other <see cref="BoundedInterval{T}"/> that might or might not be equal to the current <see cref="BoundedInterval{T}"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="other"/> <see cref="BoundedInterval{T}"/> equals the current <see cref="BoundedInterval{T}"/>; otherwise <see langword="false"/>.</returns>
    public bool Equals(BoundedInterval<T> other) =>
        From.Equals(other.From) &&
        To.Equals(other.To);

    /// <summary>
    /// Determines whether <paramref name="obj"/> is equal to the current <see cref="BoundedInterval{T}"/>.
    /// </summary>
    /// <param name="obj">The <see cref="object"/> that might or might not be equal to the current <see cref="BoundedInterval{T}"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="obj"/> equals the current <see cref="BoundedInterval{T}"/>; otherwise <see langword="false"/>.</returns>
    public override bool Equals(object? obj) =>
        obj is not null &&
        obj is BoundedInterval<T> interval &&
        Equals(interval);

    /// <summary>
    /// The hash code of the current instance.
    /// </summary>
    /// <returns>The hash code that represents the current <see cref="BoundedInterval{T}"/>.</returns>
    public override int GetHashCode() =>
        HashCode.Combine(From, To);

    /// <inheritdoc/>>
    public override string ToString()
    {
        var leftBracket = From.IsIncluded ? "[" : "(";
        var rightBracket = To.IsIncluded ? "]" : ")";
        var singletonValue = IsSingeton && From.IsIncluded ? From.Value : To.Value;

        return IsEmpty
            ? $"ø"
            : IsSingeton
                ? $"{{{singletonValue}}}"
                : $"{leftBracket}{From}..{To}{rightBracket}";
    }

    private static Bound<T> Max(Bound<T> x1, Bound<T> x2) =>
        x1 > x2 ? x1 : x2;

    private static Bound<T> Min(Bound<T> x1, Bound<T> x2) =>
        x1 < x2 ? x1 : x2;

    public static bool operator ==(BoundedInterval<T> left, BoundedInterval<T> right) =>
        left.Equals(right);

    public static bool operator !=(BoundedInterval<T> left, BoundedInterval<T> right) =>
        !(left == right);
}

//internal interface IBoundedInterval<T>
//    where T : struct, IComparable<T>, IEquatable<T>, IComparisonOperators<T, T>, IEqualityOperators<T, T>
//{
//    T From { get; }
//    T To { get; }

//    public bool Contains(T item) =>
//        item >= From &&
//        item <= To;

//    public bool Overlaps(BoundedInterval<T> other) =>
//        Contains(other.From) ||
//        Contains(other.To) ||
//        other.Contains(From) ||
//        other.Contains(To);

//    public bool TryMerge(BoundedInterval<T> other, out BoundedInterval<T> result)
//    {
//        result = default;

//        if (!Overlaps(other))
//            return false;

//        var from = Min(From, other.From);
//        var to = Max(To, other.To);

//        result = new(from, to);

//        return true;
//    }

//    private static T Max(T x1, T x2) =>
//        x1 > x2 ? x1 : x2;

//    private static T Min(T x1, T x2) =>
//        x1 < x2 ? x1 : x2;
//}