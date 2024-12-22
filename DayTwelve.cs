using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2024;

public class DayTwelveTests
{
    public static char[][] Parse(string puzzle) =>
        puzzle.Split(Environment.NewLine).Select(s => s.ToArray()).ToArray();

    public record Point(int X, int Y);
    public IList<Point> Directions = [new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1)];

    public void MapRegion(char[][] garden, IList<Point> region, List<Point> visited, int r, int c)
    {
        var point = new Point(r, c);

        if (visited.Any(v => v.X == r && v.Y == c))
        {
            return;
        }
        visited.Add(point);
        region.Add(point);
        
        foreach (var dir in Directions)
        {
            var vec = new Point(dir.X + r, dir.Y + c);
            if (vec.X >= 0 && vec.X < garden.Length && vec.Y >= 0 && vec.Y < garden[vec.X].Length &&
                garden[vec.X][vec.Y] == garden[r][c])
            {
                MapRegion(garden, region, visited, vec.X, vec.Y);
            }
        }
    }

    public long TotalPrice(string input)
    {
        var garden = Parse(input);
        var visited = new List<Point>();
        var regions = new List<IList<Point>>();

        for (var x = 0; x < garden.Length; x++)
        {
            for (var y = 0; y < garden[x].Length; y++)
            {
                if (visited.Any(p => p.X == x && p.Y == y))
                {
                    continue;
                }
                var region = new List<Point>();
                MapRegion(garden, region, visited, x, y);
                regions.Add(region);
            }
        }

        var total = 0;
        foreach (var region in regions)
        {
            var tot = 0;
            foreach (var point in region)
            {
                var adjacent = region
                    .Count(p => 
                        (p.X == point.X && (point.Y + 1 == p.Y || point.Y - 1 == p.Y)) ||
                        (p.Y == point.Y && (point.X + 1 == p.X || point.X - 1 == p.X)));

                var fenceLength = 4 - adjacent;
                tot += fenceLength;
            }
        
            total += region.Count * tot;
        }

        return total;
    }

    [Test]
    public void ExampleTest()
    {
        var example = @"AAAA
BBCD
BBCC
EEEC";

        Assert.AreEqual(140, TotalPrice(example));

        var example3 = @"OOOOO
OXOXO
OOOOO
OXOXO
OOOOO";

        Assert.AreEqual(772, TotalPrice(example3));

        var example2 = @"RRRRIICCFF
RRRRIICCCF
VVRRRCCFFF
VVRCCCJFFF
VVVVCJJCFE
VVIVCCJJEE
VVIIICJJEE
MIIIIIJJEE
MIIISIJEEE
MMMISSJEEE";

        Assert.AreEqual(1930, TotalPrice(example2));
    }

    [Test]
    public async Task InputTest()
    {
        var input = await File.ReadAllTextAsync(@"Input/DayTwelveInput.txt");

        Assert.AreEqual(1450816, TotalPrice(input));
    }
}