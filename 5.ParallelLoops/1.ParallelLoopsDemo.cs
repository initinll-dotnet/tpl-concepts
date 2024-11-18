namespace ParallelLoops;

public class ParallelLoopsDemo
{
    private static IEnumerable<int> Range(int start, int end, int step)
    {
        for (int i = start; i < end; i += step)
        {
            yield return i;
        }
    }

    public static void Execute()
    {
        var a = new Action(() => Console.WriteLine($"First {Task.CurrentId}"));
        var b = new Action(() => Console.WriteLine($"Second {Task.CurrentId}"));
        var c = new Action(() => Console.WriteLine($"Third {Task.CurrentId}"));

        Parallel.Invoke(a, b, c);
        // these are blocking operations; wait on all tasks

        Parallel.For(fromInclusive: 1, toExclusive: 11, body: x =>
        {
            Console.Write($"{x * x}\t");
        });
        Console.WriteLine();

        // has a step strictly equal to 1
        // if you want something else...
        Parallel.ForEach(source: Range(1, 20, 3), body: Console.WriteLine);

        string[] letters = { "oh", "what", "a", "night" };

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = 2
        };

        Parallel.ForEach(source: letters, parallelOptions: parallelOptions, body: letter =>
        {
            Console.WriteLine($"{letter} has length {letter.Length} (task {Task.CurrentId})");
        });
    }
}
