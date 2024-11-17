namespace TaskCoordination;

/*
Summary:
- Barrier, CountdownEvent, ManualResetEventSlim, AutoResetEvent and SemaphoreSlim are all synchronization primitives
- They have a counter that can be incremented or decremented
- Let you execute N threads at a time
- Other threads are unblocked until state changes

- CountDownEvent: Signaling and waiting separted, waits until signal count reaches 0, then unblocks
*/

public class CountDownEventDemo
{
    private static readonly int taskCount = 5;
    static readonly CountdownEvent countdownEvent = new(initialCount: taskCount);
    static readonly Random random = new();

    public static void Execute()
    {
        var tasks = new Task[taskCount];

        for (int i = 0; i < taskCount; i++)
        {
            tasks[i] = Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"Entering task {Task.CurrentId}.");
                Thread.Sleep(random.Next(3000));
                countdownEvent.Signal(); // also takes a signalcount
                                         //cte.CurrentCount/InitialCount
                Console.WriteLine($"Exiting task {Task.CurrentId}.");
            });
        }

        var finalTask = Task.Factory.StartNew(() =>
        {
            Console.WriteLine($"Waiting for other tasks in task {Task.CurrentId}");
            countdownEvent.Wait();
            Console.WriteLine("All tasks completed.");
        });

        finalTask.Wait();
    }
}