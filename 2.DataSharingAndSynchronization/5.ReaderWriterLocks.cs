namespace DataSharingAndSynchronization;

public class ReaderWriterLocks
{
    // recursion is not recommended and can lead to deadlocks
    private static ReaderWriterLockSlim padlock = new(LockRecursionPolicy.SupportsRecursion);

    public static void Execute()
    {
        int x = 0;

        List<Task> tasks = [];

        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Factory.StartNew(() =>
            {
                //padlock.EnterReadLock();
                //padlock.EnterReadLock();
                padlock.EnterUpgradeableReadLock();

                if (i % 2 == 0)
                {
                    padlock.EnterWriteLock();
                    x++;
                    padlock.ExitWriteLock();
                }

                // can now read
                Console.WriteLine($"Entered read lock, x = {x}, pausing for 3sec");
                Thread.Sleep(3000);

                //padlock.ExitReadLock();
                //padlock.ExitReadLock();
                padlock.ExitUpgradeableReadLock();

                Console.WriteLine($"Exited read lock, x = {x}.");
            }));
        }

        try
        {
            Task.WaitAll([.. tasks]);
        }
        catch (AggregateException ae)
        {
            ae.Handle(e =>
            {
                Console.WriteLine(e);
                return true;
            });
        }

        Random random = new();

        while (true)
        {
            Console.ReadKey();
            padlock.EnterWriteLock();
            Console.WriteLine("Write lock acquired");
            int newValue = random.Next(10);
            x = newValue;
            Console.WriteLine($"Set x = {x}");
            padlock.ExitWriteLock();
            Console.WriteLine("Write lock released");
        }
    }
}
