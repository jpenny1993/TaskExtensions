using System;
using System.Threading.Tasks;

namespace JPenny.TaskExtensions.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tasks = new DummyTasks();

            await Pipeline.Task(tasks.GetNumber())
                .OnException(WriteError(1))
                .OnCancelled(WriteCancelled(1))
                .OnSuccess(WriteSuccess<int>(1))
                .Finally(WriteAlways<Task<int>>(1))
            .Then(n => tasks.GetString())
                .OnException(WriteError(2))
                .OnCancelled(WriteCancelled(2))
                .OnSuccess(WriteSuccess<string>(2))
                .Finally(WriteAlways<Task<string>>(2))
            .Then(n => tasks.ThrowException<int, NotImplementedException>())
                .OnException(WriteCaughtError<NotImplementedException>(3))
                .OnException(WriteError(3))
                .OnCancelled(WriteCancelled(3))
                .OnSuccess(WriteSuccess<int>(3))
                .Finally(WriteAlways<Task<int>>(3))
            .Then(n => tasks.GetString())
                .OnException(WriteError(4))
                .OnCancelled(WriteCancelled(4))
                .OnSuccess(WriteSuccess<string>(4))
                .Finally(WriteAlways<Task<string>>(4))
            .Build(suppressErrors: true);

            Console.ReadLine();
        }

        public static Action<T> WriteAlways<T>(int stage)
        {
            return t => Console.WriteLine($"Finished: Stage {stage}");
        }

        public static Action WriteCancelled(int stage)
        {
            return () => Console.WriteLine($"Cancelled: Stage {stage}");
        }

        public static Action<T> WriteCaughtError<T>(int stage) where T : Exception
        {
            return t => Console.WriteLine($"Caught Error: Stage {stage} | {t.GetType().Name} | {t.Message}");
        }

        public static Action<AggregateException> WriteError(int stage)
        {
            return t => Console.WriteLine($"Fatal Error: Stage {stage} | {t.Flatten().GetType().Name} | {t.Flatten().Message}");
        }

        public static Action<T> WriteSuccess<T>(int stage)
        {
            return t => Console.WriteLine($"Success: Stage {stage}, Value: {t}");
        }
    }
}
