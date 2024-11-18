namespace ParallelLoops;

public class BreakingAndStoppingDemo
{
    private static void Demo()
    {
        var cts = new CancellationTokenSource();

        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cts.Token
        };

        ParallelLoopResult result = Parallel.For(
            fromInclusive: 0,
            toExclusive: 20,
            parallelOptions: parallelOptions,
            body: (int x, ParallelLoopState state) =>
        {
            Console.Write($"{x}[{Task.CurrentId}]\t");

            if (x == 10)
            {
                cts.Cancel(); // throws OperationCanceledException
                //throw new Exception(); // execution stops on exception, throws AggregateException
                //state.Stop(); // stop execution as soon as possible
                //state.Break(); // request that loop stop execution of iterations beyond current iteration asap
                // Break() - sets state.LowestBreakIteration, i.e. 10 the current one which caused the break
            }

            if (state.IsExceptional)
                Console.Write($"EX[{Task.CurrentId}]\t");

            // state.LowestBreakIteration, ShouldExitCurrentIteration
        });

        Console.WriteLine();
        Console.WriteLine($"Was loop completed? {result.IsCompleted}"); // uncomment break

        if (result.LowestBreakIteration.HasValue)
            Console.WriteLine($"Lowest break iteration: {result.LowestBreakIteration}");
    }

    public static void Execute()
    {
        try
        {
            Demo();
        }
        catch (OperationCanceledException) { }
        catch (AggregateException ae)
        {
            ae.Handle(e =>
            {
                Console.WriteLine(e.Message);
                return true;
            });
        }
    }
}
