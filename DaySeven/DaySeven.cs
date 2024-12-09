using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2024.DaySeven;

public class BridgeRepair
{
    public static IEnumerable<IEnumerable<long>> ParseInput(string input) =>
        input.Split(Environment.NewLine)
            .Select(line => line.Split(" ").Skip(1).Select(r => long.Parse(r)).Prepend(long.Parse(line.Split(":")[0])));

    public static IEnumerable<long> FindPossibilities(long num, long[] vals)
    {
        if (vals.Length == 0)
        {
            return [num];
        }

        return FindPossibilities(num + vals.First(), vals.Skip(1).ToArray())
            .Concat(FindPossibilities(num * vals.First(), vals.Skip(1).ToArray()));
    }

    public static bool HasValidSolution(IEnumerable<long> input)
    {
        var values = input.Skip(1);
        var expected = input.ElementAt(0);

        var possibilities = FindPossibilities(values.ElementAt(0), values.Skip(1).ToArray());

        return possibilities.Any(p => p == expected);
    }

    public static long SumResults(IEnumerable<IEnumerable<long>> inputs) =>
        inputs.Where(HasValidSolution).Sum(vs => vs.ElementAt(0));
}

public class DaySevenTests
{
    [Test]
    public void ExampleTest()
    {
        var puzzleInput = @"190: 10 19
3267: 81 40 27
83: 17 5
156: 15 6
7290: 6 8 6 15
161011: 16 10 13
192: 17 8 14
21037: 9 7 18 13
292: 11 6 16 20";

        var parsed = BridgeRepair.ParseInput(puzzleInput);

        Assert.AreEqual(9, parsed.Count());

        var sum = BridgeRepair.SumResults(parsed);

        Assert.AreEqual(3749, sum);
    }

    [Test]
    public async Task FirstInputTest()
    {
        var fileInput = await File.ReadAllTextAsync(@"DaySeven/DaySevenInput.txt");
        
        var parsed = BridgeRepair.ParseInput(fileInput);

        var sum = BridgeRepair.SumResults(parsed);
        
        Assert.NotNull(sum);
    }
}