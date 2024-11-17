namespace TaskCoordination;

/*
Summary:
- Barrier, CountdownEvent, ManualResetEventSlim, AutoResetEvent and SemaphoreSlim are all synchronization primitives
- They have a counter that can be incremented or decremented
- Let you execute N threads at a time
- Other threads are unblocked until state changes

- SemaphoreSlim: counter CurrentCount decremented on Wait(), incremented on Release(N), 
    can have maximum count via MaxCount
*/

public class SemaphoreSlimDemo
{
    public static void Execute()
    {
        var semaphoreSlim = new SemaphoreSlim(initialCount: 2, maxCount: 10);

        for (int i = 0; i < 20; ++i)
        {
            Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"Entering task {Task.CurrentId}.");
                semaphoreSlim.Wait(); // ReleaseCount--
                Console.WriteLine($"Processing task {Task.CurrentId}.");
            });
        }

        while (semaphoreSlim.CurrentCount <= 2)
        {
            Console.WriteLine($"Semaphore count: {semaphoreSlim.CurrentCount}");
            Console.ReadKey();
            semaphoreSlim.Release(releaseCount: 2); // ReleaseCount += n
        }
    }
}
