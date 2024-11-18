namespace ParallelLinq;

public class CancellationAndExceptions
{
    public static void Execute()
    {
        var cts = new CancellationTokenSource();
        var items = Enumerable.Range(start: 1, count: 20);

        ParallelQuery<double> results = items
            .AsParallel()
            .WithCancellation(cts.Token)
            .Select(i =>
            {
                double result = Math.Log10(i);

                // if (result > 1)
                // {
                //     throw new InvalidOperationException();
                // }

                Thread.Sleep((int)(result * 1000));
                Console.WriteLine($"Item = {i}, TaskId = {Task.CurrentId}");
                return result;
            });

        // no exception yet, but...
        try
        {
            foreach (var c in results)
            {
                if (c > 1)
                {
                    cts.Cancel();
                }

                Console.WriteLine($"result = {c}, Main thread");
            }
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine($"Canceled");
        }
        catch (AggregateException ae)
        {
            ae.Handle(e =>
            {
                Console.WriteLine($"{e.GetType().Name}: {e.Message}");
                return true;
            });
        }
    }
}
