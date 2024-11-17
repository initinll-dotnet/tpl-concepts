namespace TaskCoordination;

/*
Summary:
- Barrier, CountdownEvent, ManualResetEventSlim, AutoResetEvent and SemaphoreSlim are all synchronization primitives
- They have a counter that can be incremented or decremented
- Let you execute N threads at a time
- Other threads are unblocked until state changes

- ManualResetEvent & AutoResetEvent: like CountdownEvent with a count of 1
- AutoResetEvent: resets automatically after waiting
- ManualResetEvent: stays set until you manually reset it
*/

public class ResetEventsDemo
{
    public static void Execute()
    {
        //Manual();
        Automatic();
    }

    private static void Manual()
    {
        var manualResetEventSlim = new ManualResetEventSlim();
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        Task.Factory.StartNew(() =>
        {
            Console.WriteLine("Boiling water...");
            for (int i = 0; i < 30; i++)
            {
                token.ThrowIfCancellationRequested();
                Thread.Sleep(100);
            }
            Console.WriteLine("Water is ready.");
            manualResetEventSlim.Set();
        }, token);

        var makeTea = Task.Factory.StartNew(() =>
        {
            Console.WriteLine("Waiting for water...");
            manualResetEventSlim.Wait(5000, token);
            Console.WriteLine("Here is your tea!");
            Console.WriteLine($"Is the event set? {manualResetEventSlim.IsSet}");

            manualResetEventSlim.Reset();
            manualResetEventSlim.Wait(1000, token); // already set!
            Console.WriteLine("That was a nice cup of tea!");
        }, token);

        makeTea.Wait(token);
    }

    private static void Automatic()
    {
        // try switching between manual and auto :)
        var autoResetEvent = new AutoResetEvent(initialState: false);

        autoResetEvent.Set(); // ok, it's set

        autoResetEvent.WaitOne(); // this is ok but, in auto, it causes a reset

        if (autoResetEvent.WaitOne(1000))
        {
            Console.WriteLine("Succeeded");
        }
        else
        {
            Console.WriteLine("Timed out");
        }
    }
}