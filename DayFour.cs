using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2024;

public static class XmasCrosswordParser
{
    public static char[][] Parse(string puzzle) =>
        puzzle.Split(Environment.NewLine).Select(l => l.ToArray()).ToArray();

    public static int CountXmas_First(char[][] p)
    {
        var toReturn = 0;

        for (var i = 0; i < p.Length; i++)
        {
            for (var j = 0; j < p[i].Length; j++)
            {
                if (p[i][j] == 'X')
                {
                    if (j - 3 >= 0 && p[i][j - 1] == 'M' && p[i][j - 2] == 'A' && p[i][j - 3] == 'S')
                    {
                        toReturn++;
                    }

                    if (j + 3 <= p[i].Length - 1 && p[i][j + 1] == 'M' && p[i][j + 2] == 'A' && p[i][j + 3] == 'S')
                    {
                        toReturn++;
                    }

                    if (i - 3 >= 0 && p[i - 1][j] == 'M' && p[i - 2][j] == 'A' && p[i - 3][j] == 'S')
                    {
                        toReturn++;
                    }

                    if (i + 3 <= p.Length - 1 && p[i + 1][j] == 'M' && p[i + 2][j] == 'A' && p[i + 3][j] == 'S')
                    {
                        toReturn++;
                    }

                    if (i - 3 >= 0 && j - 3 >= 0 &&
                        p[i - 1][j - 1] == 'M' && p[i - 2][j - 2] == 'A' && p[i - 3][j - 3] == 'S')
                    {
                        toReturn++;
                    }

                    if (i - 3 >= 0 && j + 3 <= (p[i].Length - 1) &&
                        p[i - 1][j + 1] == 'M' && p[i - 2][j + 2] == 'A' && p[i - 3][j + 3] == 'S')
                    {
                        toReturn++;
                    }

                    if (i + 3 <= p.Length - 1 && j - 3 >= 0 &&
                        p[i + 1][j - 1] == 'M' && p[i + 2][j - 2] == 'A' && p[i + 3][j - 3] == 'S')
                    {
                        toReturn++;
                    }

                    if (i + 3 <= p.Length - 1 && j + 3 <= p[i].Length - 1 &&
                        p[i + 1][j + 1] == 'M' && p[i + 2][j + 2] == 'A' && p[i + 3][j + 3] == 'S')
                    {
                        toReturn++;
                    }
                }
            }
        }
        return toReturn;
    }

    public static int CountX_mas_First(char[][] p)
    {
        var toReturn = 0;

        for (var i = 0; i < p.Length; i++)
        {
            for (var j = 0; j < p[i].Length; j++)
            {
                if (p[i][j] == 'A')
                {
                    if (i - 1 >= 0 && i + 1 <= p.Length - 1 &&
                        j - 1 >= 0 && j + 1 <= p[i].Length - 1 &&
                        ((p[i - 1][j - 1] == 'M' && p[i + 1][j + 1] == 'S') || (p[i - 1][j - 1] == 'S' && p[i + 1][j + 1] == 'M')) &&
                        ((p[i + 1][j - 1] == 'M' && p[i - 1][j + 1] == 'S') || (p[i + 1][j - 1] == 'S' && p[i - 1][j + 1] == 'M')))
                    {
                        toReturn++;
                    }
                }
            }
        }
        return toReturn;
    }
}

public class DayFourTests
{
    [Test]
    public void XmasCrosswordExampleTest()
    {
        var example = @"MMMSXXMASM
MSAMXMSMSA
AMXSXMAAMM
MSAMASMSMX
XMASAMXAMM
XXAMMXXAMA
SMSMSASXSS
SAXAMASAAA
MAMMMXMMMM
MXMXAXMASX";

        var parsedExample = XmasCrosswordParser.Parse(example);

        Assert.AreEqual(10, parsedExample.Length);

        var expectedXmasOcc = 18;
        var actualXmasOcc = XmasCrosswordParser.CountXmas_First(parsedExample);

        Assert.AreEqual(expectedXmasOcc, actualXmasOcc);
    }

    [Test]
    public async Task CrosswordInputTest()
    {
        var fileInput = await File.ReadAllTextAsync(@"Input/DayFourInput.txt");

        var puzzle = XmasCrosswordParser.Parse(fileInput);

        var numXmasai = XmasCrosswordParser.CountXmas_First(puzzle);
        // 2633
        Assert.NotNull(numXmasai);
    }

    [Test]
    public void X_masCrosswordExampleTest()
    {
        var example = @"MMMSXXMASM
MSAMXMSMSA
AMXSXMAAMM
MSAMASMSMX
XMASAMXAMM
XXAMMXXAMA
SMSMSASXSS
SAXAMASAAA
MAMMMXMMMM
MXMXAXMASX";

        var parsedExample = XmasCrosswordParser.Parse(example);

        var expectedXmasOcc = 9;
        var actualXmasOcc = XmasCrosswordParser.CountX_mas_First(parsedExample);

        Assert.AreEqual(expectedXmasOcc, actualXmasOcc);
    }

    [Test]
    public async Task CrosswordX_MasInputTest()
    {
        var fileInput = await File.ReadAllTextAsync(@"Input/DayFourInput.txt");

        var puzzle = XmasCrosswordParser.Parse(fileInput);

        var numXmasai = XmasCrosswordParser.CountX_mas_First(puzzle);
        // 1936
        Assert.NotNull(numXmasai);
    }
}