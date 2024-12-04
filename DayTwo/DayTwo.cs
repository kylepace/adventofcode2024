using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2024.DayTwo;

public class ReportAnalyzer
{
    public static int CountSafeLevels(IEnumerable<IEnumerable<int>> report) =>
        report.Select(AnalyzeLevel).Count(x => x == true);

    public static int CountSafeLevelsWithDampener(IEnumerable<IEnumerable<int>> report) =>
        report.Select(AnalyzeLevelWithDampener).Count(x => x == true);

    private static bool AnalyzeLevel(IEnumerable<int> level)
    {
        var previous = level.ElementAt(0);
        var isAscending = previous < level.ElementAt(1);

        for (var i = 1; i < level.Count(); i++)
        {
            var next = level.ElementAt(i);

            if ((isAscending && previous > next) ||
                (!isAscending && previous < next))
            {
                return false;
            }

            var diff = Math.Abs(previous - next);
            if (diff < 1 || diff > 3)
            {
                return false;
            }

            previous = next;
        }

        return true;
    }

    public static IEnumerable<IEnumerable<int>> FindDampenedLevels(IEnumerable<int> level)
    {
        var toReturn = new List<IEnumerable<int>>();

        for (var i = 0; i < level.Count(); i++)
        {
            var tmp = level.ToList();
            tmp.RemoveAt(i);
            toReturn.Add(tmp);
        }

        return toReturn;
    }

    private static bool AnalyzeLevelWithDampener(IEnumerable<int> level) =>
        AnalyzeLevel(level) || FindDampenedLevels(level).Any(AnalyzeLevel);

    public static async Task<IEnumerable<IEnumerable<int>>> ParseInputIntoReportAsync(string filePath)
    {
        var fileInput = await File.ReadAllLinesAsync(filePath);
        var toReturn = new List<IEnumerable<int>>();
        foreach (var line in fileInput)
        {
            var levels = line.Split(' ', StringSplitOptions.None);
            toReturn.Add(levels.Select(int.Parse));
        }

        return toReturn;
    }
}

public class DayTwoTests
{
    [Test]
    public void ReportExampleTest()
    {
        var report = new []
        {
            [7, 6, 4, 2, 1],
            [1, 2, 7, 8, 9],
            [9, 7, 6, 2, 1],
            [1, 3, 2, 4, 5],
            [8, 6, 4, 4, 1],
            new [] { 1, 3, 6, 7, 8 }
        };

        const int exampleSafeLevelCount = 2;

        var safeLevelCount = ReportAnalyzer.CountSafeLevels(report);

        Assert.AreEqual(exampleSafeLevelCount, safeLevelCount);
    }

    [Test]
    public async Task ReportInputTest()
    {
        var report = await ReportAnalyzer.ParseInputIntoReportAsync(@"DayTwo/DayTwoInput.txt");

        var result = ReportAnalyzer.CountSafeLevels(report);

        Assert.NotNull(result);
    }

    [Test]
    public void ReportWithDampenerExampleTest()
    {
        var report = new []
        {
            [7, 6, 4, 2, 1],
            [1, 2, 7, 8, 9],
            [9, 7, 6, 2, 1],
            [1, 3, 2, 4, 5],
            [8, 6, 4, 4, 1],
            new [] { 1, 3, 6, 7, 8 }
        };

        const int exampleSafeLevelCount = 4;

        var safeLevelCount = ReportAnalyzer.CountSafeLevelsWithDampener(report);

        Assert.AreEqual(exampleSafeLevelCount, safeLevelCount);
    }

    [Test]
    public async Task ReportWithDampenerInputTest()
    {
        var report = await ReportAnalyzer.ParseInputIntoReportAsync(@"DayTwo/DayTwoInput.txt");

        var safeLevelCount = ReportAnalyzer.CountSafeLevelsWithDampener(report);

        Assert.NotNull(safeLevelCount);
    }
}