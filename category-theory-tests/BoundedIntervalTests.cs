using category_theory;

using Shouldly;

namespace category_theory_tests;

public class BoundedIntervalTests
{
    [Fact]
    public void Test_BoundedInterval_DefaultCtor()
    {
        BoundedInterval<int> def = default;
        var interval = new BoundedInterval<int>();

        def.ShouldBe(interval);
        interval.To.ShouldBe(default);
        interval.From.ShouldBe(default);
        interval.IsDefault.ShouldBe(true);
    }

    [Fact]
    public void Test_BoundedInterval_Primary_Ctor()
    {
        var from = new DateOnly(2020, 1, 1);
        var to = new DateOnly(2022, 1, 1);

        var interval = new BoundedInterval<DateOnly>(from, to);

        interval.To.ShouldBe(to);
        interval.From.ShouldBe(from);
        interval.IsDefault.ShouldBe(false);
    }

    [Fact]
    public void Test_BoundedInterval_Primary_SameFromAndTo()
    {
        var from = new TimeOnly(11, 23, 15);

        var interval = new BoundedInterval<TimeOnly>(from, from);

        interval.To.ShouldBe(from);
        interval.From.ShouldBe(from);
        interval.IsDefault.ShouldBe(false);
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
        var i = new BoundedInterval<byte>(0, 5);
        var i2 = new BoundedInterval<byte>(from, to);
        var contained = i.Contains(i2);

        contained.ShouldBe(expContains);
    }
}