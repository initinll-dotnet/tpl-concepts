namespace ParallelLinq;

public class MergeOptions
{
    public static void Execute()
    {
        var numbers = Enumerable.Range(start: 1, count: 20).ToArray();

        // FullyBuffered = all results produced before they are consumed
        // NotBuffered = each result can be consumed right after it's produced
        // Default = AutoBuffered = buffer the number of results selected by the runtime

        // producer
        ParallelQuery<double> results = numbers
            .AsParallel()
            .WithMergeOptions(ParallelMergeOptions.NotBuffered)
            .Select(x =>
            {
                var result = Math.Log10(x);
                Console.WriteLine($"Produced {result}");
                return result;
            });

        // consumer
        foreach (var result in results)
        {
            Console.WriteLine($"Consumed {result}");
        }
    }
}
