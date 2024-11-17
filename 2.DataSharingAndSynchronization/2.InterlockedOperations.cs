namespace DataSharingAndSynchronization;

public class InterlockedOperations
{
    public static void Execute()
    {
        List<Task> tasks = new();
        BankAccount backAccount = new();

        for (int i = 0; i < 10; ++i)
        {
            tasks.Add(Task.Factory.StartNew(() =>
            {
                for (int j = 0; j < 1000; ++j)
                    backAccount.Deposit(100);
            }));

            tasks.Add(Task.Factory.StartNew(() =>
            {
                for (int j = 0; j < 1000; ++j)
                    backAccount.Withdraw(100);
            }));
        }

        Task.WaitAll([.. tasks]);

        Console.WriteLine($"Final balance is {backAccount.Balance}.");

        // show interlocked methods here

        // Interlocked.MemoryBarrier is a wrapper for Thread.MemoryBarrier
        // only required on memory systems that have weak memory ordering (e.g., Itanium)
        // prevents the CPU from reordering the instructions such that those before the barrier
        // execute after those after

        // The Interlocked class provides us with a set of atomic operations for updating the shared resources in a multithreaded environment. 
        // With these operations, we no longer need to acquire explicit locks like with the lock statement.

        Console.WriteLine("All done here.");
    }
}

internal class BankAccount
{
    private int _balance;

    public int Balance
    {
        get { return _balance; }
        private set { _balance = value; }
    }

    public void Deposit(int amount)
    {
        // Provides atomic operations on variables.
        // atomic = cannot be interrupted
        // Suitable for low-level atomic operations.
        Interlocked.Add(ref _balance, amount);
    }

    public void Withdraw(int amount)
    {
        // Provides atomic operations on variables.
        // atomic = cannot be interrupted
        // Suitable for low-level atomic operations.
        Interlocked.Add(ref _balance, -amount);
        //balance -= amount;
    }
}

// interlocked class contains atomic operations on variables
// atomic = cannot be interrupted


