using System;
using System.Threading.Tasks;

namespace JPenny.Tasks.Resolvers
{
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
}
