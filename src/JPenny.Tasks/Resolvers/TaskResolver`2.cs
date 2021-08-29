using System;
using System.Threading.Tasks;

namespace JPenny.Tasks.Resolvers
{
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

        public TaskResolver(
            IPipelineTask<TPreviousResult> previousTask,
            Func<TPreviousResult, TResult> resolveFunc)
            :this(previousTask, (result) => Task.FromResult(resolveFunc(result)))
        {
        }

        public Task Resolve()
        {
            return _resolveFunc(_previousTask.Result);
        }
    }
}
