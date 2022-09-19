using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace category_theory;

internal static class Chapter2
{

}

internal class Memoize<T1, T2>
    where T1 : IEquatable<T1>
{
    private readonly Func<T1, T2> _fallbackFunc;
    private readonly IReadOnlyDictionary<T1, T2> _memory;

    public Memoize(Func<T1, T2> fallbackFunc)
    {
        _fallbackFunc = fallbackFunc;
        _memory = new Dictionary<T1, T2>();
    }

    public T2 Evaluate(T1 t) =>
        _memory.ContainsKey(t)
            ? _memory[t]
            : _fallbackFunc(t);
}

internal sealed class NonEmptyReadOnlyList<T> : IReadOnlyList<T>
{
    private readonly IReadOnlyList<T> _values;

    public NonEmptyReadOnlyList(T value)
        : this(new T[1] { value })
    {
    }

    public NonEmptyReadOnlyList(IReadOnlyList<T> values)
    {
        if (values is null || values.Count is 0 || values[0] is null)
            throw new Exception("");

        _values = values.ToArray();
        First = _values[0];
    }

    public T First { get; }
    public T this[int index] => _values[index];
    public T this[Index index] => _values[index];
    public int Count => _values.Count;

    public IEnumerator<T> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();
}
