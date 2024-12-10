using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2024;

public class DayNineTests
{
    public List<long?> BuildDisk(string input)
    {
        var toReturn = new List<long?>();
        var nums = input.ToCharArray().Select(i => int.Parse(i.ToString())); // dont like this
        
        long counter = 0;
        for (var i = 0; i < nums.Count(); i++)
        {
            if (i % 2 == 0)
            {
                for (var b = 0; b < nums.ElementAt(i); b++)
                {
                    toReturn.Add(counter);
                }

                if (i < nums.Count() - 1)
                {
                    for (var free = 0; free < nums.ElementAt(i + 1); free++)
                    {
                        toReturn.Add(null);
                    }
                    counter++;
                }
            }
        }

        return toReturn;
    }

    public IList<long?> Defrag(IList<long?> disk)
    {
        var usedBlocks = disk.Count(d => d.HasValue);
        var defragged = new List<long?>(disk);

        for (var blockIdx = disk.Count - 1; blockIdx >= 0; blockIdx--)
        {
            var firstFreeSpace = defragged.IndexOf(null);
            if (firstFreeSpace == usedBlocks)
            {
                break;
            }
            defragged[firstFreeSpace] = disk[blockIdx];
            defragged[blockIdx] = null;
        }

        return defragged;
    }

    public IList<long?> DefragContinuous(List<long?> disk)
    {
        var maxBlockIdx = disk.Max();
        
        for (var b = maxBlockIdx; b >= 0; b--)
        {
            var currentIdx = disk.IndexOf(b);
            var lastIdx = disk.LastIndexOf(b);
            var blockSize = lastIdx - currentIdx + 1;

            var blockCounter = 0;
            for (var i = 0; i < disk.Count; i++)
            {
                if (!disk[i].HasValue)
                {
                    if (i >= currentIdx)
                    {
                        break;
                    }
                    
                    blockCounter++;
                    if (blockCounter == blockSize)
                    {
                        for (var j = 0; j < blockCounter; j++)
                        {
                            disk[i - j] = b;
                            disk[lastIdx - j] = null;
                        }

                        break;
                    }
                }
                else
                {
                    blockCounter = 0;
                }
            }
        }
        
        return disk;
    }

    public long Checksum(IList<long?> disk) =>
        disk.Select((b, i) => b.HasValue ? b!.Value * i : 0).Sum();

    [Test, Ignore("Look above, it's slow.")]
    public async Task Test()
    {
        var puzzleInput = "2333133121414131402";

        var disk = BuildDisk(puzzleInput);

        var defragged = Defrag(disk);

        var checksum = Checksum(defragged);
        
        Assert.AreEqual(1928, checksum);

        var continuousDefrag = DefragContinuous(disk);

        var continuousChecksum = Checksum(continuousDefrag);

        Assert.AreEqual(2858, continuousChecksum);

        var fileInput = await File.ReadAllTextAsync(@"Input/DayNineInput.txt");

        var defraggedFile = Defrag(BuildDisk(fileInput));

        var fileSolution = Checksum(defraggedFile);

        Assert.NotNull(fileSolution);

        var defraggedContinuousFile = DefragContinuous(BuildDisk(fileInput));

        var fileContinousSolution = Checksum(defraggedContinuousFile);

        Assert.NotNull(fileContinousSolution);
    }
}