namespace ParallelLoops;

public class ThreadLocalStorageDemo
{
    private readonly Random random = new Random();

    public static void Execute()
    {
        // add numbers from 1 to 100
        int sum = 0;

        Parallel.For(
            fromInclusive: 1,
            toExclusive: 1001,
            body: x =>
            {
                Console.Write($"[{x}] t={Task.CurrentId}\t");
                Interlocked.Add(ref sum, x); // concurrent access to sum from all these threads is inefficient
                                             // all tasks can add up their respective values, then add sum to total sum
            });

        Console.WriteLine($"\nSum of 1..100 = {sum}");

        sum = 0;

        Parallel.For(
            fromInclusive: 1,
            toExclusive: 1001,
            // parallelOptions: new ParallelOptions { MaxDegreeOfParallelism = 2 },
            localInit: () => 0, // initialize local state, show code completion for next arg
            body: (int x, ParallelLoopState state, int threadLocalStorage) =>
            {
                //Console.WriteLine($"{Task.CurrentId}");
                // no need to use Interlocked.Add, as each task has its own local state
                threadLocalStorage += x;

                Console.WriteLine($"Task {Task.CurrentId} has sum {threadLocalStorage}");
                return threadLocalStorage;
            },
            localFinally: partialSum =>
            {
                Console.WriteLine($"Partial value of task {Task.CurrentId} is {partialSum}");
                // add up all partial sums from all tasks
                Interlocked.Add(ref sum, partialSum);
            }
        );

        Console.WriteLine($"Sum of 1..100 = {sum}");
    }
}
