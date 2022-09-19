namespace category_theory;

public class Program
{
    public static int Main()
    {
        var range = GetRange();

        int[] invalidArr1 = null!;
        var invalidArr2 = Array.Empty<int>();
        var validArr1 = new int[1] { 1 };
        var validArr2 = Enumerable.Range(0, 10).ToList();

        var nonEmptyValid1 = new NonEmptyReadOnlyList<int>(1); // should NOT throw
        var nonEmptyValid2 = new NonEmptyReadOnlyList<int>(validArr1); // should NOT throw
        var nonEmptyValid3 = new NonEmptyReadOnlyList<int>(validArr2); // should NOT throw

        var nonEmptyInvalid1 = new NonEmptyReadOnlyList<int?>((int?)null); // should throw
        var nonEmptyInvalid2 = new NonEmptyReadOnlyList<int>(invalidArr1); // should throw
        var nonEmptyInvalid3 = new NonEmptyReadOnlyList<int>(invalidArr2); // should throw

        foreach (var item in nonEmptyValid1)
        {
            _ = item.Equals(nonEmptyValid1.First);
        }

        var d = 5.3m;
        var id = Chapter1.Id(d);

        var comp = Chapter1.Compose<decimal, decimal, decimal>(
            dec => Chapter1.Id(dec),
            dec => Chapter1.Id(dec));

        var result = comp(d);

        var mem = new Memoize<int, DateOnly>(i => DateOnly.FromDayNumber(i));

        _ = mem.Evaluate(5);

        Console.WriteLine(id.Equals(d));

        return 0;
    }

    internal static Range<int> GetRange()
    {
        return
            new Range<int>(
                0, 
                20, 
                x => x + (x % 2 == 0 ? 1 : x + 2));
    }
}