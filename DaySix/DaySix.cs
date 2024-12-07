using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2024.DaySix;

public static class GuardMapper
{
    public static (int x, int y) FindStart(char[][] area)
    {
        for (var i = 0; i < area.Length; i++)
        {
            for (var j = 0; j < area[i].Length; j++)
            {
                if (area[i][j] == '^')
                {
                    return (i, j);
                }
            }
        }

        return (0, 0);
    }

    public static bool LeavingMap(char[][] area, char dir, int x, int y) =>
        (dir == 'U' && x == 0) ||
        (dir == 'D' && x + 1 == area.Length) ||
        (dir == 'R' && y + 1 == area[x].Length) ||
        (dir == 'L' && y == 0);

    public static bool PlotCourse(char[][] area, char dir, int x, int y)
    {
        var stuck = false;
        var stepsTaken = 0;
        var maxStepsTaken = 50000;
        while (!stuck && stepsTaken < maxStepsTaken)
        {
            area[x][y] = 'X';
            if (LeavingMap(area, dir, x, y))
            {
                stuck = true;
                continue;
            }

            switch (dir)
            {
                case 'U':
                    if (area[x - 1][y] == '#')
                    {
                        dir = 'R';
                        y++;
                    }
                    else
                    {
                        x--;
                    }
                    break;
                case 'D':
                    if (area[x + 1][y] == '#')
                    {
                        dir = 'L';
                        y--;
                        
                    }
                    else
                    {
                        x++;
                    }
                    break;
                case 'R':
                    if (area[x][y + 1] == '#')
                    {
                        dir = 'D';
                        x++;
                    }
                    else
                    {
                        y++;
                    }
                    break;
                case 'L':
                    if (area[x][y - 1] == '#')
                    {
                        dir = 'U';
                        x--;
                    }
                    else
                    {
                        y--;
                    }
                    break;
            }
            stepsTaken++;
        }
        
        return stepsTaken == maxStepsTaken;
    }

    public static int CountChars(char[][] area, char toCount) =>
        area.SelectMany(c => c.Where(ch => ch == toCount)).Count();

    public static int CountSteps(char[][] area)
    {
        var (x, y) = FindStart(area);

        PlotCourse(area, 'U', x, y);

        return CountChars(area, 'X');
    }

    public static IEnumerable<char[][]> GenerateBoards(char[][] area)
    {
        var toReturn = new List<char[][]>();
        for (var i = 0; i < area.Length; i++)
        {
            for (var j = 0; j < area[i].Length; j++)
            {
                if (area[i][j] == '.')
                {
                    var board = area.Select(c => (char[])c.Clone()).ToArray();
                    board[i][j] = '#';
                    toReturn.Add(board);
                }
            }
        }

        return toReturn;
    }

    public static int CountParadoxes(char[][] area)
    {
        var count = 0;
        var (x, y) = FindStart(area);
        
        foreach (var board in GenerateBoards(area))
        {
            var loopedOut = PlotCourse(board, 'U', x, y);
            if (loopedOut)
            {
                count++;
            }
        }
        
        return count;

    }
}

public class DaySixTests
{
    [Test]
    public void ExampleTest()
    {
        var puzzleInput = @"....#.....
.........#
..........
..#.......
.......#..
..........
.#..^.....
........#.
#.........
......#...";
        var puzzleArea = puzzleInput.Split(Environment.NewLine).Select(f => f.ToArray()).ToArray();
        var totalSteps = GuardMapper.CountSteps(puzzleArea);
        Assert.AreEqual(41, totalSteps);
    }

    [Test]
    public async Task InputTest()
    {
        var fileInput = await File.ReadAllTextAsync(@"DaySix/DaySixInput.txt");

        var puzzleArea = fileInput.Split(Environment.NewLine).Select(f => f.ToArray()).ToArray();

        var totalSteps = GuardMapper.CountSteps(puzzleArea);

        Assert.NotNull(totalSteps);
    }

    [Test]
    public void PartTwoInfiniteLoopCheck()
    {
        var puzzleInput = @"....#.....
.........#
..........
..#.......
.......#..
..........
.#.#^.....
........#.
#.........
......#...";

        var puzzleArea = puzzleInput.Split(Environment.NewLine).Select(f => f.ToArray()).ToArray();

        var (x, y) = GuardMapper.FindStart(puzzleArea);

        var timedOut = GuardMapper.PlotCourse(puzzleArea, 'U', x, y);

        Assert.IsTrue(timedOut);
    }

    [Test]
    public void PartTwoExampleTest()
    {
        var puzzleInput = @"....#.....
.........#
..........
..#.......
.......#..
..........
.#..^.....
........#.
#.........
......#...";
        var puzzleArea = puzzleInput.Split(Environment.NewLine).Select(f => f.ToArray()).ToArray();
        var validParadoxes = GuardMapper.CountParadoxes(puzzleArea);
        Assert.AreEqual(6, validParadoxes);
    }

    [Test, Ignore("Takes 2.5 seconds.")]
    public async Task PartTwoInputTest()
    {
        var fileInput = await File.ReadAllTextAsync(@"DaySix/DaySixInput.txt");

        var puzzleArea = fileInput.Split(Environment.NewLine).Select(f => f.ToArray()).ToArray();

        var validParadoxes = GuardMapper.CountParadoxes(puzzleArea);

        Assert.NotNull(validParadoxes);
    }
}