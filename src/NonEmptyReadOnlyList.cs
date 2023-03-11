using System.Collections;

namespace CodeZero.Core;

internal sealed class NonEmptyReadOnlyList<T> : IReadOnlyList<T>
{
    private readonly IReadOnlyList<T> _values;

    public NonEmptyReadOnlyList(T value)
        : this(new T[1] { value })
    {
    }

    public NonEmptyReadOnlyList(IReadOnlyList<T> values)
    {
        if (values is [])
        if (values is null || values.Count is 0 || values[0] is null)
            throw new Exception("");

        _values = values.ToArray();
        Head = _values[0];
    }

    public T Head { get; }
    public T this[int index] => _values[index];
    public T this[Index index] => _values[index];
    public int Count => _values.Count;

    public IEnumerator<T> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();
}
