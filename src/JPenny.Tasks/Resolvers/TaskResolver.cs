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
}
