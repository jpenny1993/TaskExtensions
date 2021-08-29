using System;
using System.Threading;
using System.Threading.Tasks;
using JPenny.Tasks.PipelineTasks;
using JPenny.Tasks.Resolvers;

namespace JPenny.Tasks.Builders
{
    public sealed class VoidTaskBuilder : TaskBuilderBase
    {
        private ITaskResolver SuccessTask { get; set; }

        internal VoidTaskBuilder()
        {
        }

        public VoidTaskBuilder Action(Task task)
        {
            MainTask = new TaskResolver(task);
            return this;
        }

        public VoidTaskBuilder Action(Action action)
        {
            MainTask = new TaskResolver(action, CancellationToken.None);
            return this;
        }

        public VoidTaskBuilder Action(Func<Task> taskResolver)
        {
            MainTask = new TaskResolver(taskResolver);
            return this;
        }

        public VoidTaskBuilder Catch(Action<Exception> onException)
            => Catch<Exception>(onException);

        public VoidTaskBuilder Catch<TException>(Action<TException> onException)
            where TException : Exception
        {
            AddExceptionHandler(onException);
            return this;
        }

        public VoidTaskBuilder OnCancelled(Action cancelledAction, CancellationToken cancellationToken = default)
        {
            CancelledTask = new TaskResolver(cancelledAction, cancellationToken);
            return this;
        }

        public VoidTaskBuilder OnCancelled(Task cancelledAction)
        {
            CancelledTask = new TaskResolver(cancelledAction);
            return this;
        }

        public VoidTaskBuilder OnSuccess(Action successAction, CancellationToken cancellationToken = default)
        {
            SuccessTask = new TaskResolver(successAction, cancellationToken);
            return this;
        }

        public VoidTaskBuilder OnSuccess(Func<Task> successAction)
        {
            SuccessTask = new TaskResolver(successAction);
            return this;
        }

        public VoidTaskBuilder OnSuccess(Task successTask)
        {
            SuccessTask = new TaskResolver(successTask);
            return this;
        }

        public VoidTaskBuilder OnCompleted(Action completedAction, CancellationToken cancellationToken = default)
        {
            CompletedTask = new TaskResolver(completedAction, cancellationToken);
            return this;
        }

        public VoidTaskBuilder OnCompleted(Task completedAction)
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
