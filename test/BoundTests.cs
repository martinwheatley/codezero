using CodeZero.Core;

using Shouldly;

namespace category_theory_tests;

public class BoundTests
{
    [Fact]
    public void Test_Bound_Default()
    {
        Bound<int> bound = default;

        bound.Value.ShouldBe(default);
        bound.IsIncluded.ShouldBeFalse();
    }

    [Fact]
    public void Test_Bound_Default_Ctor()
    {
        var bound = new Bound<int>();

        bound.Value.ShouldBe(default);
        bound.IsIncluded.ShouldBeFalse();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Bound_Ctor(bool included)
    {
        var value = new DateOnly(2022, 1, 1);
        var bound = new Bound<DateOnly>(value, included);

        bound.Value.ShouldBe(value);
        bound.IsIncluded.ShouldBe(included);
    }

    [Fact]
    public void Test_Bound_Comparison()
    {
        var value = new DateOnly(2022, 1, 1);
        var bound1 = new Bound<DateOnly>(value, true);
        var bound2 = new Bound<DateOnly>(value, false);

        (bound1 > bound2).ShouldBeTrue();
        (bound1 >= bound2).ShouldBeTrue();
        (bound2 > bound1).ShouldBeFalse();
        (bound2 >= bound1).ShouldBeFalse();
        bound1.CompareTo(bound2).ShouldBe(1);
        bound2.CompareTo(bound1).ShouldBe(-1);
    }

    [Fact]
    public void Test_Bound_Equality()
    {
        var value = new TimeOnly(23, 11, 15);
        var bound1 = new Bound<TimeOnly>(value, true);
        var bound2 = new Bound<TimeOnly>(value, false);

        (bound1 == bound2).ShouldBeFalse();
        (bound1 != bound2).ShouldBeTrue();
        bound1.Equals(bound2).ShouldBeFalse();
        (!bound1.Equals(bound2)).ShouldBeTrue();
    }
}
