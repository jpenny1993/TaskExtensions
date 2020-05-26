# TaskExtensions
Extensions on tasks, and an attempt to create a pipeline where multiple tasks can be chained.

Example Usage:
```
static async Task Main(string[] args)
{
    var x = new DummyTasks();

    await Pipeline
        .Init(x.GetNumber(3))
            .OnException(ex => Console.WriteLine($"Error: {ex.Flatten().GetType().Name} | {ex.Flatten().Message}"))
            .OnCancelled(() => Console.WriteLine("Cancelled stage 1"))
            .OnSuccess(r => Console.WriteLine($"Success: number is {r}"))
            .Finally(x => Console.WriteLine("Finished stage 1"))
        .Then(n => x.GetString(n))
            .OnException(ex => Console.WriteLine($"Error: {ex.Flatten().GetType().Name} | {ex.Flatten().Message}"))
            .OnCancelled(() => Console.WriteLine("Cancelled stage 2"))
            .OnSuccess(r => Console.WriteLine($"Success: string is {r}"))
            .Finally(x => Console.WriteLine("Finished stage 2"))
        .Then(n => x.NotImplemented())
            .OnException(ex => Console.WriteLine($"Error: {ex.Flatten().GetType().Name} | {ex.Flatten().Message}"))
            .OnException((NotImplementedException ex) => Console.WriteLine($"Caught: {ex.GetType().Name} | {ex.Message}"))
            .OnCancelled(() => Console.WriteLine("Cancelled stage 3"))
            .OnSuccess(r => Console.WriteLine($"Success: this should never happen"))
            .Finally(x => Console.WriteLine("Finished stage 3"))
        .Build(suppressErrors: true);

    Console.ReadLine();
}
```

Expected Output:
```
Success: number is 3
Success: string is 3
Error: NotImplementedException | ...
Caught: NotImplementedException | ...
```
