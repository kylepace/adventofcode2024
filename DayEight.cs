using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2024;

public static class AntinodeDetector
{
    public static char[][] Parse(string puzzle) =>
        puzzle.Split(Environment.NewLine).Select(s => s.ToArray()).ToArray();

    public static char[][] MapAntinodes(char[][] map)
    {
        var toReturn = map.Select(c => (char[])c.Clone()).ToArray();

        for (var i = 0; i < map.Length; i++)
        {
            for (var j = 0; j < map[i].Length; j++)
            {
                for (var x = 0; x < map.Length; x++)
                {
                    for (var y = 0; y < map[x].Length; y++)
                    {
                        if (map[i][j] != '.' &&
                            map[i][j] == map[x][y] &&
                            !(i == x && j == y))
                        {
                            var antix = i + (i - x);
                            var antiy = j + (j - y);
                            if (antix >= 0 && antix < map.Length &&
                                antiy >= 0 && antiy < map[i].Length)
                            {
                                toReturn[antix][antiy] = '#';
                            }
                        }
                    }
                }
            }
        }

        return toReturn;
    }

    public static char[][] MapAntinodes_2(char[][] map)
    {
        var toReturn = map.Select(c => (char[])c.Clone()).ToArray();

        for (var i = 0; i < map.Length; i++)
        {
            for (var j = 0; j < map[i].Length; j++)
            {
                for (var x = 0; x < map.Length; x++)
                {
                    for (var y = 0; y < map[x].Length; y++)
                    {
                        if (map[i][j] != '.' &&
                            map[i][j] == map[x][y] &&
                            !(i == x && j == y))
                        {
                            var count = 1;
                            var harmonize = true;
                            toReturn[i][j] = '#';
                            while (harmonize)
                            {
                                var antix = i + (i - x) * count;
                                var antiy = j + (j - y) * count;
                                if (antix >= 0 && antix < map.Length &&
                                    antiy >= 0 && antiy < map[i].Length)
                                {
                                    toReturn[antix][antiy] = '#';
                                    count++;
                                }
                                else
                                {
                                    harmonize = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        return toReturn;
    }
}

public class DayEightTests
{
    [Test]
    public void ExampleTest()
    {
        var puzzleInput = @"............
........0...
.....0......
.......0....
....0.......
......A.....
............
............
........A...
.........A..
............
............";

        var parsed = AntinodeDetector.Parse(puzzleInput);

        var antinodes = AntinodeDetector.MapAntinodes(parsed)
            .SelectMany(a => a).Count(a => a == '#');
        
        Assert.AreEqual(14, antinodes);
    }

    [Test]
    public async Task FirstInputTest()
    {
        var fileInput = await File.ReadAllTextAsync(@"Input/DayEightInput.txt");

        var parsed = AntinodeDetector.Parse(fileInput);

        var antinodes = AntinodeDetector.MapAntinodes(parsed)
            .SelectMany(a => a).Count(a => a == '#');

        Assert.NotNull(antinodes);
    }

    [Test]
    public void SecondExampleTest()
    {
        var puzzleInput = @"............
........0...
.....0......
.......0....
....0.......
......A.....
............
............
........A...
.........A..
............
............";

        var parsed = AntinodeDetector.Parse(puzzleInput);

        var antinodes = AntinodeDetector.MapAntinodes_2(parsed)
            .SelectMany(a => a).Count(a => a == '#');

        Assert.AreEqual(34, antinodes);
    }

    [Test]
    public async Task SecondInputTest()
    {
        var fileInput = await File.ReadAllTextAsync(@"Input/DayEightInput.txt");

        var parsed = AntinodeDetector.Parse(fileInput);

        var antinodes = AntinodeDetector.MapAntinodes_2(parsed)
            .SelectMany(a => a).Count(a => a == '#');

        Assert.NotNull(antinodes);
    }
}