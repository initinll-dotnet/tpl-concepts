namespace DataSharingAndSynchronization;

public class MutexExample
{
    public static void Execute()
    {
        LocalMutex();
        // GlobalMutex();

        Console.WriteLine("All done here.");
    }

    private class BankAccount
    {
        public int Balance { get; private set; }

        public BankAccount(int balance)
        {
            Balance = balance;
        }

        public void Deposit(int amount)
        {
            Balance += amount;
        }

        public void Withdraw(int amount)
        {
            Balance -= amount;
        }

        public void Transfer(BankAccount where, int amount)
        {
            where.Balance += amount;
            Balance -= amount;
        }
    }

    private static void LocalMutex()
    {
        List<Task> tasks = [];
        BankAccount bankAccount1 = new(0);
        BankAccount bankAccount2 = new(0);

        // many synchro types deriving from WaitHandle
        // Mutex = mutual exclusion

        // two types of mutexes
        // this is a _local_ mutex
        Mutex mutex1 = new();
        Mutex mutex2 = new();

        for (int i = 0; i < 10; ++i)
        {
            var bankAccount1_deposit_tasks = Task.Factory.StartNew(() =>
            {
                for (int j = 0; j < 1000; ++j)
                {
                    bool haveLock = mutex1.WaitOne();
                    try
                    {
                        bankAccount1.Deposit(1); // deposit 10,000 overall
                    }
                    finally
                    {
                        if (haveLock) mutex1.ReleaseMutex();
                    }
                }
            });

            var bankAccount2_deposit_tasks = Task.Factory.StartNew(() =>
            {
                for (int j = 0; j < 1000; ++j)
                {
                    bool haveLock = mutex2.WaitOne();
                    try
                    {
                        bankAccount2.Deposit(1); // deposit 10,000
                    }
                    finally
                    {
                        if (haveLock) mutex2.ReleaseMutex();
                    }
                }
            });

            // transfer needs to lock both accounts
            var backAccount_transfer_tasks = Task.Factory.StartNew(() =>
            {
                for (int j = 0; j < 1000; j++)
                {
                    bool haveLock = Mutex.WaitAll([mutex1, mutex2]);
                    try
                    {
                        bankAccount1.Transfer(bankAccount2, 1); // transfer 10k from ba1 to ba2
                    }
                    finally
                    {
                        if (haveLock)
                        {
                            mutex1.ReleaseMutex();
                            mutex2.ReleaseMutex();
                        }
                    }
                }
            });

            tasks =
            [
                bankAccount1_deposit_tasks,
                bankAccount2_deposit_tasks,
                backAccount_transfer_tasks
            ];
        }

        Task.WaitAll([.. tasks]);

        Console.WriteLine($"Final balance is: BankAccount 1 = {bankAccount1.Balance}, BankAccount 2 = {bankAccount2.Balance}.");
    }

    private static void GlobalMutex()
    {
        const string appName = "MyApp";
        Mutex mutex;
        try
        {
            mutex = Mutex.OpenExisting(appName);
            Console.WriteLine($"Sorry, {appName} is already running.");
            return;
        }
        catch (WaitHandleCannotBeOpenedException)
        {
            Console.WriteLine("We can run the program just fine.");
            // first arg = whether to give current thread initial ownership
            mutex = new Mutex(false, appName);
        }

        Console.ReadKey();
    }
}
