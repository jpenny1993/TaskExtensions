using System;
using System.Threading;
using System.Threading.Tasks;
using JPenny.Tasks.PipelineTasks;
using JPenny.Tasks.Resolvers;

namespace JPenny.Tasks.Builders
{
    public sealed class ResultantTaskBuilder<TOutput> : TaskBuilderBase
    {
        private Func<TOutput, Task> SuccessTask { get; set; }

        internal ResultantTaskBuilder()
        {
        }

        public ResultantTaskBuilder<TOutput> Action(Task<TOutput> task)
        {
            MainTask = new TaskResolver(task);
            return this;
        }

        public ResultantTaskBuilder<TOutput> Action(Func<Task<TOutput>> taskResolver)
        {
            MainTask = new TaskResolver(taskResolver);
            return this;
        }

        public ResultantTaskBuilder<TOutput> Catch(Action<Exception> onException)
            => Catch<Exception>(onException);

        public ResultantTaskBuilder<TOutput> Catch<TException>(Action<TException> onException)
            where TException : Exception
        {
            AddExceptionHandler(onException);
            return this;
        }

        public ResultantTaskBuilder<TOutput> OnCancelled(Action cancelledAction, CancellationToken cancellationToken = default)
        {
            CancelledTask = new TaskResolver(cancelledAction, cancellationToken);
            return this;
        }

        public ResultantTaskBuilder<TOutput> OnCancelled(Task cancelledAction)
        {
            CancelledTask = new TaskResolver(cancelledAction);
            return this;
        }

        public ResultantTaskBuilder<TOutput> OnSuccess(Action successAction, CancellationToken cancellationToken = default)
        {
            SuccessTask = (result) => new Task(successAction, cancellationToken);
            return this;
        }

        public ResultantTaskBuilder<TOutput> OnSuccess(Action<TOutput> successAction, CancellationToken cancellationToken = default)
        {
            SuccessTask = (result) => new Task(() => successAction(result), cancellationToken);
            return this;
        }

        public ResultantTaskBuilder<TOutput> OnSuccess(Func<TOutput, Task> successAction)
        {
            SuccessTask = successAction;
            return this;
        }

        public ResultantTaskBuilder<TOutput> OnSuccess(Task successTask)
        {
            SuccessTask = (result) => successTask;
            return this;
        }

        public ResultantTaskBuilder<TOutput> OnCompleted(Action completedAction, CancellationToken cancellationToken = default)
        {
            CompletedTask = new TaskResolver(completedAction, cancellationToken);
            return this;
        }

        public ResultantTaskBuilder<TOutput> OnCompleted(Task completedAction)
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
