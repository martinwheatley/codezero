using category_theory;

using Shouldly;

namespace category_theory_tests;

public class BoundedIntervalTests
{
    [Fact]
    public void Test_BoundedInterval_Default()
    {
        BoundedInterval<int> def = default;
        var interval = new BoundedInterval<int>();

        def.ShouldBe(interval);
        interval.To.ShouldBe(default);
        interval.From.ShouldBe(default);
        interval.IsDefault.ShouldBeTrue();
        interval.IsSingeton.ShouldBeFalse();
        interval.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void Test_BoundedInterval_Exclusive_SameBound()
    {
        var bound = Bound<decimal>.Exclusive(2.03m);

        var interval = new BoundedInterval<decimal>(bound, bound);
        
        interval.IsEmpty.ShouldBeTrue();
        interval.IsDefault.ShouldBeFalse();
        interval.IsSingeton.ShouldBeFalse();
        interval.From.ShouldBe(bound);
        interval.To.ShouldBe(bound);
    }

    [Fact]
    public void Test_BoundedInterval_Exclusive_DifferetBounds()
    {
        var lower = Bound<decimal>.Exclusive(2.03m);
        var upper = Bound<decimal>.Exclusive(2.04m);

        var interval = new BoundedInterval<decimal>(lower, upper);

        interval.IsEmpty.ShouldBeFalse();
        interval.IsDefault.ShouldBeFalse();
        interval.IsSingeton.ShouldBeFalse();
        interval.From.ShouldBe(lower);
        interval.To.ShouldBe(upper);
    }

    [Fact]
    public void Test_BoundedInterval_LeftClosed_RightOpen_SameBound()
    {
        var lower = Bound<decimal>.Inclusive(2.03m);
        var upper = Bound<decimal>.Exclusive(2.03m);

        var interval = new BoundedInterval<decimal>(lower, upper);

        interval.IsEmpty.ShouldBeTrue();
        interval.IsDefault.ShouldBeFalse();
        interval.IsSingeton.ShouldBeFalse();
        interval.From.ShouldBe(lower);
        interval.To.ShouldBe(upper);
    }

    [Fact]
    public void Test_BoundedInterval_LeftOpen_RightClosed_SameBound()
    {
        var lower = Bound<decimal>.Exclusive(2.03m);
        var upper = Bound<decimal>.Inclusive(2.03m);

        var interval = new BoundedInterval<decimal>(lower, upper);

        interval.IsEmpty.ShouldBeTrue();
        interval.IsDefault.ShouldBeFalse();
        interval.IsSingeton.ShouldBeFalse();
        interval.From.ShouldBe(lower);
        interval.To.ShouldBe(upper);
    }

    [Fact]
    public void Test_BoundedInterval_LeftClosed_RightClosed_SameBound()
    {
        var lower = Bound<decimal>.Inclusive(2.03m);
        var upper = Bound<decimal>.Inclusive(2.03m);

        var interval = new BoundedInterval<decimal>(lower, upper);

        interval.IsEmpty.ShouldBeFalse();
        interval.IsDefault.ShouldBeFalse();
        interval.IsSingeton.ShouldBeTrue();
        interval.From.ShouldBe(lower);
        interval.To.ShouldBe(upper);
    }

    [Fact]
    public void Test_BoundedInterval_T_Ctor()
    {
        var from = new DateOnly(2020, 1, 1);
        var to = new DateOnly(2022, 1, 1);

        var interval = new BoundedInterval<DateOnly>(from, to);

        interval.To.Value.ShouldBe(to);
        interval.From.Value.ShouldBe(from);
        interval.IsEmpty.ShouldBeFalse();
        interval.IsDefault.ShouldBeFalse();
        interval.IsSingeton.ShouldBeFalse();
    }

    [Fact]
    public void Test_BoundedInterval_Primary_SameFromAndTo()
    {
        var from = new TimeOnly(11, 23, 15);

        var interval = new BoundedInterval<TimeOnly>(from, from);

        interval.To.Value.ShouldBe(from);
        interval.From.Value.ShouldBe(from);
        interval.IsEmpty.ShouldBeFalse();
        interval.IsDefault.ShouldBeFalse();
        interval.IsSingeton.ShouldBeTrue();
    }

    [Fact]
    public void Test_BoundedInterval_Primary_Ctor_Throwing()
    {
        var from = 0.5m;
        var to = 1.5m;

        _ = Should.Throw<ArgumentException>(() => new BoundedInterval<decimal>(to, from));
    }

    [Theory]
    [InlineData(-5, 0, false)]
    [InlineData(-5, 1, true)]
    [InlineData(-5, 15, true)]
    [InlineData(2, 5, true)]
    [InlineData(10, 15, true)]
    [InlineData(12, 15, false)]
    public void Test_BoundedInterval_Overlap(int from, int to, bool expOverlaps)
    {
        var i1 = new BoundedInterval<int>(1, 10);
        var i2 = new BoundedInterval<int>(from, to);
        var overlaps = i1.Overlaps(i2);

        overlaps.ShouldBe(expOverlaps);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(0.01, true)]
    [InlineData(0.025, true)]
    [InlineData(0.05, true)]
    [InlineData(1, false)]
    public void Test_BoundedInterval_Contains_Item(double item, bool expContains)
    {
        var i = new BoundedInterval<double>(0.01, 0.05);
        var contained = i.Contains(item);

        contained.ShouldBe(expContains);
    }

    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(0, 5, true)]
    [InlineData(2, 3, true)]
    [InlineData(3, 5, true)]
    [InlineData(5, 5, true)]
    [InlineData(10, 15, false)]
    [InlineData(12, 15, false)]
    public void Test_BoundedInterval_Contains_OtherInterval(byte from, byte to, bool expContains)
    {
        var i1 = new BoundedInterval<byte>(0, 5);
        var i2 = new BoundedInterval<byte>(from, to);
        var contained = i1.Contains(i2);

        contained.ShouldBe(expContains);
    }

