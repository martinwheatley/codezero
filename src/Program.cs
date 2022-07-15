namespace category_theory;

public class Program
{
    public static int Main()
    {
        var d = 5.3m;
        var id = Chapter1.Id(d);

        var comp = Chapter1.Compose<decimal, decimal, decimal>(
            dec => Chapter1.Id(dec),
            dec => Chapter1.Id(dec));

        var result = comp(d);

        Console.WriteLine(id.Equals(d));

        return 0;
    }
}