using System;
using System.Threading.Tasks;

namespace JPenny.Tasks.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tasks = new DummyTasks();

            // Singular Task Pipeline
            await Pipeline.Create()
                .Task(tasks.GetNumber())
                .BuildThen<int>(options => options
                    .Action(result => Console.WriteLine("Task 1 Output: {0}", result))
                    .Catch<NotImplementedException>(ex => Console.WriteLine("Task 1: Specific Exception, {0}", ex))
                    .Catch(ex => Console.WriteLine("Task 1: Exception, {0}", ex))
                    .OnCancelled(() => Console.WriteLine("Task 1: Cancelled"))
                    .OnSuccess(() => Console.WriteLine("Task 1: Success"))
                    .OnCompleted(() => Console.WriteLine("Task 1: Completed"))
                )
                .OnCancelled(() => Console.WriteLine("Pipeline 1: Cancelled"))
                .OnCompleted(() => Console.WriteLine("Pipeline 1: Complete"))
                .BuildAndExecuteAsync();

            // Multiple Synchronous but separate pipeline tasks
            await Pipeline.Create()
                .OnCancelled(() => Console.WriteLine("Pipeline 2: Cancelled"))
                .OnCompleted(() => Console.WriteLine("Pipeline 2: Complete"))
                .Task(tasks.GetNumber())
                .Then((int result) => Console.WriteLine("Success: {0}", result))
                .Task(tasks.GetString())
                .Then((string result) => Console.WriteLine("Success: {0}", result))
                .BuildTask(options => options
                    .Action(tasks.ThrowSystemException<int>())
                    .Catch(ex => Console.WriteLine("Exception: {0}", ex))
                )
                .BuildAndExecuteAsync();

            Console.ReadLine();
        }
    }
}
