namespace AsyncAwait;

public class AsyncAwaitDemo
{
    public async void Execute1()
    {
        // Bloking vs Non-Blocking

        // // ----------------------------------------------------------------------------

        // // blocking statement, it will block the main thread
        // Console.WriteLine();
        // var val1 = CalculateValue();
        // Console.WriteLine($"Value 1 is {val1}");
        // // this does not run until the CalculateValue method is done
        // Console.WriteLine("Value 1 Done");
        // Console.WriteLine();

        // // ----------------------------------------------------------------------------

        // // non blocking statement, it will not block the main thread
        // var val2 = CalculateValueAsync1();
        // // creates continuation that executes asynchronously when the target Task completes
        // val2.ContinueWith(t =>
        // {
        //     Console.WriteLine($"Value 2 is {t.Result.ToString()}");
        // });
        // // waits for the target Task to complete execution
        // val2.Wait();
        // // this runs without waiting for the CalculateValueAsync1 method to complete
        // Console.WriteLine("Value 2 Done");
        // Console.WriteLine();

        // // ----------------------------------------------------------------------------
    }

    public async Task Execute2()
    {
        // Async Await
        // - async keyword
        // - await keyword
        // - Task or Task<T> return type

        // // ----------------------------------------------------------------------------

        // // async method
        // Console.WriteLine();
        // // await keyword, it will not block the main thread
        // var val3 = await CalculateValueAsync2();
        // // this after does not run until the CalculateValueAsync2 method is done, non-blocking
        // Console.WriteLine($"Value 3 is {val3}");
        // Console.WriteLine("Value 3 Done");

        await CalculateValueAsync4();
    }

    private int CalculateValue()
    {
        Thread.Sleep(5000);
        return 123;
    }

    private Task<int> CalculateValueAsync1()
    {
        return Task.Factory.StartNew(() =>
        {
            Thread.Sleep(5000);
            return 123;
        });
    }

    private async Task<int> CalculateValueAsync2()
    {
        await Task.Delay(5000);
        return 123;
    }

    private async Task CalculateValueAsync3()
    {
        // old way of doing threading
        // access result using Task.Result
        Task<int> t1 = Task.Factory.StartNew(() =>
        {
            Thread.Sleep(5000);
            return 123;
        });

        // differernt way of doing threading
        // access result using await keyword
        Task<Task<int>> t2 = Task.Factory.StartNew(async () =>
        {
            await Task.Delay(5000);
            return 123;
        });

        // unwrapping the task
        Task<int> t3 = Task.Factory.StartNew(async () =>
        {
            await Task.Delay(5000);
            return 123;
        }).Unwrap();

        // double unwrapping the task
        int result1 = await await Task.Factory.StartNew(async () =>
        {
            await Task.Delay(5000);
            return 123;
        });

        int result2 = await await Task.Factory.StartNew(async delegate
        {
            await Task.Delay(5000);
            return 123;
        });

        int result3 = await Task.Run(async () =>
        {
            await Task.Delay(5000);
            return 123;
        });

        // modern and clean way of doing threading, recommended
        int result4 = await Task.Run(async delegate
        {
            await Task.Delay(5000);
            return 123;
        });
    }

    private async Task CalculateValueAsync4()
    {
        Console.WriteLine();

        var cts = new CancellationTokenSource();
        var token = cts.Token;

        var t1 = Task.Run(() =>
        {
            Task.Delay(1000);
            return 123;
        }, cancellationToken: token);

        var t2 = Task.Run(() =>
        {
            throw new InvalidOperationException();
            // cts.Token.ThrowIfCancellationRequested();
            Task.Delay(10_000);
            return 456;
        }, cancellationToken: token);

        // var result1 = await Task.WhenAny(t1, t2);
        // Console.WriteLine($"Result: {result1.Result}");

        try
        {
            cts.Cancel();
            var result = await Task.WhenAll(t1, t2);
            Console.WriteLine($"Result: {result[0]}, {result[1]}");
        }
        catch (AggregateException ae)
        {
            ae.Handle(e =>
            {
                Console.WriteLine($"Result - AggregateException | {e.GetType().Name}: {e.Message}");
                return true;
            });
        }
        catch (Exception e)
        {
            Console.WriteLine($"Result - Exception | {e.GetType().Name}: {e.Message}");
        }

        try
        {
            Console.WriteLine($"Task t1 status: {t1.Status}");

            if (t1.Status == TaskStatus.RanToCompletion)
            {
                Console.WriteLine($"Task t1 result: {t1.Result}");
            }
            else if (t1.Status == TaskStatus.Faulted)
            {
                Console.WriteLine($"Task t1 faulted: {t1.Exception.InnerException.Message}");
            }
            else if (t1.Status == TaskStatus.Canceled)
            {
                Console.WriteLine($"Task t1 canceled.");
            }

            Console.WriteLine($"Task t2 status: {t2.Status}");

            if (t2.Status == TaskStatus.RanToCompletion)
            {
                Console.WriteLine($"Task t2 result: {t2.Result}");
            }
            else if (t2.Status == TaskStatus.Faulted)
            {
                Console.WriteLine($"Task t2 faulted: {t2.Exception.GetType().Name}:{t2.Exception.InnerException.Message}");
            }
            else if (t2.Status == TaskStatus.Canceled)
            {
                Console.WriteLine($"Task t2 canceled.");
            }
        }
        catch (AggregateException ae)
        {
            ae.Handle(e =>
            {
                Console.WriteLine($"AggregateException | {e.GetType().Name}: {e.Message}");
                return true;
            });
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception | {e.GetType().Name}: {e.Message}");
        }
    }
}
