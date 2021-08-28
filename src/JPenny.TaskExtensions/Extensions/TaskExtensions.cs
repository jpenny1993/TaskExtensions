using System;
using System.Threading;
using System.Threading.Tasks;

namespace JPenny.TaskExtensions.Extensions
{
    /// <summary>
    /// A set of extension methods to allow fluent chaining of tasks and actions.
    /// </summary>
    public static class TaskExtensions
    {
        private const TaskContinuationOptions DefaultOptions = TaskContinuationOptions.PreferFairness | TaskContinuationOptions.ExecuteSynchronously;

        private const TaskContinuationOptions CancelOptions = TaskContinuationOptions.OnlyOnCanceled | DefaultOptions;
        private const TaskContinuationOptions ErrorOptions = TaskContinuationOptions.OnlyOnFaulted | DefaultOptions;
        private const TaskContinuationOptions SuccessOptions = TaskContinuationOptions.OnlyOnRanToCompletion | DefaultOptions;

        /// <summary>
        /// Runs the provided action when the task has finished.
        /// It will run regardless of if the task has completed successfully or not.
        /// </summary>
        /// <typeparam name="TResult">The return type of the task.</typeparam>
        /// <param name="task">The task being run.</param>
        /// <param name="alwaysAction">The action to run when the task has succeeded.</param>
        /// <returns>The provided task for further extensions.</returns>
        public static Task<TResult> Finally<TResult>(
            this Task<TResult> task,
            Action<Task<TResult>> alwaysAction,
            CancellationToken cancellationToken = default,
            TaskScheduler taskScheduler = default)
        {
            if (cancellationToken == default)
                cancellationToken = CancellationToken.None;

            if (taskScheduler == default)
                taskScheduler = TaskScheduler.Current;

            task.ContinueWith(alwaysAction, cancellationToken, DefaultOptions, taskScheduler)
                .ConfigureAwait(false);
            return task;
        }

        /// <summary>
        /// Runs the provided action when the task is cancelled.
        /// </summary>
        /// <typeparam name="TResult">The return type of the task.</typeparam>
        /// <param name="task">The task being run.</param>
        /// <param name="cancelledAction">The action to run when the task is cancelled.</param>
        /// <returns>The provided task for further extensions.</returns>
        public static Task<TResult> OnCancelled<TResult>(
            this Task<TResult> task,
            Action cancelledAction,
            CancellationToken cancellationToken = default,
            TaskScheduler taskScheduler = default)
        {
            if (cancellationToken == default)
                cancellationToken = CancellationToken.None;

            if (taskScheduler == default)
                taskScheduler = TaskScheduler.Current;

            task.ContinueWith(_ => cancelledAction(), cancellationToken, CancelOptions, taskScheduler)
                .ConfigureAwait(false);
            return task;
        }

        /// <summary>
        /// Runs the provided action when the an exception s thrown from inside the task.
        /// </summary>
        /// <typeparam name="TResult">The return type of the task.</typeparam>
        /// <param name="task">The task being run.</param>
        /// <param name="errorHandler">The action to run when the exception is caught.</param>
        /// <returns>The provided task for further extensions.</returns>
        public static Task<TResult> OnException<TResult>(
            this Task<TResult> task,
            Action<AggregateException> errorHandler,
            CancellationToken cancellationToken = default,
            TaskScheduler taskScheduler = default)
        {
            if (cancellationToken == default)
                cancellationToken = CancellationToken.None;

            if (taskScheduler == default)
                taskScheduler = TaskScheduler.Current;

            task.ContinueWith(t => errorHandler(t.Exception), cancellationToken, ErrorOptions, taskScheduler)
                .ConfigureAwait(false);
            return task;
        }

        /// <summary>
        /// Runs the provided action when the specified Exception is caught from inside the task.
        /// </summary>
        /// <typeparam name="TResult">The return type of the task.</typeparam>
        /// <param name="task">The task being run.</param>
        /// <param name="errorHandler">The action to run when the exception is caught.</param>
        /// <returns>The provided task for further extensions.</returns>
        public static Task<TResult> OnException<TException, TResult>(
            this Task<TResult> task,
            Action<TException> errorHandler,
            CancellationToken cancellationToken = default,
            TaskScheduler taskScheduler = default)
            where TException : Exception
        {
            if (cancellationToken == default)
                cancellationToken = CancellationToken.None;

            if (taskScheduler == default)
                taskScheduler = TaskScheduler.Current;

            task.ContinueWith(continuation => {
                continuation.Exception.Handle(error =>
                {
                    var isEx = error is TException;
                    if (isEx)
                    {
                        errorHandler((TException)error);
                    }

                    return isEx;
                });
            }, cancellationToken, ErrorOptions, taskScheduler)
            .ConfigureAwait(false);
            return task;
        }

        /// <summary>
        /// Runs the provided action when the task has completed successfully.
        /// </summary>
        /// <typeparam name="TResult">The return type of the task.</typeparam>
        /// <param name="task">The task being run.</param>
        /// <param name="successAction">The action to run when the task has succeeded.</param>
        /// <returns>The provided task for further extensions.</returns>
        public static Task<TResult> OnSuccess<TResult>(
            this Task<TResult> task,
            Action<TResult> successAction,
            CancellationToken cancellationToken = default,
            TaskScheduler taskScheduler = default)
        {
            if (cancellationToken == default)
                cancellationToken = CancellationToken.None;

            if (taskScheduler == default)
                taskScheduler = TaskScheduler.Current;

            task.ContinueWith(t => successAction(t.Result), cancellationToken, SuccessOptions, taskScheduler)
                .ConfigureAwait(false);
            return task;
        }

        /// <summary>
        /// Waits for the current task to complete, then runs the following action.
        /// </summary>
        /// <typeparam name="TResult">The return type of the task.</typeparam>
        /// <typeparam name="TNewResult">The return type of the following task.</typeparam>
        /// <param name="task">The task being run.</param>
        /// <param name="followingAction">The following action to run using the result of the task.</param>
        /// <returns>The resultant task of the following task.</returns>
        public static Task Then<TResult>(
            this Task<TResult> task,
            Action<TResult> followingAction,
            CancellationToken cancellationToken = default,
            TaskScheduler taskScheduler = default)
        {
            if (cancellationToken == default)
                cancellationToken = CancellationToken.None;

            if (taskScheduler == default)
                taskScheduler = TaskScheduler.Current;

            return task.ContinueWith(t => followingAction(t.Result), cancellationToken, SuccessOptions, taskScheduler);
        }

        /// <summary>
        /// Waits for the current task to complete, then runs the following action.
        /// </summary>
        /// <typeparam name="TResult">The return type of the task.</typeparam>
        /// <typeparam name="TNewResult">The return type of the following task.</typeparam>
        /// <param name="task">The task being run.</param>
        /// <param name="followingAction">The following action to run using the result of the task.</param>
        /// <returns>The resultant task of the following task.</returns>
        public static Task<TNewResult> Then<TResult, TNewResult>(
            this Task<TResult> task,
            Func<TResult, TNewResult> followingAction,
            CancellationToken cancellationToken = default,
            TaskScheduler taskScheduler = default)
        {
            if (cancellationToken == default)
                cancellationToken = CancellationToken.None;

            if (taskScheduler == default)
                taskScheduler = TaskScheduler.Current;

            return task.ContinueWith(t => followingAction(t.Result), cancellationToken, SuccessOptions, taskScheduler);
        }
    }
}
