using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

public static class ManualOrderer
{
    public static (IEnumerable<IEnumerable<int>>, IEnumerable<IEnumerable<int>>) ParseInstructions(string ins)
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

    public static int FindMiddlePageSum(string input)
    {
        var parsed = ParseInstructions(input);
        var rules = parsed.Item1;
        var validUpdates = new List<IEnumerable<int>>();

        foreach (var update in parsed.Item2)
        {
            var visited = new List<int>();
            var valid = true;
            foreach (var page in update)
            {
                if (!visited.Contains(page))
                {
                    visited.Add(page);
                }
                
                var foundRules = rules.Where(r => r.Last() == page);
                if (foundRules.Any())
                {
                    foreach (var foundRule in foundRules)
                    {
                        if (update.Contains(foundRule.First()) && !visited.Contains(foundRule.First()))
                        {
                            valid = false;
                        }
                    }
                }
            }

            if (valid)
            {
                validUpdates.Add(update);
            }
        }

        return validUpdates.Select(vu => vu.ElementAt(vu.Count() / 2)).Sum();
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
        var expected = 143;

        var actual = ManualOrderer.FindMiddlePageSum(_example);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public async Task FirstTest()
    {
        var fileInput = await File.ReadAllTextAsync(@"DayFive/DayFiveInput.txt");
        
        var result = ManualOrderer.FindMiddlePageSum(fileInput);

        Assert.NotNull(result);
    }
}