using System;
using System.Threading;
using System.Threading.Tasks;
using Audacia.Random.Extensions;

namespace JPenny.Tasks.ConsoleApp
{
    internal class DummyTasks
    {
        private Random Random { get; } = new Random(989852);

        public Task<T> GetCancelledTask<T>()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            return Task.FromCanceled<T>(cts.Token);
        }

        public Task<int> GetNumber()
        {
            var number = Random.Next(1, 1000);
            return Task.FromResult(number);
        }

        public Task<string> GetString()
        {
            var str = Random.Sentence();
            return Task.FromResult(str);
        }

        public Task<TResult> ThrowException<TResult, TException>(TException exception)
            where TException : Exception
        {
            return Task.FromException<TResult>(exception);
        }

        public Task<TResult> ThrowException<TResult, TException>()
            where TException : Exception, new()
        {
            return ThrowException<TResult, Exception>(new TException());
        }

        public Task<TResult> ThrowSystemException<TResult>()
        {
            return ThrowException<TResult, Exception>(new Exception("Kaboom!"));
        }
    }
}