    [Theory]
    [InlineData('a', 'b', false, default(char), default(char))]
    [InlineData('a', 'f', true, 'a', 'm')]
    [InlineData('a', 'z', true, 'a', 'z')]
    [InlineData('f', 'h', true, 'e', 'm')]
    [InlineData('k', 'z', true, 'e', 'z')]
    [InlineData('s', 'z', false, default(char), default(char))]
    public void Test_BoundedInterval_TryMerge_Other(char c1, char c2, bool expMergeSuccess, char expLower, char expUpper)
    {
        var i1 = new BoundedInterval<char>('e', 'm');
        var i2 = new BoundedInterval<char>(c1, c2);

        var canMerge = i1.TryMerge(i2, out var result);

        canMerge.ShouldBe(expMergeSuccess);
        result.From.Value.ShouldBe(expLower);
        result.To.Value.ShouldBe(expUpper);
    }

    [Fact]
    public void Test_Contains_SingletonInterval()
    {
        // THIS SHOULD BE A SINGLETON!!!
        var i = 1;
        var lower = Bound<int>.Exclusive(1);
        var upper = Bound<int>.Inclusive(2);
        var interval = new BoundedInterval<int>(lower, upper);

        interval.Contains(i).ShouldBeFalse();
    }

    [Fact]
    public void Test_Adjacency_FirstIntervalEndIsOpen()
    {
        var lb1 = new Bound<int>(1, true);
        var ub1 = new Bound<int>(5, false);

        var lb2 = new Bound<int>(5, true);
        var ub2 = new Bound<int>(10, true);

        var i1 = new BoundedInterval<int>(lb1, ub1);
        var i2 = new BoundedInterval<int>(lb2, ub2);

        i1.IsAdjacentOn(i2).ShouldBeTrue();
    }

    [Fact]
    public void Test_Adjacency_FirstIntervalEndIsClosed()
    {
        var lb1 = new Bound<int>(1, true);
        var ub1 = new Bound<int>(5, true);

        var lb2 = new Bound<int>(5, false);
        var ub2 = new Bound<int>(10, true);

        var i1 = new BoundedInterval<int>(lb1, ub1);
        var i2 = new BoundedInterval<int>(lb2, ub2);

        i1.IsAdjacentOn(i2).ShouldBeTrue();
    }

    [Fact]
    public void Test_BoundedInterval_TryParse()
    {
        var s = "[2022-01-01..2023-01-01)";
        var canParse = BoundedInterval<DateOnly>.TryParse(s, out var interval);

        canParse.ShouldBeTrue();
        interval.From.ShouldBe(new Bound<DateOnly>(new DateOnly(2022, 1, 1), true));
        interval.To.ShouldBe(new Bound<DateOnly>(new DateOnly(2023, 1, 1), false));
    }
}