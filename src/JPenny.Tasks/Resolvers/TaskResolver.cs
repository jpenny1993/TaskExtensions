using System;
using System.Threading;
using System.Threading.Tasks;

namespace JPenny.Tasks.Resolvers
{
    public sealed class TaskResolver : ITaskResolver
    {
        private Func<Task> _resolveFunc;

        public TaskResolver(Func<Task> resolveFunc)
        {
            _resolveFunc = resolveFunc;
        }

        public TaskResolver(Action action, CancellationToken cancellationToken)
            : this(() => new Task(action, cancellationToken))
        {
        }

        public TaskResolver(Task task)
            : this(() => task)
        {
        }

        public Task Resolve()
        {
            return _resolveFunc();
        }
    }

    public class TaskResolver<TPreviousResult> : ITaskResolver
    {
        private IPipelineTask<TPreviousResult> _previousTask;
        private Func<TPreviousResult, Task> _resolveFunc;

        public TaskResolver(
            IPipelineTask<TPreviousResult> previousTask,
            Func<TPreviousResult, Task> resolveFunc)
        {
            _previousTask = previousTask;
            _resolveFunc = resolveFunc;
        }

        public TaskResolver(
            IPipelineTask<TPreviousResult> previousTask,
            Action<TPreviousResult> action)
            : this(previousTask, (result) => new Task(() => action(result)))
        {
        }

        public Task Resolve()
        {
            return _resolveFunc(_previousTask.Result);
        }
    }

    public class TaskResolver<TPreviousResult, TResult> : ITaskResolver
    {
        private IPipelineTask<TPreviousResult> _previousTask;
        Func<TPreviousResult, Task<TResult>> _resolveFunc;

        public TaskResolver(
            IPipelineTask<TPreviousResult> previousTask,
            Func<TPreviousResult, Task<TResult>> resolveFunc)
        {
            _previousTask = previousTask;
            _resolveFunc = resolveFunc;
        }

        public Task Resolve()
        {
            return _resolveFunc(_previousTask.Result);
        }
    }
}
