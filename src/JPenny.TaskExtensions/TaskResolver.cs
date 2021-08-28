using System;
using System.Threading.Tasks;

namespace JPenny.TaskExtensions
{
    public interface ITaskResolver
    {
        Task Resolve();
    }

    public class TaskResolver : ITaskResolver
    {
        public static ITaskResolver Default { get; } = new TaskResolver(() => Task.CompletedTask);

        private Func<Task> _resolveFunc;

        public TaskResolver(Func<Task> resolveFunc)
        {
            _resolveFunc = resolveFunc;
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
