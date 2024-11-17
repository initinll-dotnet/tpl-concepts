namespace DataSharingAndSynchronization;

public class CriticalSections
{
    public static void Execute()
    {
        BankAccount_Transactions();

        Console.WriteLine("All done here.");
    }

    private static void BankAccount_Transactions()
    {
        List<Task> tasks = [];

        // .net 8 lock implementation
        BankAccount1 bankAccount1 = new();
        // .net 9 lock implementation
        BankAccount1 bankAccount2 = new();

        for (int i = 0; i < 10; ++i)
        {
            tasks.Add(Task.Factory.StartNew(() =>
            {
                for (int j = 0; j < 1000; ++j)
                {
                    bankAccount1.Deposit(100);
                    bankAccount2.Deposit(100);
                }
            }));

            tasks.Add(Task.Factory.StartNew(() =>
            {
                for (int j = 0; j < 1000; ++j)
                {
                    bankAccount1.Withdraw(100);
                    bankAccount2.Withdraw(100);
                }
            }));
        }

        Task.WaitAll([.. tasks]);

        Console.WriteLine($"BankAccount1 - Final balance is {bankAccount1.Balance}.");
        Console.WriteLine($"BankAccount2 - Final balance is {bankAccount2.Balance}.");
    }

    private class BankAccount1
    {
        // .net 8 and earlier implementation
        private readonly object _padlock = new();
        public int Balance { get; private set; }

        public void Deposit(int amount)
        {

            lock (_padlock)
            {
                // += is really two operations
                // op1 is temp <- get_Balance() + amount
                // op2 is set_Balance(temp)
                // something can happen _between_ op1 and op2

                Balance += amount;
            }
        }

        public void Withdraw(int amount)
        {
            lock (_padlock)
            {
                Balance -= amount;
            }
        }
    }

    private class BankAccount2
    {
        // new .net 9 implementation
        private readonly Lock _lockObject = new();
        public int Balance { get; private set; }

        public void Deposit(int amount)
        {

            lock (_lockObject)
            {
                // += is really two operations
                // op1 is temp <- get_Balance() + amount
                // op2 is set_Balance(temp)
                // something can happen _between_ op1 and op2

                Balance += amount;
            }
        }

        public void Withdraw(int amount)
        {
            lock (_lockObject)
            {
                Balance -= amount;
            }
        }
    }
}
