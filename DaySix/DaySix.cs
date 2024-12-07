using System;
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

    public static void PlotCourse(char[][] area, char dir, int x, int y)
    {
        var stuck = false;
        
        while (!stuck)
        {
            area[x][y] = 'X';
            if (LeavingMap(area, dir, x, y))
            {
                stuck = true;
                continue;
            }

            if (dir == 'U')
            {
                if (area[x - 1][y] == '.' || area[x - 1][y] == 'X')
                {
                    x--;
                }
                else
                {
                    dir = 'R';
                    y++;
                }
            }
            else if (dir == 'D')
            {
                if (area[x + 1][y] == '.' || area[x + 1][y] == 'X')
                {
                    x++;
                }
                else
                {
                    dir = 'L';
                    y--;
                }
            }
            else if (dir == 'R')
            {
                if (area[x][y + 1] == '.' || area[x][y + 1] == 'X')
                {
                    y++;
                }
                else
                {
                    dir = 'D';
                    x++;
                }
            }
            else // L
            {
                if (area[x][y - 1] == '.' || area[x][y - 1] == 'X')
                {
                    y--;
                }
                else
                {
                    dir = 'U';
                    x--;
                }
            }
        }
    }

    public static int CountChars(char[][] area, char toCount) =>
        area.SelectMany(c => c.Where(ch => ch == toCount)).Count();

    public static int CountSteps(char[][] area)
    {
        var (x, y) = FindStart(area);

        PlotCourse(area, 'U', x, y);

        return CountChars(area, 'X');
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
        var totalSteps = GuardMapper.CountSteps(puzzleArea);
        Assert.AreEqual(6, totalSteps);
    }
}