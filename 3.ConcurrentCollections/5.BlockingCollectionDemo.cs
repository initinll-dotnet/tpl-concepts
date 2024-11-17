using System.Collections.Concurrent;

namespace ConcurrentCollections;

public class BlockingCollectionDemo
{
    static readonly BlockingCollection<int> messages = new(
        collection: new ConcurrentBag<int>(),
        boundedCapacity: 10); /* bounded */

    static readonly CancellationTokenSource cts = new();

    private static void ProduceAndConsume()
    {
        var producer = Task.Factory.StartNew(RunProducer);
        var consumer = Task.Factory.StartNew(RunConsumer);

        try
        {
            Task.WaitAll([producer, consumer], cts.Token);
        }
        catch (AggregateException ae)
        {
            ae.Handle(e => true);
        }
    }

    private static readonly Random random = new();

    private static void RunConsumer()
    {
        foreach (var item in messages.GetConsumingEnumerable())
        {
            cts.Token.ThrowIfCancellationRequested();
            Console.WriteLine($"-{item}");
            Thread.Sleep(random.Next(1000));
        }
    }

    private static void RunProducer()
    {
        while (true)
        {
            cts.Token.ThrowIfCancellationRequested();
            int i = random.Next(100);
            messages.Add(i);
            Console.WriteLine($"+{i}\t");
            Thread.Sleep(random.Next(1000));
        }
    }

    public static void Execute()
    {
        Task.Factory.StartNew(ProduceAndConsume, cts.Token);

        Console.ReadKey();
        cts.Cancel();
    }
}
