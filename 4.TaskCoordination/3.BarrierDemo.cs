namespace TaskCoordination;

/*
Summary:
- Barrier, CountdownEvent, ManualResetEventSlim, AutoResetEvent and SemaphoreSlim are all synchronization primitives
- They have a counter that can be incremented or decremented
- Let you execute N threads at a time
- Other threads are unblocked until state changes

- Barrier: blocks all threads until N threads are waiting, 
    then lets those N threads continue via SignalAndWait()
*/

public class BarrierDemo
{
    static readonly Barrier barrier = new(participantCount: 2, postPhaseAction: b =>
    {
        Console.WriteLine($"Phase {b.CurrentPhaseNumber} is finished.");
        //b.ParticipantCount
        //b.ParticipantsRemaining

        // add/remove participants
    });

    private static void Water()
    {
        Console.WriteLine("Putting the kettle on (takes a bit longer).");
        Thread.Sleep(2000);
        barrier.SignalAndWait(); // signaling and waiting fused
        Console.WriteLine("Putting water into cup.");
        barrier.SignalAndWait();
        Console.WriteLine("Putting the kettle away.");

    }

    private static void Cup()
    {
        Console.WriteLine("Finding the nicest tea cup (only takes a second).");
        barrier.SignalAndWait();
        Console.WriteLine("Adding tea.");
        barrier.SignalAndWait();
        Console.WriteLine("Adding sugar");
    }

    public static void Execute()
    {
        var water = Task.Factory.StartNew(Water);
        var cup = Task.Factory.StartNew(Cup);

        var tea = Task.Factory.ContinueWhenAll([water, cup], tasks =>
        {
            Console.WriteLine("Enjoy your cup of tea.");
        });

        tea.Wait();
    }
}
