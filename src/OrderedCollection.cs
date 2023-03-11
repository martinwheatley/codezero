using System.Collections;
using System.Numerics;

namespace CodeZero.Core;

internal sealed class OrderedCollection<T> : IReadOnlySet<T>
    where T : struct, IComparable<T>, IEquatable<T>, IComparisonOperators<T, T, bool>, IEqualityOperators<T, T, bool>
{
    private readonly Func<T, T> _nextValueGenerator;
    private readonly Lazy<IReadOnlyList<T>> _values;

    public OrderedCollection(T from, T to, Func<T, T> nextValueGenerator)
    {
        From = from;
        To = to;

        _nextValueGenerator = nextValueGenerator;

        _values =
            new(() =>
            {
                var prev = from;
                var list = new List<T> { prev };

                while (prev < to)
                {
                    var next = nextValueGenerator(prev);

                    if (next < prev)
                        throw new ArgumentException($"{nameof(nextValueGenerator)} must generate strictly increasing values.", nameof(nextValueGenerator));

                    if (next < to)
                        list.Add(next);

                    prev = next;
                }

                list.Add(to);

                return list;
            });
    }

    public T From { get; }
    public T To { get; }
    public int Count => _values.Value.Count;

    public bool Contains(T item) => throw new NotImplementedException();
    public IEnumerator<T> GetEnumerator()
    {
        foreach (var element in _values.Value)
            yield return element;
    }

    public bool IsProperSubsetOf(IEnumerable<T> other) => throw new NotImplementedException();
    public bool IsProperSupersetOf(IEnumerable<T> other) => throw new NotImplementedException();
    public bool IsSubsetOf(IEnumerable<T> other) => throw new NotImplementedException();
    public bool IsSupersetOf(IEnumerable<T> other) => throw new NotImplementedException();
    public bool Overlaps(IEnumerable<T> other) => throw new NotImplementedException();
    public bool SetEquals(IEnumerable<T> other) => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
}