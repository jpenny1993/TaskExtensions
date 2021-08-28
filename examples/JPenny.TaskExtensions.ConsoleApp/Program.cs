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
            await Pipeline.Create()
                .Task(tasks.GetNumber())
                .Then(options => options
                    .Action((int result) => Console.WriteLine("Success: {0}", result))
                    .Catch<NotImplementedException>(ex => Console.WriteLine("Specific Exception: {0}", ex))
                )
                .OnCancelled(() => Console.WriteLine("Cancelled"))
                .OnCompleted(() => Console.WriteLine("Complete"))
                .BuildAndExecuteAsync();

            // Multiple Synchronous but separate pipeline tasks
            await Pipeline.Create()
                .OnCancelled(() => Console.WriteLine("Cancelled"))
                .OnCompleted(() => Console.WriteLine("Complete"))
                .Task(options => options
                    .Action(tasks.GetNumber())
                    .Catch(ex => Console.WriteLine("Specific Exception: {0}", ex))
                    //.OnSuccess(result => Console.WriteLine("Success: {0}", result))
                )
                .Then((int result) => Console.WriteLine("Success: {0}", result))
                .Then(options => options
                    .Action(tasks.GetString())
                    .Catch<NotImplementedException>(ex => Console.WriteLine("Specific Exception: {0}", ex))
                )
                .Then(result => Console.WriteLine("Success: {0}", result))
                .Then(options => options
                    .Action((Task)tasks.ThrowSystemException<int>())
                    .Catch(ex => Console.WriteLine("Exception: {0}", ex))
                )
                .BuildAndExecuteAsync();

            Console.ReadLine();
        }
    }
}
