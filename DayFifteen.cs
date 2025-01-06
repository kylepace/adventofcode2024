using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2024;

public enum Move
{
    Up,
    Down,
    Left,
    Right
}

public class DayFifteenTests
{
    public Move ToMove(char input) => input switch
    {
        '<' => Move.Left,
        '>' => Move.Right,
        '^' => Move.Up,
        _ => Move.Down,
    };

    public (char[][] grid, Move[] moves) Parse(string input)
    {
        var grid = new List<char[]>();
        var moves = new List<Move>();
        var lines = input.Split(Environment.NewLine);

        var parsingGrid = true;
        foreach (var line in lines)
        {
            if (line.Length == 0)
            {
                parsingGrid = false;
                continue;
            }

            if (parsingGrid)
            {
                grid.Add(line.ToCharArray());
            }
            else
            {
                moves.AddRange(line.ToCharArray().Select(ToMove));
            }
        }
        
        return (grid.ToArray(), moves.ToArray());
    }

    public void PrintGrid(char[][] grid)
    {
        var toPrint = string.Empty;
        for (var i = 0; i < grid.Length; i++)
        {
            for (var j = 0; j < grid[i].Length; j++)
            {
                if (grid[i][j] == '@')
                {
                    toPrint += '@';
                }
                else
                {
                    toPrint += grid[i][j];
                }
            }
            toPrint += Environment.NewLine;
        }
        Console.WriteLine(toPrint);
    }

    public (int i, int j) FindRobot(char[][] grid)
    {
        for (var i = 0; i < grid.Length; i++)
        {
            for (var j = 0; j < grid[i].Length; j++)
            {
                if (grid[i][j] == '@')
                {
                    return (i, j);
                }
            }
        }
        return (0, 0);
    }

    public (int i, int j) ToVector(Move move) => move switch
    {
        Move.Up => (-1, 0),
        Move.Down => (1, 0),
        Move.Left => (0, -1),
        Move.Right => (0, 1),
        _ => throw new NotImplementedException()
    };

    public bool CanPush(char[][] grid, (int i, int j) currentPos, (int i, int j) vector)
    {
        var nextI = currentPos.i + vector.i;
        var nextJ = currentPos.j + vector.j;

        if (grid[nextI][nextJ] == '#')
        {
            return false;
        }

        if (grid[nextI][nextJ] == '.' || CanPush(grid, (nextI, nextJ), vector))
        {
            grid[nextI][nextJ] = grid[currentPos.i][currentPos.j];
            return true;
        }

        return false;
    }

    public void ApplyMoves(char[][] grid, Move[] moves)
    {
        var robotPos = FindRobot(grid);
        
        foreach (var move in moves)
        {
            var (vecI, vecJ) = ToVector(move);
            var (newI, newJ) = (robotPos.i + vecI, robotPos.j + vecJ);
            if (grid[newI][newJ] == '#')
            {
                continue;
            }

            if (grid[newI][newJ] == '.')
            {
                grid[robotPos.i][robotPos.j] = '.';
                grid[newI][newJ] = '@';
                robotPos = (newI, newJ);
            }

            if (grid[newI][newJ] == 'O')
            {
                if (CanPush(grid, (newI, newJ), (vecI, vecJ)))
                {
                    grid[robotPos.i][robotPos.j] = '.';
                    grid[newI][newJ] = '@';
                    robotPos = (newI, newJ);
                }
            }
        }
    }

    public int SumGps(char[][] grid)
    {
        var toReturn = 0;
        for (var i = 0; i < grid.Length; i++)
        {
            for (var j = 0; j < grid[i].Length; j++)
            {
                if (grid[i][j] == 'O' || grid[i][j] == '[')
                {
                    toReturn += (100 * i) + j;
                }
            }
        }
        return toReturn;
    }

    public List<char> ExplodeTile(char tile) => tile switch
    {
        '#' => ['#', '#'],
        'O' => ['[', ']'],
        '.' => ['.', '.'],
        _ => ['@', '.'],
    };

    public char[][] Explode(char[][] grid)
    {
        var toReturn = new List<List<char>>();

        for (var i = 0; i < grid.Length; i++)
        {
            var line = new List<char>();
            for (var j = 0; j < grid[i].Length; j++)
            {
                line.AddRange(ExplodeTile(grid[i][j]));
            }    
            toReturn.AddRange(line);
        }

        return toReturn.Select(c => c.ToArray()).ToArray();
    }
    
    public bool CanPushVertical(char[][] grid, (int i, int j)[] boxPos, (int i, int j) vector)
    {
        var leftI = boxPos[0].i + vector.i;
        var leftJ = boxPos[0].j + vector.j;
        var rightI = boxPos[1].i + vector.i;
        var rightJ = boxPos[1].j + vector.j;

        if (grid[leftI][leftJ] == '#' || grid[rightI][rightJ] == '#')
        {
            return false;
        }

        if ((grid[leftI][leftJ] == '.' && grid[rightI][rightJ] == '.') ||
            (grid[leftI][leftJ] == '.' && grid[rightI][rightJ] == '[' && CanPushVertical(grid, [(rightI, rightJ), (rightI, rightJ + 1)], vector)) ||
            (grid[leftI][leftJ] == '[' && CanPushVertical(grid, [(leftI, leftJ), (rightI, rightJ)], vector)) ||
            (grid[leftI][leftJ] == ']' && grid[rightI][rightJ] == '.' && CanPushVertical(grid, [(leftI, leftJ - 1), (leftI, leftJ)], vector)) ||
            (grid[leftI][leftJ] == ']' && grid[rightI][rightJ] == '[' &&
                CanPushVertical(grid, [(leftI, leftJ - 1), (leftI, leftJ)], vector) &&
                CanPushVertical(grid, [(rightI, rightJ), (rightI, rightJ + 1)], vector)))
        {
            return true;
        }

        return false;
    }

