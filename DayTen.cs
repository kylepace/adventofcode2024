using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2024;

public class DayTenTests
{
    public int[][] ParseTrail(string trailheads) =>
        trailheads.Split(Environment.NewLine)
            .Select(l => l.ToCharArray().Select(c => int.Parse(c.ToString())).ToArray()).ToArray();

    public IList<(int, int)?> FindTrails(int[][] mtn, int x, int y)
    {
        var el = mtn[x][y];

        if (el == 9)
        {
            return [(x, y)];
        }

        var acc = new List<(int, int)?>();
        if (x - 1 >= 0 && mtn[x - 1][y] == el + 1)
        {
            acc.AddRange(FindTrails(mtn, x - 1, y));
        }

        if (x + 1 < mtn.Length && mtn[x + 1][y] == el + 1)
        {
            acc.AddRange(FindTrails(mtn, x + 1, y));
        }

        if (y - 1 >= 0 && mtn[x][y - 1] == el + 1)
        {
            acc.AddRange(FindTrails(mtn, x, y - 1));
        }

        if (y + 1 < mtn[x].Length && mtn[x][y + 1] == el + 1)
        {
            acc.AddRange(FindTrails(mtn, x, y + 1));
        }

        return acc;
    }

    public int ScoreTrail(int[][] mtn, int x, int y) =>
        FindTrails(mtn, x, y).Where(i => i.HasValue).Distinct().Count();

    public int RateTrail(int[][] mtn, int x, int y) =>
        FindTrails(mtn, x, y).Where(i => i.HasValue).Count();

    public (int score, int rating) ScoreTrails(string trailheads)
    {
        var mtn = ParseTrail(trailheads);

        var trailScores = new List<(int score, int rating)>();

        for (var i = 0; i < mtn.Length; i++)
        {
            for (var j = 0; j < mtn[i].Length; j++)
            {
                if (mtn[i][j] == 0)
                {
                    trailScores.Add((ScoreTrail(mtn, i, j), RateTrail(mtn, i, j)));
                }
            }
        }
    
        return (trailScores.Sum(i => i.score), trailScores.Sum(i => i.rating));
    }

    [Test]
    public void ExampleTest()
    {
        var terrain = @"89010123
78121874
87430965
96549874
45678903
32019012
01329801
10456732";

        var trailheadScore = ScoreTrails(terrain);

        Assert.AreEqual(36, trailheadScore.score);

        Assert.AreEqual(81, trailheadScore.rating);
    }

    [Test]
    public async Task InputTest()
    {
        var fileInput = await File.ReadAllTextAsync(@"Input/DayTenInput.txt");

        var trailheadScore = ScoreTrails(fileInput);

        Assert.NotNull(trailheadScore.score);
        Assert.NotNull(trailheadScore.rating);
    }
}