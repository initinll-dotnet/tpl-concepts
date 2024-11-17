namespace TaskCoordination;

/*
Summary:
- Continuations are tasks that run after another task completes
    - State, cancellation options, etc. can be specified
- Continuations can be conditional
    - E.g. TaskContinuationOptions.NotOnFaulted, means don't run if the task faulted
- ContinueWhenAll (one-to-many) and ContinueWhenAny (one-to-any) are useful for waiting for multiple tasks to complete
*/
public class CreatingContinuationsDemo
{
    public static void Execute()
    {
        SimpleContinuation();
        ContinueWhen();
    }

    private static void SimpleContinuation()
    {
        Console.WriteLine();
        Console.WriteLine("Simple Continuation");
        Console.WriteLine();

        var task1 = Task.Factory.StartNew(() =>
        {
            Console.WriteLine($"Boil water (task {Task.CurrentId}), then...");
            throw null;
        });

        var task2 = task1.ContinueWith(t1 =>
        {
            // alternatively can also rethrow exceptions
            if (t1.IsFaulted)
                throw t1.Exception.InnerException;

            Console.WriteLine($"{t1.Id} is {t1.Status}, so pour into cup  {Task.CurrentId})");
        }/*, TaskContinuationOptions.NotOnFaulted*/);

        try
        {
            task2.Wait();
        }
        catch (AggregateException ae)
        {
            ae.Handle(e =>
            {
                Console.WriteLine("Exception: " + e);
                return true;
            });
        }
    }

    private static void ContinueWhen()
    {
        Console.WriteLine();
        Console.WriteLine("Continue When");
        Console.WriteLine();

        var task1 = Task.Factory.StartNew(() => "Task 1");
        var task2 = Task.Factory.StartNew(() => "Task 2");

        // also ContinueWhenAny
        var task3 = Task.Factory.ContinueWhenAll([task1, task2],
          tasks =>
          {
              Console.WriteLine("Tasks completed:");
              foreach (var t in tasks)
                  Console.WriteLine(" - " + t.Result);
              Console.WriteLine("All tasks done");
          });

        task3.Wait();
    }
}
