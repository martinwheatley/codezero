using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace category_theory;

internal readonly struct Value<T>
    where T : struct, IMinValue<T>, IMaxValue<T>
{
    private readonly T _value;

    public Value(T value)
    {
        _value = value;
    }

    public static implicit operator Value<T>(T value) => new(value);
}

internal static class Value
{
    public static Value<T> Create<T>(T value)
        where T : struct, IMinValue<T>, IMaxValue<T> => new(value);
}

internal interface IRangeValue<T> where T : struct, IMinValue<T>, IMaxValue<T>
{
    T Value { get; }
}

internal interface IMinValue<T>
{
    T MinValue { get; }
}

internal interface IMaxValue<T>
{
    T MaxValue { get; }
}

internal interface IValueIncrementor<T> where T : IComparable<T>
{
    T GetNext(T value);
}

internal sealed class Range<T> : IReadOnlySet<T>
    where T : struct, IComparable<T>
{
    private readonly T _start;
    private readonly T _end;
    private readonly Func<T, T> _nextValueGenerator;
    private readonly Lazy<IReadOnlyList<T>> _values;

    public Range(T start, T end, Func<T, T> nextValueGenerator)
    {
        _start = start;
        _end = end;
        _nextValueGenerator = nextValueGenerator;
        
        _values =
            new(() =>
            {
                var temp = start;
                var list = new List<T> { temp };

                while (temp.CompareTo(end) < 0)
                {
                    var next = nextValueGenerator(temp);

                    if (next.CompareTo(temp) < 0)
                        throw new ArgumentException($"{nameof(nextValueGenerator)} must generate strictly increasing values.", nameof(nextValueGenerator));

                    if (next.CompareTo(end) < 0)
                        list.Add(next);
                    
                    temp = next;
                }

                list.Add(end);

                return list;
            });
    }

    public bool IsValid { get; }
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

public sealed class IntIncrementor : IValueIncrementor<int>
{
    private readonly Func<int, int> _nextIntGenerator;

    public IntIncrementor(Func<int, int> nextIntGenerator)
    {
        _nextIntGenerator = nextIntGenerator;
    }

    public int GetNext(int value) => _nextIntGenerator(value);
}

//public readonly struct DateOnlyValue : IRangeValue<DateOnly>
//{
//    public DateOnlyValue(DateOnly value)
//    {
//        Value = value;
//    }

//    public DateOnly MinValue => DateOnly.MinValue;
//    public DateOnly MaxValue => DateOnly.MaxValue;
//    public DateOnly Value { get; }

//    public static implicit operator DateOnlyValue(DateOnly value) => new(value);
//}