    public void Swap(char[][] grid, (int i, int j)[] boxPos, (int i, int j) vector)
    {
        grid[boxPos[0].i + vector.i][boxPos[0].j + vector.j] = grid[boxPos[0].i][boxPos[0].j];
        grid[boxPos[1].i + vector.i][boxPos[1].j + vector.j] = grid[boxPos[1].i][boxPos[1].j];

        grid[boxPos[0].i][boxPos[0].j] = '.';
        grid[boxPos[1].i][boxPos[1].j] = '.';
    }

    public bool SwapVertical(char[][] grid, (int i, int j)[] boxPos, (int i, int j) vector)
    {
        var leftI = boxPos[0].i + vector.i;
        var leftJ = boxPos[0].j + vector.j;
        var rightI = boxPos[1].i + vector.i;
        var rightJ = boxPos[1].j + vector.j;

        if (grid[leftI][leftJ] == '#' || grid[rightI][rightJ] == '#')
        {
            return false;
        }
    
        if ((grid[leftI][leftJ] == '.' && grid[rightI][rightJ] == '.') ||
            (grid[leftI][leftJ] == '.' && grid[rightI][rightJ] == '[' &&SwapVertical(grid, [(rightI, rightJ), (rightI, rightJ + 1)], vector)) ||
            (grid[leftI][leftJ] == ']' && grid[rightI][rightJ] == '.' && SwapVertical(grid, [(leftI, leftJ - 1), (leftI, leftJ)], vector)) ||
            (grid[leftI][leftJ] == '[' && SwapVertical(grid, [(leftI, leftJ), (rightI, rightJ)], vector)) ||
            (grid[leftI][leftJ] == ']' && grid[rightI][rightJ] == '[' &&
                SwapVertical(grid, [(leftI, leftJ - 1), (leftI, leftJ)], vector) &&
                SwapVertical(grid, [(rightI, rightJ), (rightI, rightJ + 1)], vector)))
        {
            Swap(grid, boxPos, vector);
            return true;
        }

        return false;
    }

    public void ApplyBigBoxMoves(char[][] grid, Move[] moves)
    {
        var robotPos = FindRobot(grid);
        
        foreach (var move in moves)
        {
            var (vecI, vecJ) = ToVector(move);
            var (newI, newJ) = (robotPos.i + vecI, robotPos.j + vecJ);
            var newChar = grid[newI][newJ];

            if (newChar == '#')
            {
                continue;
            }

            if (newChar == '.')
            {
                grid[robotPos.i][robotPos.j] = '.';
                grid[newI][newJ] = '@';
                robotPos = (newI, newJ);
            }

            if (newChar == '[' || newChar == ']')
            {
                if (vecI == 0)
                {
                    if (CanPush(grid, (newI, newJ), (vecI, vecJ)))
                    {
                        grid[robotPos.i][robotPos.j] = '.';
                        grid[newI][newJ] = '@';
                        robotPos = (newI, newJ);
                    }
                }

                if (vecJ == 0)
                {
                    if (newChar == ']' && CanPushVertical(grid, [(newI, newJ - 1), (newI, newJ)], (vecI, vecJ)))
                    {
                        SwapVertical(grid, [(newI, newJ - 1), (newI, newJ)], (vecI, vecJ));
                        grid[robotPos.i][robotPos.j] = '.';
                        grid[newI][newJ] = '@';
                        robotPos = (newI, newJ);
                        grid[newI][newJ - 1] = '.';
                    }
                    else if (newChar == '[' && CanPushVertical(grid, [(newI, newJ), (newI, newJ + 1)], (vecI, vecJ)))
                    {
                        SwapVertical(grid, [(newI, newJ), (newI, newJ + 1)], (vecI, vecJ));
                        grid[robotPos.i][robotPos.j] = '.';
                        grid[newI][newJ] = '@';
                        robotPos = (newI, newJ);
                        grid[newI][newJ + 1] = '.';
                    }
                }
            }
        }
    }

    [Test]
    public void SmallExampleTest()
    {
        var input = @"########
#..O.O.#
##@.O..#
#...O..#
#.#.O..#
#...O..#
#......#
########

<^^>>>vv<v>>v<<";

        var (grid, moves) = Parse(input);

        ApplyMoves(grid, moves);

        Assert.AreEqual(2028, SumGps(grid));
    }

    [Test]
    public void LargeExampleTest()
    {
var input = @"##########
#..O..O.O#
#......O.#
#.OO..O.O#
#..O@..O.#
#O#..O...#
#O..O..O.#
#.OO.O.OO#
#....O...#
##########

<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
>^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^";

        var (grid, moves) = Parse(input);

        var explodeGrid = Explode(grid);

        ApplyMoves(grid, moves);

        Assert.AreEqual(10092, SumGps(grid));

        ApplyBigBoxMoves(explodeGrid, moves);

        Assert.AreEqual(9021, SumGps(explodeGrid));
    }

    [Test]
    public async Task InputTest()
    {
        var input = await File.ReadAllTextAsync(@"Input/DayFifteenInput.txt");

        var (grid, moves) = Parse(input);

        var explodeGrid = Explode(grid);

        ApplyMoves(grid, moves);

        Assert.AreEqual(1463715, SumGps(grid));

        ApplyBigBoxMoves(explodeGrid, moves);

        Assert.AreEqual(1481392, SumGps(explodeGrid));
    }
}