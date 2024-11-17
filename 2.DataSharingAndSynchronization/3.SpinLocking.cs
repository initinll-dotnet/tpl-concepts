namespace DataSharingAndSynchronization;

// SpinLock is a lightweight lock that spins in a loop waiting to acquire the lock
/*
The SpinLock structure is a low-level, mutual-exclusion synchronization primitive that spins 
while it waits to acquire a lock. On multicore computers, when wait times are expected to be short 
and when contention is minimal, SpinLock can perform better than other kinds of locks. 
*/
public class SpinLocking
{
    public static void Execute()
    {
        SpinLockDemo();

        // LockRecursion(5);

        Console.ReadKey();
        Console.WriteLine("All done here.");
    }

    private class BankAccount
    {
        public int Balance { get; private set; }

        public void Deposit(int amount)
        {
            Balance += amount;
        }

        public void Withdraw(int amount)
        {
            Balance -= amount;
        }
    }

    private static void SpinLockDemo()
    {
        List<Task> tasks = [];
        BankAccount ba = new();

        // spinning avoid overhead of resheduling
        // useful if you expect the wait time to be very short

        SpinLock sl = new();

        // owner tracking keeps a record of which thread acquired it to improve debugging

        for (int i = 0; i < 10; ++i)
        {
            tasks.Add(Task.Factory.StartNew(() =>
            {
                for (int j = 0; j < 1000; ++j)
                {
                    bool lockTaken = false;
                    try
                    {
                        // sl.IsHeld
                        // sl.IsHeldByCurrentThread
                        sl.Enter(ref lockTaken);
                        ba.Deposit(100);
                    }
                    finally
                    {
                        if (lockTaken) sl.Exit();
                    }
                }
            }));

            tasks.Add(Task.Factory.StartNew(() =>
            {
                for (int j = 0; j < 1000; ++j)
                {
                    bool lockTaken = false;
                    try
                    {
                        sl.Enter(ref lockTaken);
                        ba.Withdraw(100);
                    }
                    finally
                    {
                        if (lockTaken) sl.Exit();
                    }
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        Console.WriteLine($"Final balance is {ba.Balance}.");
    }



    // true = exception, false = deadlock
    private static SpinLock sl = new SpinLock(true);

    private static void LockRecursion(int x)
    {
        // lock recursion is being able to take the same lock multiple times
        bool lockTaken = false;
        try
        {
            sl.Enter(ref lockTaken);
        }
        catch (LockRecursionException e)
        {
            Console.WriteLine("Exception: " + e);
        }
        finally
        {
            if (lockTaken)
            {
                Console.WriteLine($"Took a lock, x = {x}.");
                LockRecursion(x - 1);
                sl.Exit();
            }
            else
            {
                Console.WriteLine($"Failed to take a lock, x = {x}");
            }
        }
    }
}
