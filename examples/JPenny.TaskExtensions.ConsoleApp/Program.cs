using System;
using System.Threading.Tasks;

namespace JPenny.TaskExtensions.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tasks = new DummyTasks();

            await tasks.GetNumber()
                .OnException(ex => Console.WriteLine($"Error: {ex.Flatten().GetType().Name} | {ex.Flatten().Message}"))
                .OnCancelled(() => Console.WriteLine("Cancelled stage 1"))
                .OnSuccess(r => Console.WriteLine($"Success: number is {r}"))
                .Finally(x => Console.WriteLine("Finished stage 1"))
            .Then(n => tasks.GetString())
                .OnException(ex => Console.WriteLine($"Error: {ex.Flatten().GetType().Name} | {ex.Flatten().Message}"))
                .OnCancelled(() => Console.WriteLine("Cancelled stage 2"))
                .OnSuccess(r => Console.WriteLine($"Success: string is {r}"))
                .Finally(x => Console.WriteLine("Finished stage 2"))
            .Then(n => tasks.ThrowException<int, NotImplementedException>())
                .OnException(ex => Console.WriteLine($"Error: {ex.Flatten().GetType().Name} | {ex.Flatten().Message}"))
                .OnException((NotImplementedException ex) => Console.WriteLine($"Caught: {ex.GetType().Name} | {ex.Message}"))
                .OnCancelled(() => Console.WriteLine("Cancelled stage 3"))
                .OnSuccess(r => Console.WriteLine($"Success: this should never happen"))
                .Finally(x => Console.WriteLine("Finished stage 3"));

            Console.ReadLine();
        }
    }
}
