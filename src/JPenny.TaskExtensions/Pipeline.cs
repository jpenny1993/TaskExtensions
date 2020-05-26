using System;
using System.Threading;
using System.Threading.Tasks;

namespace JPenny.TaskExtensions
{
    public static class Pipeline
    {
        public static Pipeline<TResult> Task<TResult>(Task<TResult> task)
        {
            return new Pipeline<TResult>(task);
        }

        public static Pipeline<TResult> Task<TResult>(Task<TResult> task, CancellationTokenSource cancellationTokenSource)
        {
            return new Pipeline<TResult>(task, cancellationTokenSource);
        }

        public static Pipeline<TResult> Task<TResult>(Task<TResult> task, TimeSpan cancelAfter)
        {
            return new Pipeline<TResult>(task, cancelAfter);
        }
    }

    public class Pipeline<TResult>
    {
        private readonly CancellationTokenSource _cts;
        private readonly Task<TResult> _task;
        private readonly TaskScheduler _taskScheduler;

        public Pipeline(Task<TResult> task, CancellationTokenSource cancellationTokenSource, TaskScheduler taskScheduler)
        {
            _taskScheduler = taskScheduler;
            _cts = cancellationTokenSource;
            _task = Task.Run(() => task, _cts.Token);
        }

        public Pipeline(Task<TResult> task, CancellationTokenSource cancellationTokenSource)
            : this(task, cancellationTokenSource, TaskScheduler.Current) { }

        public Pipeline(Task<TResult> task)
            : this(task, new CancellationTokenSource()) { }

        public Pipeline(Task<TResult> task, TimeSpan cancelAfter)
            : this(task, new CancellationTokenSource(cancelAfter)) { }

        /// <summary>
        /// Returns an awaitable task.
        /// </summary>
        /// <param name="suppressErrors">Prevents the application from halting on the occasion that a task throws an exception.</param>
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
            Tasks.Finally(_task, alwaysAction, _cts.Token, _taskScheduler);
            return this;
        }

        public Pipeline<TResult> OnCancelled(Action cancelledAction)
        {
            Tasks.OnCancelled(_task, cancelledAction, _cts.Token, _taskScheduler);
            return this;
        }

        public Pipeline<TResult> OnException(Action<AggregateException> errorAction)
        {
            Tasks.OnException(_task, errorAction, _cts.Token, _taskScheduler);
            return this;
        }

        public Pipeline<TResult> OnException<TException>(Action<TException> errorAction)
            where TException : Exception
        {
            Tasks.OnException(_task, errorAction, _cts.Token, _taskScheduler);
            return this;
        }

        public Pipeline<TResult> OnSuccess(Action<TResult> successAction)
        {
            Tasks.OnSuccess(_task, successAction, _cts.Token, _taskScheduler);
            return this;
        }

        // TODO
        //public Pipeline<TNewResult> Then<TNewResult>(Action<TResult> followingAction)
        //{
        //    var newTask = Tasks.Then(_task, followingAction);
        //    return new Pipeline<TNewResult>(newTask, _cts);
        //}

        public Pipeline<TNewResult> Then<TNewResult>(Func<TResult, TNewResult> followingAction)
        {
            var newTask = Tasks.Then(_task, followingAction, _cts.Token, _taskScheduler);
            return new Pipeline<TNewResult>(newTask, _cts, _taskScheduler);
        }

        public Pipeline<TNewResult> Then<TNewResult>(Func<TResult, Task<TNewResult>> followingAction)
        {
            var newTask = Tasks.Then(_task, followingAction, _cts.Token);
            return new Pipeline<TNewResult>(newTask, _cts, _taskScheduler);
        }
    }
}
