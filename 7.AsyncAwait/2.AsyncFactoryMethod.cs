using Microsoft.Diagnostics.Tracing.Parsers.Clr;

namespace AsyncAwait;

public class AsyncFactoryMethod
{
    public static async Task Execute()
    {
        var val = 123;
        var foo = await Foo.CreateAsync(val);
        Console.WriteLine(foo.MyValue);
    }
}

public class Foo
{
    private int _value;

    public int MyValue => _value;

    private Foo(int value)
    {
        _value = value;
    }

    private async Task<Foo> InitAsync()
    {
        await Task.Delay(5000);
        return this;
    }

    public static Task<Foo> CreateAsync(int value)
    {
        var result = new Foo(value);
        return result.InitAsync();
    }
}

