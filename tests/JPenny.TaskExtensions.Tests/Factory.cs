using System;
using System.Threading;
using System.Threading.Tasks;
using Audacia.Random.Extensions;

namespace JPenny.TaskExtensions.Tests
{
    public class Factory
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

        public Task<T> ThrowSystemException<T>()
        {
            return Task.FromException<T>(new Exception("Kaboom!"));
        }
    }
}
