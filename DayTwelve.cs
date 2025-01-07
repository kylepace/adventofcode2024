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
    public IList<Point> Directions = [new Point(0, 1), new Point(1, 0), new Point(0, -1), new Point(-1, 0)];
    
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

    public (long total, long discount) TotalPrice(string input)
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

        var discount = 0;

        foreach (var region in regions)
        {
            var tot = 0;
            foreach (var point in region)
            {
                var up = new Point(point.X - 1, point.Y);
                var down = new Point(point.X + 1, point.Y);
                var left = new Point(point.X, point.Y - 1);
                var right = new Point(point.X, point.Y + 1);

                if (!region.Any(p => p == up) && !region.Any(p => p == right))
                {
                    tot++;
                }

                if (!region.Any(p => p == right) && !region.Any(p => p == down))
                {
                    tot++;
                }
                
                if (!region.Any(p => p == down) && !region.Any(p => p == left))
                {
                    tot++;
                }
                
                if (!region.Any(p => p == left) && !region.Any(p => p == up))
                {
                    tot++;
                }

                var upRight = new Point(point.X - 1, point.Y + 1);
                var downRight = new Point(point.X + 1, point.Y + 1);
                var downLeft = new Point(point.X + 1, point.Y - 1);
                var upLeft = new Point(point.X - 1, point.Y - 1);

                if (region.Any(p => p == up) && region.Any(p => p == right) && !region.Any(p => p == upRight))
                {
                    tot++;
                }
                
                if (region.Any(p => p == right) && region.Any(p => p == down) && !region.Any(p => p == downRight))
                {
                    tot++;
                }
                
                if (region.Any(p => p == down) && region.Any(p => p == left) && !region.Any(p => p == downLeft))
                {
                    tot++;
                }
                
                if (region.Any(p => p == left) && region.Any(p => p == up) && !region.Any(p => p == upLeft))
                {
                    tot++;
                }
            }

            discount += tot * region.Count;
        }

        return (total, discount);
    }

    [Test]
    public void ExampleTest()
    {
        var example = @"AAAA
BBCD
BBCC
EEEC";

        var exampleSol = TotalPrice(example);
        Assert.AreEqual(140, exampleSol.total);

        Assert.AreEqual(80, exampleSol.discount);

        Console.WriteLine("--------");

        var example3 = @"OOOOO
OXOXO
OOOOO
OXOXO
OOOOO";

        var example3Sol = TotalPrice(example3);
        Assert.AreEqual(772, example3Sol.total);
        Assert.AreEqual(436, example3Sol.discount);

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

        Assert.AreEqual(1930, TotalPrice(example2).total);
        Assert.AreEqual(1206, TotalPrice(example2).discount);
    }

    [Test, Ignore("Slow")]
    public async Task InputTest()
    {
        var input = await File.ReadAllTextAsync(@"Input/DayTwelveInput.txt");

        var inputSol = TotalPrice(input);
        Assert.AreEqual(1450816, inputSol.total);
        Assert.AreEqual(865662, inputSol.discount);
    }
}