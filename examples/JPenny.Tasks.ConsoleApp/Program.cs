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
                .ThenBuilder<int>(options => options
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
                .TaskBuilder(options => options
                    .Action(tasks.ThrowSystemException<int>())
                    .Catch(ex => Console.WriteLine("Exception: {0}", ex))
                )
                .BuildAndExecuteAsync();

            await Pipeline.Create()
                .TaskBuilder<int>(t => t
                    .Action(tasks.GetNumber())
                    .OnSuccess(result => Console.WriteLine("Task 1: Number generated = {0}", result))
                    .OnCompleted(() => Console.WriteLine("Task 1: Completed"))
                )
                .ThenBuilder<int>(t => t
                    .Action(task1Result => (task1Result % 2 == 0) ? (task1Result / 2) : (task1Result * 3 + 1))
                    .OnSuccess(result => Console.WriteLine("Task 2: Number generated = {0}", result))
                    .OnCompleted(() => Console.WriteLine("Task 2: Completed"))
                )
                .Then(task2Result => task2Result + 7)
                .Then(task3Result => Console.WriteLine("Task 4: Task 3 Result = {0}", task3Result))
                .OnCompleted(() => Console.WriteLine("Pipeline 3: Complete"))
                .BuildAndExecuteAsync();

            await Pipeline.Create()
                .CancelAfter(4)
                .Catch(ex => Console.WriteLine("Pipeline Error: {0}", ex.Message))
                .OnCancelled(() => Console.WriteLine("Pipeline Aborted"))
                .OnSuccess(() => Console.WriteLine("Pipeline Succeeded"))
                .OnCompleted(() => Console.WriteLine("Pipeline Finished"))
                .Task(() => Console.WriteLine("\r\nPipeline Started"))
                .TaskBuilder(t => t
                    .Action(() => Task.Delay(TimeSpan.FromSeconds(5)))
                    .Catch(ex => Console.WriteLine("Task 1: Exception Caught"))
                    .OnCancelled(() => Console.WriteLine("Task 1: Cancelled"))
                    .OnSuccess(() => Console.WriteLine("Task 1: Success"))
                    .OnCompleted(() => Console.WriteLine("Task 1: Finished"))
                )
                .TaskBuilder(t => t
                    .Action(() => Task.Delay(TimeSpan.FromSeconds(5)))
                    .Catch(ex => Console.WriteLine("Task 2: Exception Caught"))
                    .OnCancelled(() => Console.WriteLine("Task 2: Cancelled"))
                    .OnSuccess(() => Console.WriteLine("Task 2: Success"))
                    .OnCompleted(() => Console.WriteLine("Task 2: Finished"))
                )
                .TaskBuilder(t => t
                    .Action(() => Task.Delay(TimeSpan.FromSeconds(5)))
                    .Catch(ex => Console.WriteLine("Task 3: Exception Caught"))
                    .OnCancelled(() => Console.WriteLine("Task 3: Cancelled"))
                    .OnSuccess(() => Console.WriteLine("Task 3: Success"))
                    .OnCompleted(() => Console.WriteLine("Task 3: Finished"))
                )
                .TaskBuilder(t => t
                    .Action(() => Task.Delay(TimeSpan.FromSeconds(5)))
                    .Catch(ex => Console.WriteLine("Task 4: Exception Caught"))
                    .OnCancelled(() => Console.WriteLine("Task 4: Cancelled"))
                    .OnSuccess(() => Console.WriteLine("Task 4: Success"))
                    .OnCompleted(() => Console.WriteLine("Task 4: Finished"))
                )
                .BuildAndExecuteAsync();

            Console.ReadLine();
        }
    }
}
