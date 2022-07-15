using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace category_theory;
internal static class Chapter1
{
    public static T Id<T>(T x) => x;

    public static Func<A, C> Compose<A, B, C>(Func<A, B> f, Func<B, C> g) => a => g(f(a));
}