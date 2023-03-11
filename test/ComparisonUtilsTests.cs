using CodeZero.Core;
using Shouldly;

namespace category_theory_tests
{
    public class ComparisonUtilsTests
    {
        [Fact]
        public void Test_ComparisonUtils()
        {
            var a = 1;
            var b = 2;
            
            var gt = a.IsGreaterThan(b);
            var lt = a.IsLessThan(b);
            var gte = a.IsGreaterThanOrEqualTo(b);
            var lte = a.IsLessThanOrEqualTo(b);

            gt.ShouldBeFalse();
            lt.ShouldBeTrue();
            gte.ShouldBeFalse();
            lte.ShouldBeTrue();
        }
    }
}