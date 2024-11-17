using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentCollections;

public class ConcurrentBagDemo
{
    public static void Execute()
    {
        // stack is LIFO
        // queue is FIFO
        // concurrent bag provides NO ordering guarantees

        // keeps a separate list of items for each thread
        // typically requires no synchronization, unless a thread tries to remove an item
        // while the thread-local bag is empty (item stealing)

        var bag = new ConcurrentBag<int>();
        var tasks = new List<Task>();

        for (int i = 0; i < 10; i++)
        {
            var j = i;
            tasks.Add(Task.Factory.StartNew(() =>
            {
                bag.Add(j);
                Console.WriteLine($"{Task.CurrentId} has added {j}");

                int result;
                if (bag.TryPeek(out result))
                {
                    Console.WriteLine($"{Task.CurrentId} has peeked the value {result}");
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // take whatever's last
        int last;
        if (bag.TryTake(out last))
        {
            Console.WriteLine($"Last element is {last}");
        }
    }
}
