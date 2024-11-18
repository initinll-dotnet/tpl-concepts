namespace ParallelLinq;

public class CustomAggregationDemo
{
    public static void Execute()
    {
        // some operations in LINQ perform an aggregation
        //var sum = Enumerable.Range(1, 1000).Sum();
        //var sum = ParallelEnumerable.Range(1, 1000).Sum();


        // Sum is just a specialized case of Aggregate
        //var sum = Enumerable.Range(1, 1000).Aggregate(0, (i, acc) => i + acc);

        // var sum = ParallelEnumerable.Range(1, 1000)
        //   .Aggregate(
        //       seed: 0, // initial seed for calculations
        //       (partialSum, i) => partialSum += i, // per task
        //       (total, subtotal) => total += subtotal, // combine all tasks
        //       i => i); // final result processing

        var sum = ParallelEnumerable.Range(1, 1000)
            .Aggregate(
                seed: 0, // Initial seed for calculations
                (partialSum, i) =>
                {
                    Console.WriteLine($"Task {Task.CurrentId}: Adding {i} to partialSum = {partialSum}");
                    partialSum += i;
                    return partialSum;
                }, // Per-task aggregation
                (total, subtotal) =>
                {
                    Console.WriteLine($"Combining subtotal {subtotal} into total = {total}");
                    total += subtotal;
                    return total;
                }, // Combining results from tasks
                i =>
                {
                    Console.WriteLine($"Final result transformation: {i}");
                    return i;
                } // Final result processing
            );

        Console.WriteLine($"Sum is {sum}");
    }
}
