using System;
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
