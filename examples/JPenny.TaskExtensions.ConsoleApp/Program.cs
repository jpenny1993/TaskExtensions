using System;
using System.Threading.Tasks;

namespace JPenny.TaskExtensions.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tasks = new DummyTasks();

            // Singular Task Pipeline
            await new Pipe<int>(tasks.GetNumber())
                .OnSuccess(result => Console.WriteLine("Success: {0}", result))
                .OnException(ex => Console.WriteLine("Global Exception: {0}", ex))
                .OnException<NotImplementedException>(ex => Console.WriteLine("Specific Exception: {0}", ex))
                .OnCancelled(() => Console.WriteLine("Cancelled"))
                .OnComplete(() => Console.WriteLine("Complete"))
                .RunAsync();

            // Multiple Synchronous but separate pipeline tasks
            await Pipeline.Create()
                .Pipe(tasks.GetNumber(), p =>
                {
                    p.OnSuccess(result => Console.WriteLine("Success: {0}", result))
                    .OnException(ex => Console.WriteLine("Global Exception: {0}", ex))
                    .OnException<NotImplementedException>(ex => Console.WriteLine("Specific Exception: {0}", ex))
                    .OnCancelled(() => Console.WriteLine("Cancelled"))
                    .OnComplete(() => Console.WriteLine("Complete"));
                })
                .Pipe(() => {
                    return new Pipe<string>(tasks.GetString())
                    .OnSuccess(result => Console.WriteLine("Success: {0}", result))
                    .OnException(ex => Console.WriteLine("Global Exception: {0}", ex))
                    .OnException<NotImplementedException>(ex => Console.WriteLine("Specific Exception: {0}", ex))
                    .OnCancelled(() => Console.WriteLine("Cancelled"))
                    .OnComplete(() => Console.WriteLine("Complete"));
                })
                .Pipe(() => {
                    return new Pipe<int>(tasks.ThrowSystemException<int>())
                    .OnSuccess(result => Console.WriteLine("Success: {0}", result))
                    .OnException(ex => Console.WriteLine("Global Exception: {0}", ex))
                    .OnException<NotImplementedException>(ex => Console.WriteLine("Specific Exception: {0}", ex))
                    .OnCancelled(() => Console.WriteLine("Cancelled"))
                    .OnComplete(() => Console.WriteLine("Complete"));
                })
                .Pipe(() => {
                    return new Pipe<string>(tasks.GetString())
                    .OnSuccess(result => Console.WriteLine("Success: {0}", result))
                    .OnException(ex => Console.WriteLine("Global Exception: {0}", ex))
                    .OnException<NotImplementedException>(ex => Console.WriteLine("Specific Exception: {0}", ex))
                    .OnCancelled(() => Console.WriteLine("Cancelled"))
                    .OnComplete(() => Console.WriteLine("Complete"));
                })
                .RunAsync();

            Console.ReadLine();
        }
    }
}
