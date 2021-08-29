using System;
using System.Threading;
using System.Threading.Tasks;
using JPenny.Tasks.PipelineTasks;
using JPenny.Tasks.Resolvers;

namespace JPenny.Tasks.Builders
{
    public sealed class VoidTaskBuilder<TInput> : TaskBuilderBase
    {
        private IPipelineTask<TInput> PreviousTask { get; }

        private ITaskResolver SuccessTask { get; set; }

        internal VoidTaskBuilder(IPipelineTask<TInput> previousTask)
        {
            PreviousTask = previousTask;
        }

        public VoidTaskBuilder<TInput> Action(Task task)
        {
            MainTask = new TaskResolver(task);
            return this;
        }

        public VoidTaskBuilder<TInput> Action(Action<TInput> action)
        {
            MainTask = new TaskResolver<TInput>(PreviousTask, action);
            return this;
        }

        public VoidTaskBuilder<TInput> Action(Func<TInput, Task> taskResolver)
        {
            MainTask = new TaskResolver<TInput>(PreviousTask, taskResolver);
            return this;
        }

        public VoidTaskBuilder<TInput> Catch(Action<Exception> onException)
            => Catch<Exception>(onException);

        public VoidTaskBuilder<TInput> Catch<TException>(Action<TException> onException)
            where TException : Exception
        {
            AddExceptionHandler(onException);
            return this;
        }

        public VoidTaskBuilder<TInput> OnCancelled(Action cancelledAction, CancellationToken cancellationToken = default)
        {
            CancelledTask = new TaskResolver(cancelledAction, cancellationToken);
            return this;
        }

        public VoidTaskBuilder<TInput> OnCancelled(Task cancelledAction)
        {
            CancelledTask = new TaskResolver(cancelledAction);
            return this;
        }

        public VoidTaskBuilder<TInput> OnSuccess(Action successAction, CancellationToken cancellationToken = default)
        {
            SuccessTask = new TaskResolver(successAction, cancellationToken);
            return this;
        }

        public VoidTaskBuilder<TInput> OnSuccess(Func<Task> taskResolverFunc)
        {
            SuccessTask = new TaskResolver(taskResolverFunc);
            return this;
        }

        public VoidTaskBuilder<TInput> OnSuccess(Task successTask)
        {
            SuccessTask = new TaskResolver(successTask);
            return this;
        }

        public VoidTaskBuilder<TInput> OnCompleted(Action completedAction, CancellationToken cancellationToken = default)
        {
            CompletedTask = new TaskResolver(completedAction, cancellationToken);
            return this;
        }

        public VoidTaskBuilder<TInput> OnCompleted(Task completedAction)
        {
            CompletedTask = new TaskResolver(completedAction);
            return this;
        }

        internal IPipelineTask Build()
        {
            return new VoidTask
            {
                ExceptionHandlers = ExceptionHandlers,
                MainTaskResolver = MainTask,
                CancelledTaskResolver = CancelledTask,
                SuccessTaskResolver = SuccessTask,
                CompletedTaskResolver = CompletedTask
            };
        }
    }
}
