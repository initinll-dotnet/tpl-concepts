using System.Collections.Concurrent;

namespace ConcurrentCollections;

public class ConcurrentDictionaryDemo
{
    private static readonly ConcurrentDictionary<string, string> capitals = new();

    private static void AddParis()
    {
        // there is no add, since you don't know if it will succeed
        bool success = capitals.TryAdd("France", "Paris");

        string who = Task.CurrentId.HasValue ? ("Task " + Task.CurrentId) : "Main thread";
        Console.WriteLine($"{who} {(success ? "added" : "did not add")} the element.");
    }

    public static void Execute()
    {
        AddParis();
        Task.Factory.StartNew(AddParis).Wait();

        //capitals["Russia"] = "Leningrad";
        var s = capitals.AddOrUpdate("Russia", "Moscow", (k, old) => old + " --> Moscow");
        Console.WriteLine("The capital of Russia is " + capitals["Russia"]);

        capitals["Sweden"] = "Uppsala";
        var capOfNorway = capitals.GetOrAdd("Sweden", "Stockholm");
        Console.WriteLine($"The capital of Sweden is {capOfNorway}.");

        // removal
        const string toRemove = "Russia"; // make a mistake here
        var didRemove = capitals.TryRemove(toRemove, out string removed);
        if (didRemove)
        {
            Console.WriteLine($"We just removed {removed}");
        }
        else
        {
            Console.WriteLine($"Failed to remove capital of {toRemove}");
        }

        // some operations are slow, e.g.,
        Console.WriteLine($"The ");

        foreach (var kv in capitals)
        {
            Console.WriteLine($" - {kv.Value} is the capital of {kv.Key}");
        }

        Console.ReadKey();
    }
}