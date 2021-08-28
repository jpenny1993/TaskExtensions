# TaskExtensions
Extensions on tasks, and an attempt to create a pipeline where multiple tasks can be chained.

Example Usage:
```
static async Task Main(string[] args)
{
    var x = new DummyTasks();

    await Pipeline.Create()
        .Task(o => o
			.Action(x.GetNumber(3))
            .Catch(ex => Console.WriteLine($"Error: {ex.Flatten().GetType().Name} | {ex.Flatten().Message}"))
            .OnCancelled(() => Console.WriteLine("Cancelled stage 1"))
            .OnSuccess(r => Console.WriteLine($"Success: number is {r}"))
            .OnCompleted(x => Console.WriteLine("Finished stage 1"))
		)
        .Then(x.GetString(n))
		.Then((string str) => Console.WriteLine("String calculated: {0}", str))
        .Then(o => o
			.Action(x.NotImplemented())
            .Catch(ex => Console.WriteLine($"Error: {ex.Flatten().GetType().Name} | {ex.Flatten().Message}"))
            .Catch((NotImplementedException ex) => Console.WriteLine($"Caught: {ex.GetType().Name} | {ex.Message}"))
            .OnCancelled(() => Console.WriteLine("Cancelled stage 3"))
            .OnSuccess(r => Console.WriteLine($"Success: this should never happen"))
            .OnCompleted(x => Console.WriteLine("Finished stage 3"))
		)
		.OnSuccess(() => Console.WriteLine("All tasks ran successfully."))
		.OnCancelled(() => Console.WriteLine("Pipeline aborted."))
		.OnCompleted(() => Console.WriteLine("Pipeline Finished..."))
        .BuildAndExecuteAsync();

    Console.ReadLine();
}
```

Expected Output:
```
Success: number is 3
Finished stage 1
String calculated: 3
Caught: NotImplementedException | ...
Finished stage 3
Pipeline aborted.
Pipeline finished...
```
