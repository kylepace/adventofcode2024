using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2024;

public record struct Point(long X, long Y);
public record struct Vector(long X, long Y);
public class Robot(Point CurrentPoint, Vector V)
{
    public Point CurrentPoint { get; set; } = CurrentPoint;
    public Vector V { get; } = V;
}

public class Grid
{
    private readonly Robot[] _robots;
    private readonly int _width;
    private readonly int _height;

    public Grid(int width, int height, Robot[] robots)
    {
        _width = width;
        _height = height;
        _robots = robots;
    }

    public void SecondElapsed()
    {
        foreach (var robot in _robots)
        {
            var newX = robot.CurrentPoint.X + robot.V.X;
            var newY = robot.CurrentPoint.Y + robot.V.Y;

            if (newX < 0)
            {
                newX += _width;
            }

            if (newX >= _width)
            {
                newX -= _width;
            }

            if (newY < 0)
            {
                newY += _height;
            }

            if (newY >= _height)
            {
                newY -= _height;
            }
            robot.CurrentPoint = new Point(newX, newY);
        }
    }

    public int CalculateSafetyFactor()
    {
        var halfWidth = _width / 2;
        var halfHeight = _height / 2;

        var q1 = _robots
            .Count(r => 
                r.CurrentPoint.X < halfWidth &&
                r.CurrentPoint.Y < halfHeight
            );
        
        var q2 = _robots
            .Count(r => 
                r.CurrentPoint.X > halfWidth &&
                r.CurrentPoint.Y < halfHeight
            );

        var q3 = _robots
            .Count(r => 
                r.CurrentPoint.X < halfWidth &&
                r.CurrentPoint.Y > halfHeight
            );

        var q4 = _robots
            .Count(r => 
                r.CurrentPoint.X > halfWidth &&
                r.CurrentPoint.Y > halfHeight
            );

        return q1 * q2 * q3 * q4;
    }

    public string Draw()
    {
        var grid = string.Empty;
        
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                var robots = _robots.Where(r => r.CurrentPoint.X == j && r.CurrentPoint.Y == i).Count();
                if (robots > 0)
                {
                    grid += robots.ToString();
                }
                else
                {
                    grid += ".";
                }
            }
            grid += Environment.NewLine;
        }

        return grid;
    }

    public bool CheckTree()
    {
        var groupings = _robots.GroupBy(r => r.CurrentPoint.X);
        var groupCount = groupings.Count(g => g.Count() > 30);
        var xGroupings = _robots.GroupBy(r => r.CurrentPoint.Y);
        var yGroupCount = xGroupings.Count(g => g.Count() > 20);
        return groupCount >= 2 && yGroupCount >= 2;
    }
}

public class DayFourteenTests
{
    public Robot[] Parse(string input)
    {
        var robots = new List<Robot>();
        var lines = input.Split(Environment.NewLine);
        foreach (var line in lines)
        {
            var pair = line
                .Split(" ")
                .Select(p => p.Split("=").Last().Split(',').Select(int.Parse));
            var point = new Point(pair.First().First(), pair.First().Last());
            var vector = new Vector(pair.Last().First(), pair.Last().Last());
            robots.Add(new Robot(point, vector));
        }

        return robots.ToArray();
    }

    [Test]
    public void ExampleTestPartOne()
    {
        var input = @"p=0,4 v=3,-3
p=6,3 v=-1,-3
p=10,3 v=-1,2
p=2,0 v=2,-1
p=0,0 v=1,3
p=3,0 v=-2,-2
p=7,6 v=-1,-3
p=3,0 v=-1,-2
p=9,3 v=2,3
p=7,3 v=-1,2
p=2,4 v=2,-3
p=9,5 v=-3,-3";

        var robots = Parse(input);
        var grid = new Grid(11, 7, robots);

        var seconds = 100;

        for (var i = 0; i < seconds; i++)
        {
            grid.SecondElapsed();
        }

        Assert.AreEqual(12, grid.CalculateSafetyFactor());
    }

    [Test]
    public async Task InputPartOne()
    {
        var fileInput = await File.ReadAllTextAsync(@"Input/DayFourteen.txt");

        var grid = new Grid(101, 103, Parse(fileInput));

        var seconds = 100;
        for (var i = 0; i < seconds; i++)
        {
            grid.SecondElapsed();
        }

        Assert.IsNotNull(grid.CalculateSafetyFactor());
    }

    [Test, Ignore("Too fancy.")]
    public async Task EasterEggTest()
    {
        var input = await File.ReadAllTextAsync(@"Input/DayFourteen.txt");

        var robots = Parse(input);
        var grid = new Grid(101, 103, robots);

        for (var i = 0; i < 10000; i++)
        {
            grid.SecondElapsed();
            if (grid.CheckTree())
            {
                var gridString = grid.Draw();
                Console.Write(gridString);
                Console.WriteLine($"------------{i}-------------");
            }
        }
    }
}