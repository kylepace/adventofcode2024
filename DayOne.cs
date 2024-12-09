using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2024;

public class ListDistanceCalculator
{
    public static int FindDistanceRookie(IEnumerable<int> list1, IEnumerable<int> list2)
    {
        var totalDistance = 0;
        var sorted1 = list1.OrderBy(l => l);
        var sorted2 = list2.OrderBy(l => l);
        for (var i = 0; i < list1.Count(); i++)
        {
            var locationId1 = sorted1.ElementAt(i);
            var locationId2 = sorted2.ElementAt(i);

            totalDistance += Math.Abs(locationId1 - locationId2);
        }
        return totalDistance;
    }

    public static int FindDistanceElegant(IEnumerable<int> list1, IEnumerable<int> list2) => 
        list1.OrderBy(l => l)
            .Zip(list2.OrderBy(l => l), (x, y) => Math.Abs(x - y))
            .Sum();

    public static async Task<(IEnumerable<int>, IEnumerable<int>)> ParseInputIntoListsRookieAsync(string filePath)
    {
        var fileInput = await File.ReadAllLinesAsync(filePath);
        var list1 = new List<int>();
        var list2 = new List<int>();
        var stringSplitDelimitor = new string [] { "  " };
        foreach (var line in fileInput)
        {
            var locationIds = line.Split(stringSplitDelimitor, StringSplitOptions.None);
            list1.Add(int.Parse(locationIds[0]));
            list2.Add(int.Parse(locationIds[1]));
        }

        return (list1, list2);
    }

    public static int FindSimilarityScoreRookie(IEnumerable<int> list1, IEnumerable<int> list2)
    {
        var similarityScore = 0;
        foreach (var locationId in list1)
        {
            var similarity = list2.Where(x => x == locationId).Count();
            similarityScore += locationId * similarity;
        }
        return similarityScore;
    }

    public static int FindSimilarityScoreElegant(IEnumerable<int> list1, IEnumerable<int> list2) =>
        list1.Select(l => l * list2.Where(x => x == l).Count()).Sum();
}

public class DayOneTests
{
    [Test]
    public async Task DistanceDayOneInputTest()
    {
        var (list1, list2) = await ListDistanceCalculator.ParseInputIntoListsRookieAsync(@"Input/DayOneInput.txt");

        var distanceRookie = ListDistanceCalculator.FindDistanceRookie(list1, list2);
        var distanceElegant = ListDistanceCalculator.FindDistanceElegant(list1, list2);

        Assert.AreEqual(distanceRookie, distanceElegant);
    }

    [Test]
    public void DistanceExampleTest()
    {
        var list1 = new [] { 3, 4, 2, 1, 3, 3 };
        var list2 = new [] { 4, 3, 5, 3, 9, 3 };

        const int exampleDistance = 11;
        var distance = ListDistanceCalculator.FindDistanceRookie(list1, list2);
        Assert.AreEqual(exampleDistance, distance);

        var elegantDistance = ListDistanceCalculator.FindDistanceElegant(list1, list2);
        Assert.AreEqual(exampleDistance, elegantDistance);
    }

    [Test]
    public void SimilarityExampleTest()
    {
        var list1 = new [] { 3, 4, 2, 1, 3, 3 };
        var list2 = new [] { 4, 3, 5, 3, 9, 3 };

        const int exampleSimilarity = 31;
        var similarity = ListDistanceCalculator.FindSimilarityScoreRookie(list1, list2);
        Assert.AreEqual(exampleSimilarity, similarity);

        var elegantSimilarity = ListDistanceCalculator.FindSimilarityScoreElegant(list1, list2);
        Assert.AreEqual(exampleSimilarity, elegantSimilarity);
    }

  [Test]
    public async Task SimilarityDayOneInputTest()
    {
        var (list1, list2) = await ListDistanceCalculator.ParseInputIntoListsRookieAsync(@"Input/DayOneInput.txt");

        var similarityRookie = ListDistanceCalculator.FindSimilarityScoreRookie(list1, list2);
        var similarityElegant = ListDistanceCalculator.FindSimilarityScoreElegant(list1, list2);

        Assert.AreEqual(similarityRookie, similarityElegant);
    }
}