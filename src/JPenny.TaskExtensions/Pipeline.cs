using System;
using System.Threading;
using System.Threading.Tasks;

namespace JPenny.TaskExtensions
{
    public static class Pipeline
    {
        public static Pipeline<TResult> Init<TResult>(Task<TResult> task)
        {
            return new Pipeline<TResult>(task);
        }

        public static Pipeline<TResult> Init<TResult>(Task<TResult> task, CancellationTokenSource cancellationTokenSource)
        {
            return new Pipeline<TResult>(task, cancellationTokenSource);
        }

        public static Pipeline<TResult> Init<TResult>(Task<TResult> task, TimeSpan cancelAfter)
        {
            return new Pipeline<TResult>(task, cancelAfter);
        }
    }

    public class Pipeline<TResult>
    {
        private readonly CancellationTokenSource _cts;
        private readonly Task<TResult> _task;

        public Pipeline(Task<TResult> task, CancellationTokenSource cancellationTokenSource)
        {
            _cts = cancellationTokenSource;
            _task = Task.Run(() => task, _cts.Token);
        }

        public Pipeline(Task<TResult> task)
            : this(task, new CancellationTokenSource()) { }

        public Pipeline(Task<TResult> task, TimeSpan cancelAfter)
            : this(task, new CancellationTokenSource(cancelAfter)) { }

        public async Task Build(bool suppressErrors = false)
        {
            if (!suppressErrors)
            {
                await _task;
                return;
            }

            try
            {
                await _task;
            }
            catch { }
        }

        public Pipeline<TResult> Finally(Action<Task<TResult>> alwaysAction)
        {
            Tasks.Finally(_task, alwaysAction);
            return this;
        }

        public Pipeline<TResult> OnCancelled(Action cancelledAction)
        {
            Tasks.OnCancelled(_task, cancelledAction);
            return this;
        }

        public Pipeline<TResult> OnException(Action<AggregateException> errorAction)
        {
            Tasks.OnException(_task, errorAction);
            return this;
        }

        public Pipeline<TResult> OnException<TException>(Action<TException> errorAction)
            where TException : Exception
        {
            Tasks.OnException(_task, errorAction);
            return this;
        }

        public Pipeline<TResult> OnSuccess(Action<TResult> successAction)
        {
            Tasks.OnSuccess(_task, successAction);
            return this;
        }

        public Pipeline<TNewResult> Then<TNewResult>(Func<TResult, TNewResult> followingAction)
        {
            var newTask = Task.Run(async () =>
            {
                var result = await _task;
                return followingAction(result);
            }, _cts.Token);

            return new Pipeline<TNewResult>(newTask, _cts);
        }

        public Pipeline<TNewResult> Then<TNewResult>(Func<TResult, Task<TNewResult>> followingTask)
        {
            var newTask = Task.Run(async () =>
            {
                var result = await _task;
                return await followingTask(result);
            }, _cts.Token);

            return new Pipeline<TNewResult>(newTask, _cts);
        }
    }
}
