using System.Threading.Channels;

namespace ConcurrentCollections;

public class ChannelDemo
{
    static readonly Channel<int> channel = Channel.CreateBounded<int>(
        new BoundedChannelOptions(10) // Bounded capacity of 10
        {
            FullMode = BoundedChannelFullMode.Wait // Wait for space if full
        });

    static readonly CancellationTokenSource cts = new();

    private static async Task ProduceAndConsumeAsync()
    {
        var producer = Task.Run(() => RunProducerAsync(cts.Token));
        var consumer = Task.Run(() => RunConsumerAsync(cts.Token));

        try
        {
            await Task.WhenAll(producer, consumer);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was canceled.");
        }
    }

    private static readonly Random random = new();

    private static async Task RunConsumerAsync(CancellationToken cancellationToken)
    {
        await foreach (var item in channel.Reader.ReadAllAsync(cancellationToken))
        {
            Console.WriteLine($"-{item}");
            await Task.Delay(random.Next(1000), cancellationToken);
        }
    }

    private static async Task RunProducerAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            int i = random.Next(100);
            await channel.Writer.WriteAsync(i, cancellationToken);
            Console.WriteLine($"+{i}\t");
            await Task.Delay(random.Next(1000), cancellationToken);
        }

        channel.Writer.Complete(); // Signal completion to the consumer
    }

    public static void Execute()
    {
        Task.Run(() => ProduceAndConsumeAsync());

        Console.ReadKey();
        cts.Cancel();
    }
}