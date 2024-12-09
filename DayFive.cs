using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2024;

public static class ManualOrderer
{
    public static (IEnumerable<IEnumerable<int>> rules, IEnumerable<IEnumerable<int>> updates) ParseInstructions(string ins)
    {
        var lines = ins.Split(Environment.NewLine);
        var rules = lines
            .Where(s => s.Contains("|"))
            .Select(s => s.Split("|").Select(s => int.Parse(s)));
            
        var updates = lines
            .Where(s => s.Contains(",")) 
            .Select(s => s.Split(",").Select(s => int.Parse(s)));

        return (rules, updates);    
    }

    public static (bool valid, IEnumerable<(int, int)> toSwap) IsValid(IEnumerable<IEnumerable<int>> rules, IEnumerable<int> update)
    {
        var visited = new List<int>();
        var valid = true;
        var fixes = new List<(int, int)>();
        foreach (var page in update)
        {
            if (!visited.Contains(page))
            {
                visited.Add(page);
            }
            
            var foundRules = rules.Where(r => r.Last() == page);
            
            foreach (var foundRule in foundRules)
            {
                if (update.Contains(foundRule.First()) && !visited.Contains(foundRule.First()))
                {
                    fixes.Add((foundRule.First(), page));
                    valid = false;
                }
            }
        }

        return (valid, fixes);
    }

    public static IEnumerable<int> FixUpdate(IEnumerable<IEnumerable<int>> rules, IList<int> update, IEnumerable<(int, int)> toSwap)
    {
        foreach (var swap in toSwap)
        {
            var firstIndex = update.IndexOf(swap.Item1);
            var secondIndex = update.IndexOf(swap.Item2);
            update[firstIndex] = swap.Item2;
            update[secondIndex] = swap.Item1;
        }

        var isValid = IsValid(rules, update);

        return isValid.valid ? update : FixUpdate(rules, update, isValid.toSwap.Take(1));
    }

    public static (IEnumerable<IEnumerable<int>> validUpdates, IEnumerable<IEnumerable<int>> invalidUpdates)
        Classify(IEnumerable<IEnumerable<int>> rules, IEnumerable<IEnumerable<int>> potentialUpdates)
    {
        var validUpdates = new List<IEnumerable<int>>();
        var invalidUpdates = new List<IEnumerable<int>>();

        foreach (var update in potentialUpdates)
        {
            var isValid = IsValid(rules, update);

            if (isValid.valid)
            {
                validUpdates.Add(update);
            }
            else
            {
                invalidUpdates.Add(FixUpdate(rules, update.ToList(), isValid.toSwap));
            }
        }

        return (validUpdates, invalidUpdates);
    }

    public static (int validSums, int invalidSums) FindMiddlePageSum(string input)
    {
        var (rules, updates) = ParseInstructions(input);
        
        var (validUpdates, invalidUpdates) = Classify(rules, updates);

        return (
            validUpdates.Select(vu => vu.ElementAt(vu.Count() / 2)).Sum(),
            invalidUpdates.Select(vu => vu.ElementAt(vu.Count() / 2)).Sum());
    }
}

public class DayFiveTests
{
    private readonly string _example =  @"47|53
97|13
97|61
97|47
75|29
61|13
75|53
29|13
97|29
53|29
61|53
97|53
61|29
47|13
75|47
97|75
47|61
75|61
47|29
75|13
53|13

75,47,61,53,29
97,61,53,29,13
75,29,13
75,97,47,61,53
61,13,29
97,13,75,29,47";

    [Test]
    public void OrderingTest()
    {
        var result = ManualOrderer.ParseInstructions(_example);
    
        Assert.AreEqual(21, result.Item1.Count());
        Assert.AreEqual(6, result.Item2.Count());
    }

    [Test]
    public void FirstExampleTest()
    {
        var validExpected = 143;
        var invalidExpected = 123;

        var actual = ManualOrderer.FindMiddlePageSum(_example);

        Assert.AreEqual(validExpected, actual.validSums);
        Assert.AreEqual(invalidExpected, actual.invalidSums);
    }

    [Test, Ignore("Too slow.")]
    public async Task FirstTest()
    {
        var fileInput = await File.ReadAllTextAsync(@"Input/DayFiveInput.txt");
        
        var result = ManualOrderer.FindMiddlePageSum(fileInput);
        
        Assert.NotNull(result.validSums);
        Assert.NotNull(result.invalidSums);
    }
}