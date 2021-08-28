using System;
using System.Threading;
using System.Threading.Tasks;
using JPenny.TaskExtensions.Tasks;

namespace JPenny.TaskExtensions.Builders
{
    public sealed class ResultantTaskBuilder<TInput, TOutput> : TaskBuilder
    {
        private IPipelineTask<TInput> PreviousTask { get; }

        private Func<TOutput, Task> SuccessTask { get; set; }

        internal ResultantTaskBuilder(IPipelineTask<TInput> previousTask)
        {
            PreviousTask = previousTask;
        }

        public ResultantTaskBuilder<TInput, TOutput> Action(Action<TInput> action)
        {
            MainTask = new TaskResolver<TInput>(PreviousTask, action);
            return this;
        }

        public ResultantTaskBuilder<TInput, TOutput> Action(Func<TInput, Task<TOutput>> taskResolver)
        {
            MainTask = new TaskResolver<TInput, TOutput>(PreviousTask, taskResolver);
            return this;
        }

        public ResultantTaskBuilder<TInput, TOutput> Catch(Action<Exception> onException)
            => Catch<Exception>(onException);

        public ResultantTaskBuilder<TInput, TOutput> Catch<TException>(Action<TException> onException)
            where TException : Exception
        {
            AddExceptionHandler(onException);
            return this;
        }

        public ResultantTaskBuilder<TInput, TOutput> OnCancelled(Action cancelledAction, CancellationToken cancellationToken = default)
        {
            CancelledTask = new TaskResolver(cancelledAction, cancellationToken);
            return this;
        }

        public ResultantTaskBuilder<TInput, TOutput> OnCancelled(Task cancelledAction)
        {
            CancelledTask = new TaskResolver(cancelledAction);
            return this;
        }

        public ResultantTaskBuilder<TInput, TOutput> OnSuccess(Action successAction, CancellationToken cancellationToken = default)
        {
            SuccessTask = (result) => new Task(successAction, cancellationToken);
            return this;
        }

        public ResultantTaskBuilder<TInput, TOutput> OnSuccess(Action<TOutput> successAction, CancellationToken cancellationToken = default)
        {
            SuccessTask = (result) => new Task(() => successAction(result), cancellationToken);
            return this;
        }

        public ResultantTaskBuilder<TInput, TOutput> OnSuccess(Func<TOutput, Task> successAction)
        {
            SuccessTask = successAction;
            return this;
        }

        public ResultantTaskBuilder<TInput, TOutput> OnSuccess(Task successTask)
        {
            SuccessTask = (result) => successTask;
            return this;
        }

        public ResultantTaskBuilder<TInput, TOutput> OnCompleted(Action completedAction, CancellationToken cancellationToken = default)
        {
            CompletedTask = new TaskResolver(completedAction, cancellationToken);
            return this;
        }

        public ResultantTaskBuilder<TInput, TOutput> OnCompleted(Task completedAction)
        {
            CompletedTask = new TaskResolver(completedAction);
            return this;
        }

        internal IPipelineTask<TOutput> Build()
        {
            var task = new ResultantTask<TOutput>
            {
                ExceptionHandlers = ExceptionHandlers,
                MainTaskResolver = MainTask,
                CancelledTaskResolver = CancelledTask,
                CompletedTaskResolver = CompletedTask
            };

            if (SuccessTask != default)
            {
                task.SuccessTaskResolver = new TaskResolver<TOutput>(task, SuccessTask);
            }

            return task;
        }
    }
}
