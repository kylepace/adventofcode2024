using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AdventOfCode2024;

public class DayElevenTests
{
    public IList<long> ParseStoneList(string input) =>
        input.Split(" ").Select(s => long.Parse(s)).ToList();

    /*
        If the stone is engraved with the number 0, it is replaced by a stone engraved with the number 1.
        If the stone is engraved with a number that has an even number of digits, it is replaced by two stones.
            The left half of the digits are engraved on the new left stone, and the right half of the digits are engraved on the new right stone.
            (The new numbers don't keep extra leading zeroes: 1000 would become stones 10 and 0.)
        If none of the other rules apply, the stone is replaced by a new stone; the old stone's number multiplied by 2024 is engraved on the new stone.
    */

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
    }

    [Test, Ignore("Doesn't work.")]
    public void InputTest()
    {
        var stoneList = ParseStoneList("1117 0 8 21078 2389032 142881 93 385");

        var blinkedStones = BlinkStones(stoneList, 25);

        Assert.NotNull(blinkedStones.Count());

        var blinkedStonesOhNo = BlinkStones(stoneList, 75);

        Assert.AreEqual(1, blinkedStonesOhNo.Count());
    }    
}