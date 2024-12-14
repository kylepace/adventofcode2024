using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AdventOfCode2024;

public class DayElevenTests
{
    public IList<long> ParseStoneList(string input) =>
        input.Split(" ").Select(s => long.Parse(s)).ToList();

    public IList<long> BlinkStones(IList<long> stones, int timesToBlink)
    {
        var stoneLedger = new List<long>(stones);
        
        for (var i = 0; i < timesToBlink; i++)
        {
            var blinkedList = new List<long>();
            for (var j = 0; j < stoneLedger.Count(); j++)
            {
                var stone = stoneLedger[j];
                if (stone == 0)
                {
                    blinkedList.Add(1);
                    continue;
                }
                
                var stoneAsChars = stone.ToString().ToCharArray();
                if (stoneAsChars.Length % 2 == 0)
                {
                    blinkedList.Add(long.Parse(new string(stoneAsChars[..(stoneAsChars.Length / 2)])));
                    blinkedList.Add(long.Parse(new string(stoneAsChars[(stoneAsChars.Length / 2) ..])));
                }
                else
                {
                    blinkedList.Add(stone * 2024);
                }
            }
            stoneLedger = [.. blinkedList];
        }
        return stoneLedger;
    }

    private static readonly IDictionary<string, long> _blinkCache = new Dictionary<string, long>();

    private static Func<long, int, long> Memoize(Func<long, int, long> func)
    {
        return (arg, arg2) =>
        {
            var key = arg.ToString() + " " + arg2.ToString();
            if (!_blinkCache.TryGetValue(key, out long res))
            {
                res = func(arg, arg2);

                _blinkCache.Add(key, res);
            }

            return res;
        };
    }

    public long MemoizedBlink(long stone, int step) => Memoize(BlinkStone)(stone, step);

    public long BlinkStone(long stone, int step)
    {
        if (step == 0)
        {
            return 1;
        }

        if (stone == 0)
        {
            return MemoizedBlink(1, step - 1);
        }

        var stoneAsChars = stone.ToString().ToCharArray();
        if (stoneAsChars.Length % 2 == 0)
        {
            return MemoizedBlink(long.Parse(new string(stoneAsChars[..(stoneAsChars.Length / 2)])), step - 1) +
                    MemoizedBlink(long.Parse(new string(stoneAsChars[(stoneAsChars.Length / 2) ..])), step - 1);
        }

        return MemoizedBlink(stone * 2024, step - 1);
    }

    public long BlinkStonesFaster(long[] stones, int timesToBlink) =>
        stones.Sum(s => MemoizedBlink(s, timesToBlink));

    [Test]
    public void ExampleTest()
    {
        var example = "0 1 10 99 999";

        var stoneList = ParseStoneList(example);

        var blinkedStones = BlinkStones(stoneList, 1);

        var expectedList = "1 2024 1 0 9 9 2021976";

        Assert.AreEqual(7, blinkedStones.Count());

        Assert.AreEqual(expectedList, string.Join(' ', blinkedStones.Select(s => s.ToString())));

        var blinkTwentyFive = BlinkStones(new List<long> { 125, 17 }, 25);

        Assert.AreEqual(55312, blinkTwentyFive.Count());

        var fasterBlink = BlinkStonesFaster([125, 17 ], 25);

        Assert.AreEqual(55312, fasterBlink);
    }

    [Test]
    public void InputTest()
    {
        var stoneList = ParseStoneList("1117 0 8 21078 2389032 142881 93 385");

        var blinkedStones = BlinkStones(stoneList, 25);

        Assert.NotNull(blinkedStones.Count());

        var blinkStonesMore = BlinkStonesFaster([.. stoneList], 75);

        Assert.NotNull(blinkStonesMore);
    }    
}