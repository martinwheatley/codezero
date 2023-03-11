namespace CodeZero.Core;

/// <summary>
/// Represents a proper bounded interval of type <typeparamref name="T"/>, where <typeparamref name="T"/> follows total order.
/// </summary>
/// <typeparam name="T">The type of elements in the interval.</typeparam>
public readonly struct BoundedInterval<T> : IEquatable<BoundedInterval<T>>
    where T : struct, IComparable<T>, IEquatable<T>, IParsable<T>
{
    /// <summary>
    /// Initializes a <see cref="BoundedInterval{T}"/> with endpoints <paramref name="from"/> and <paramref name="to"/>.
    /// </summary>
    /// <param name="from">The lower <see cref="Bound{T}"/> of the interval, which might or might not be included.</param>
    /// <param name="to">The upper <see cref="Bound{T}"/> of the interval, which might or might not be included.</param>
    /// <exception cref="ArgumentException"></exception>
    public BoundedInterval(Bound<T> from, Bound<T> to)
    {
        if (to.Value.Equals(from.Value) && to.IsIncluded != from.IsIncluded)
            throw new ArgumentException($"Invalid {nameof(BoundedInterval<T>)}: {nameof(to)}.Value and {nameof(from)}.Value must not be equal, when {nameof(to)}.IsIncluded and {nameof(from)}.IsIncluded are unequal.");

        if (to.IsLessThan(from))
            throw new ArgumentException($"Invalid interval: the value of '{nameof(to)}' ({to}) must be greater than or equal to the value of '{nameof(from)}' ({from}).");

        From = from;
        To = to;
    }

    /// <summary>
    /// Initializes a <see cref="BoundedInterval{T}"/> with inclusive endpoints <paramref name="from"/> and <paramref name="to"/>.
    /// </summary>
    /// <param name="from">The lower inclusive <see cref="Bound{T}"/> of the interval.</param>
    /// <param name="to">The upper inclusive <see cref="Bound{T}"/> of the interval.</param>
    /// <exception cref="ArgumentException"></exception>
    public BoundedInterval(T from, T to)
        : this(new Bound<T>(from, true), new Bound<T>(to, true))
    {
    }

    /// <summary>
    /// Determines whether the <see cref="BoundedInterval{T}"/> contains no elements; <see cref="From"/>.Value == <see cref="To"/>.Value, furthermore neither <see cref="From"/> or <see cref="To"/> are included.
    /// </summary>
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
    /// The lower <see cref="Bound{T}"/> of the interval.
    /// </summary>
    public Bound<T> From { get; }

    /// <summary>
    /// The upper <see cref="Bound{T}"/> of the interval.
    /// </summary>
    public Bound<T> To { get; }

    /// <summary>
    /// Determines whether <paramref name="item"/> is contained within the bounds of the current <see cref="BoundedInterval{T}"/>.
    /// </summary>
    /// <param name="item">The element that might or might not be contained in the <see cref="BoundedInterval{T}"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="item"/> is included in the current <see cref="BoundedInterval{T}"/>; otherwise <see langword="false"/>.</returns>
    public bool Contains(T item)
    {
        return
            (From.IsIncluded, To.IsIncluded) switch
            {
                (true, true) => From.Value.IsLessThanOrEqualTo(item) && item.IsLessThanOrEqualTo(To.Value),
                (true, false) => From.Value.IsLessThanOrEqualTo(item) && item.IsLessThan(To.Value),
                (false, true) => From.Value.IsLessThan(item) && item.IsLessThanOrEqualTo(To.Value),
                (false, false) => From.Value.IsLessThan(item) && item.IsLessThan(To.Value)
            };
    }

    /// <summary>
    /// Determines whether <paramref name="other"/> is a sub-interval of the current <see cref="BoundedInterval{T}"/>.
    /// </summary>
    /// <param name="other">The other <see cref="BoundedInterval{T}"/> that might or might not be contained in the current <see cref="BoundedInterval{T}"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="other"/> is a sub-interval of the current <see cref="BoundedInterval{T}"/>; otherwise <see langword="false"/>.</returns>
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
    /// <returns><see langword="true"/> if <paramref name="other"/> overlaps the current <see cref="BoundedInterval{T}"/>; otherwise <see langword="false"/>.</returns>
    public bool Overlaps(BoundedInterval<T> other) =>
        Contains(other.From) ||
        Contains(other.To) ||
        other.Contains(From) ||
        other.Contains(To);

    /// <summary>
    /// Determines whether <paramref name="other"/> is adjacent on the current <see cref="BoundedInterval{T}"/> in either the beginning or the end of the interval.
    /// </summary>
    /// <param name="other">The other <see cref="BoundedInterval{T}"/> that might or might not be adjacent on the current <see cref="BoundedInterval{T}"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="other"/> is adjacent on the current <see cref="BoundedInterval{T}"/>; otherwise <see langword="false"/>.</returns>
    public bool IsAdjacentOn(BoundedInterval<T> other) =>
        other.Precedes(this) ||
        other.Follows(this);

    /// <summary>
    /// Determines whether the current <see cref="BoundedInterval{T}"/> exactly precedes <paramref name="other"/>.
    /// </summary>
    /// <param name="other">The other <see cref="BoundedInterval{T}"/> that might or might not be preceded by the current <see cref="BoundedInterval{T}"/>.</param>
    /// <returns><see langword="true"/> if the current <see cref="BoundedInterval{T}"/> exactly precedes <paramref name="other"/>; otherwise <see langword="false"/>.</returns>
    public bool Precedes(BoundedInterval<T> other) =>
        To.Value.Equals(other.From.Value) &&
        To.IsIncluded != other.From.IsIncluded;

    /// <summary>
    /// Determines whether the current <see cref="BoundedInterval{T}"/> exactly follows <paramref name="other"/>.
    /// </summary>
    /// <param name="other">The other <see cref="BoundedInterval{T}"/> that might or might not be followed by the current <see cref="BoundedInterval{T}"/>.</param>
    /// <returns><see langword="true"/> if the current <see cref="BoundedInterval{T}"/> exactly follows <paramref name="other"/>; otherwise <see langword="false"/>.</returns>
    public bool Follows(BoundedInterval<T> other) =>
        other.Precedes(this);

    private bool Contains(Bound<T> bound) =>
        bound >= From &&
        bound <= To;

    /// <summary>
    /// Merges <paramref name="other"/> with the current <see cref="BoundedInterval{T}"/>. The return value indicates whether the merge succeeded.
    /// </summary>
    /// <param name="other">The <see cref="BoundedInterval{T}"/> to merge with the current <see cref="BoundedInterval{T}"/>.</param>
    /// <param name="result">
    /// When this method returns <see langword="true"/>, <paramref name="result"/> is a new <see cref="BoundedInterval{T}"/> with From = Min(<see langword="this"/>.From, <paramref name="other"/>.From) 
    /// and To = Max(<see langword="this"/>.To, <paramref name="other"/>.To).
    /// <para>
    /// When this method returns <see langword="false"/>, the current <see cref="BoundedInterval{T}"/> and the <paramref name="other"/> <see cref="BoundedInterval{T}"/> don't overlap, 
    /// and <paramref name="result"/> is <see langword="default"/>(<see cref="BoundedInterval{T}"/>).
    /// </para>
    /// </param>
    /// <returns><see langword="true"/> if <paramref name="other"/> overlaps the current <see cref="BoundedInterval{T}"/>; otherwise <see langword="false"/>.</returns>
    public bool TryMerge(BoundedInterval<T> other, out BoundedInterval<T> result)
    {
        result = default;

        if (!(Overlaps(other) || IsAdjacentOn(other)))
            return false;

        var minLower = Min(From, other.From);
        var maxUpper = Max(To, other.To);

        result = new(minLower, maxUpper);

        return true;
    }

    //TODO: Needs to be tested before it's shipped with the public API.
    internal bool TryPartition(T value, out BoundedInterval<T>[] result)
    {
        result = Array.Empty<BoundedInterval<T>>();

        if (!Contains(value))
            return false;

        // Should probably return an OrderedCollection<BoundedInterval<T>>

        result =
            new BoundedInterval<T>[2]
            {
                new BoundedInterval<T>(From, new Bound<T>(value, false)),
                new BoundedInterval<T>(new Bound<T>(value, true), To)
            };

        return true;
    }

    /// <summary>
    /// Determines whether <paramref name="other"/> <see cref="BoundedInterval{T}"/> is equal to the current <see cref="BoundedInterval{T}"/>.
    /// </summary>
    /// <param name="other">The <see cref="BoundedInterval{T}"/> that might or might not be equal to the current <see cref="BoundedInterval{T}"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="other"/> <see cref="BoundedInterval{T}"/> equals the current <see cref="BoundedInterval{T}"/>; otherwise <see langword="false"/>.</returns>
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
                : $"{leftBracket}{From.Value}..{To.Value}{rightBracket}";
    }

    public static bool TryParse(string s, out BoundedInterval<T> result) =>
        TryParse(s.AsSpan(), null, out result);

    public static bool TryParse(string s, IFormatProvider? provider, out BoundedInterval<T> result) =>
        TryParse(s.AsSpan(), provider, out result);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out BoundedInterval<T> result)
    {
        result = default;

        if (s.IsEmpty)
            return false;

        var firstIndexOfDotDot = s.IndexOf("..");
        var lastIndexOfDotDot = s.LastIndexOf("..");

        if (!(firstIndexOfDotDot == lastIndexOfDotDot))
            return false;

        if (!T.TryParse(s[1..firstIndexOfDotDot].ToString(), provider, out var from))
            return false;

        if (!T.TryParse(s[(firstIndexOfDotDot + 2)..^1].ToString(), provider, out var to))
            return false;

        bool? fromIncluded =
            s[0] switch
            {
                '[' => true,
                '(' => false,
                _ => null
            };

        bool? toIncluded =
            s[^1] switch
            {
                ']' => true,
                ')' => false,
                _ => null
            };

        if (fromIncluded is null || toIncluded is null)
            return false;

        var lowerBound = new Bound<T>(from, fromIncluded.Value);
        var upperBound = new Bound<T>(to, toIncluded.Value);

        result = new BoundedInterval<T>(lowerBound, upperBound);

        return true;
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