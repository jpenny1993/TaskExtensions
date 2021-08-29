using System.Threading;
using System.Threading.Tasks;

namespace JPenny.Tasks.PipelineTasks
{
    public sealed class ResultantTask<TResult> : PipelineTask, IPipelineTask, IPipelineTask<TResult>
    {
        private Task<TResult> _task;

        public TResult Result => _task.Result;

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _task = (Task<TResult>)MainTaskResolver.Resolve();
            return ExecuteAsync(_task, cancellationToken);
        }
    }
}
