using category_theory;

using Shouldly;

namespace category_theory_tests;

public class NonZeroTests
{
    [Fact]
    public void TestNonZero_Int_EmptyCtor()
    {
        var sut = new NonZero<int>();

        1.ShouldBe(sut);
    }

    [Fact]
    public void TestNonZero_Int_Default()
    {
        NonZero<int> sut = default;

        1.ShouldBe(sut);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void TestNonZero_Int_ValidCtor(int value)
    {
        var sut = new NonZero<int>(value);

        value.ShouldBe(sut);
    }

    [Fact]
    public void TestNonZero_Decimal_EmptyCtor()
    {
        var sut = new NonZero<decimal>();

        1m.ShouldBe(sut);
    }

    [Fact]
    public void TestNonZero_Decimal_Default()
    {
        NonZero<decimal> sut = default;

        1m.ShouldBe(sut);
    }

    [Theory]
    [InlineData(-5_300)]
    [InlineData(-1)]
    [InlineData(-0.000001)]
    [InlineData(0.00001)]
    [InlineData(1)]
    [InlineData(4_387)]
    public void TestNonZero_Decimal_ValidCtor(decimal value)
    {
        var sut = new NonZero<decimal>(value);

        value.ShouldBe(sut);
    }
}