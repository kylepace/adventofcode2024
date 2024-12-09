using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2024;

public partial class CorruptedMultiplier
{
    [GeneratedRegex(@"mul\((\d+),(\d+)\)", RegexOptions.IgnoreCase)]
    private static partial Regex MulRegex();
    [GeneratedRegex(@"(\d+)", RegexOptions.IgnoreCase)]
    private static partial Regex DigitRegex();
    [GeneratedRegex(@"mul\((\d+),(\d+)\)|don\'t\(\)|do\(\)", RegexOptions.IgnoreCase)]
    private static partial Regex DoAndDontRegex();

    public static int MultiplyValidEntries(string corr) =>
        MulRegex().Matches(corr)
            .Select(m => MultiplyEntries(m.Value))
            .Sum();

    private static int MultiplyEntries(string entry) =>
        DigitRegex().Matches(entry).Aggregate(1, (x, y) => x * int.Parse(y.Value));

    public static int MultiplyWithDoAndDont(string corr)
    {
        var shouldMultiply = true;
        var toReturn = 0;
        var matches = DoAndDontRegex().Matches(corr);
        foreach (Match match in matches)
        {
            var val = match.Value;
            if (shouldMultiply && val.StartsWith("mul"))
            {
                toReturn += MultiplyEntries(val);
            }
            else if (val.StartsWith("don"))
            {
                shouldMultiply = false;
            }
            else if (val.StartsWith("do"))
            {
                shouldMultiply = true;
            }
        }
        
        return toReturn;
    }

    public static int MultiplyWithDoAndDont_Two(string corr) =>
        DoAndDontRegex().Matches(corr)
            .Aggregate((true, 0), (x, y) =>
                y.Value.StartsWith("mul")
                    ? (x.Item1, x.Item2 + (x.Item1 ? MultiplyEntries(y.Value) : 0))
                    : (!y.Value.StartsWith("don"), x.Item2)).Item2;
}

public class DayThreeTests
{
    [Test]
    public void CorruptedExampleTest()
    {
        var corruptedExample = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))";

        const int exampleValue = 161;

        Assert.AreEqual(exampleValue, CorruptedMultiplier.MultiplyValidEntries(corruptedExample));
    }

    [Test]
    public async Task CorruptedInputTest()
    {
        var fileInput = await File.ReadAllTextAsync(@"Input/DayThreeInput.txt");

        var result = CorruptedMultiplier.MultiplyValidEntries(fileInput);

        Assert.NotNull(result);
    }

    [Test]
    public void DoAndDontExampleTest()
    {
        var corruptedExample = "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";

        const int exampleValue = 48;

        Assert.AreEqual(exampleValue, CorruptedMultiplier.MultiplyWithDoAndDont(corruptedExample));
    }

    [Test]
    public async Task DoAndDontInputTest()
    {
        var fileInput = await File.ReadAllTextAsync(@"Input/DayThreeInput.txt");

        var result = CorruptedMultiplier.MultiplyWithDoAndDont(fileInput);

        var result2 = CorruptedMultiplier.MultiplyWithDoAndDont_Two(fileInput);

        Assert.AreEqual(result, result2);
    }
}