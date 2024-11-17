using System.Collections.Concurrent;

namespace ConcurrentCollections;

public class ConcurrentStackDemo
{
    public static void Execute()
    {
        ConcurrentStack<int> stack = new();

        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        stack.Push(4);

        int result;
        if (stack.TryPeek(out result))
            Console.WriteLine($"{result} is on top");

        if (stack.TryPop(out result))
            Console.WriteLine($"Popped {result}");

        var items = new int[5];
        if (stack.TryPopRange(items, 0, 5) > 0) // actually pops only 3 items
        {
            var text = string.Join(", ", items.Select(i => i.ToString()));
            Console.WriteLine($"Popped these items: {text}");
        }

    }
}