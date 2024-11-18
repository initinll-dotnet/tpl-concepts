using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace ParallelLoops;

public class PartitioningDemo
{
    [Benchmark]
    public void SquareEachValue()
    {
        const int count = 100_000;
        var values = Enumerable.Range(start: 0, count: count);
        var results = new int[count];

        // automatic (framework defined) partition of the source data
        Parallel.ForEach(source: values, body: x =>
        {
            results[x] = (int)Math.Pow(x, 2);
        });
    }

    [Benchmark]
    public void SquareEachValueChunked()
    {
        /*
        Partitioner.Create(0, 20, 5) splits the range [0, 20] into smaller subranges:
            Partition 1 -> (startIndex: 0, endIndex: 5)
            Partition 2 -> (startIndex: 5, endIndex: 10)
            Partition 3 -> (startIndex: 10, endIndex: 15)
            Partition 4 -> (startIndex: 15, endIndex: 20)

        Each partition is a tuple (startIndex, endIndex).
        */

        const int count = 100_000;
        var values = Enumerable.Range(start: 0, count: count);
        var results = new int[count];

        // manual partition of the source data
        OrderablePartitioner<Tuple<int, int>> partitions = Partitioner.Create(
            fromInclusive: 0,
            toExclusive: count,
            rangeSize: 10_000); // rangeSize = size of each subrange

        Parallel.ForEach(source: partitions, body: partition =>
        {
            var startIndex = partition.Item1;
            var endIndex = partition.Item2;

            // process each partition parallely
            for (int i = startIndex; i < endIndex; i++)
            {
                results[i] = (int)Math.Pow(i, 2);
            }
        });
    }

    public static void Execute()
    {
        var summary = BenchmarkRunner.Run<PartitioningDemo>();
        Console.WriteLine(summary);
    }
}
